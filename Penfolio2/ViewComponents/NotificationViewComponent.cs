using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Penfolio2.Data;
using Penfolio2.Models;


namespace Penfolio2.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public NotificationViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public IViewComponentResult Invoke(int? id = null, string? viewName = null)
        {
            var userId = GetUserId();

            if(userId == null)
            {
                if(!string.IsNullOrEmpty(viewName))
                {
                    if (viewName.CompareTo("CreateFriendRequest") == 0)
                    {
                        return View(viewName, new RequestFriendViewModel());
                    }
                    else
                    {
                        return View(viewName, new RequestAccessViewModel());
                    }
                }
                else
                {
                    return View(new NotificationViewModel());
                }
            }

            PenUser user = _db.PenUsers.Where(i => i.Id == userId).FirstOrDefault();

            if(user == null)
            {
                if (!string.IsNullOrEmpty(viewName))
                {
                    if(viewName.CompareTo("CreateFriendRequest") == 0)
                    {
                        return View(viewName, new RequestFriendViewModel());
                    }
                    else
                    {
                        return View(viewName, new RequestAccessViewModel());
                    }
                }
                else
                {
                    return View(new NotificationViewModel());
                }
            }

            if (!string.IsNullOrEmpty(viewName))
            {
                if(viewName.CompareTo("CreateFriendRequest") == 0)
                {
                    if(id == null)
                    {
                        return View(viewName, new RequestFriendViewModel());
                    }

                    var accessPermission = _db.AccessPermissions.Where(i => i.AccessPermissionId == id).FirstOrDefault();

                    if(accessPermission == null)
                    {
                        return View(viewName, new RequestFriendViewModel());
                    }

                    user = PopulatePenUserForFriendRequest(user);

                    List<AuthorsForFriendAccessViewModel> authors = new List<AuthorsForFriendAccessViewModel>();
                    List<PenProfile> penProfiles = new List<PenProfile>();

                    //if accessPermission is for a piece of writing
                    if(accessPermission.WritingId != null)
                    {
                        List<WritingProfile> writingProfiles = _db.WritingProfiles.Where(i => i.WritingId == accessPermission.WritingId).ToList();
                        penProfiles = user.PenProfiles.ToList();

                        foreach(var writingProfile in writingProfiles)
                        {
                            var profile = _db.PenProfiles.Where(i => i.ProfileId == writingProfile.ProfileId).FirstOrDefault();

                            if(profile != null)
                            {
                                List<IdentityError> errors = new List<IdentityError>();
                                bool isAnonymous = true;

                                if (IsAccessableByUser(profile.AccessPermissionId, ref errors, "search"))
                                {
                                    isAnonymous = false;
                                }

                                profile = PopulatePenProfileForFriendRequest(profile);

                                var author = new AuthorsForFriendAccessViewModel
                                {
                                    ProfileId = profile.ProfileId,
                                    DisplayName = isAnonymous ? "Anonymous" : profile.DisplayName,
                                    Friendships = profile.Friends,
                                    IsAnonymous = isAnonymous
                                };

                                authors.Add(author);
                            }
                        }
                    } //if accessPermission is for a profile
                    else if(accessPermission.ProfileId != null)
                    {
                        var receiverProfile = _db.PenProfiles.Where(i => i.ProfileId == accessPermission.ProfileId).FirstOrDefault();

                        if(receiverProfile != null)
                        {
                            receiverProfile = PopulatePenProfileForFriendRequest(receiverProfile);

                            foreach(var profile in user.PenProfiles)
                            {
                                if(!receiverProfile.Friends.Any(i => i.Active && i.SecondFriendId == profile.ProfileId) && !_db.FriendRequests.Any(i => i.Resolved == false && i.RequesterId == receiverProfile.ProfileId && i.RequesteeId == profile.ProfileId) && !_db.FriendRequests.Any(i => i.Resolved == false && i.RequesterId == profile.ProfileId && i.RequesteeId == receiverProfile.ProfileId))
                                {
                                    penProfiles.Add(profile);
                                }
                            }
                        }
                    }

                    var model = new RequestFriendViewModel
                    {
                        AccessPermissionId = id.Value,
                        AccessPermission = accessPermission,
                        PenProfiles = penProfiles,
                        Authors = authors
                    };

                    return View(viewName, model);
                }
                else if(viewName.CompareTo("CreateAccessRequest") == 0)
                {
                    if (id == null)
                    {
                        return View(viewName, new RequestAccessViewModel());
                    }

                    var accessPermission = _db.AccessPermissions.Where(i => i.AccessPermissionId == id).FirstOrDefault();

                    accessPermission = PopulateAccessPermission(accessPermission);

                    if (accessPermission == null)
                    {
                        return View(viewName, new RequestAccessViewModel());
                    }

                    user = PopulatePenUserForAccessRequest(user);

                    List<PenProfile> penProfiles = new List<PenProfile>();

                    foreach (var profile in user.PenProfiles)
                    {
                        if(!accessPermission.IndividualAccessGrants.Any(i => i.Active && i.GranteeId ==  profile.ProfileId) && !accessPermission.IndividualAccessRevokes.Any(i => i.Active && i.RevokeeId == profile.ProfileId) && !accessPermission.AccessRequests.Any(i => i.Resolved == false && i.RequesterId == profile.ProfileId))
                        {
                            penProfiles.Add(profile);
                        }
                    }

                    var model = new RequestAccessViewModel
                    {
                        AccessPermissionId = id.Value,
                        PenProfiles = penProfiles
                    };

                    return View(viewName, model);
                }
            } //if there's a viewName
            
            List<NotificationViewModel> notifications = new List<NotificationViewModel>();
            NotificationsViewModel notificationsView;

            user = PopulatePenUser(user);

            foreach(var profile in user.PenProfiles)
            {
                //for all the writings connected to this profile
                foreach(var writing in profile.ProfileWritings)
                {
                    if(writing.Writing != null && writing.Writing.AccessPermission != null)
                    {
                        //for all of the access requests connected to this writing
                        foreach(var accessRequest in writing.Writing.AccessPermission.AccessRequests.Where(i => i.Resolved == false))
                        {
                            NotificationViewModel notification = new NotificationViewModel
                            {
                                AccessRequest = accessRequest,
                                NotificationDate = accessRequest.RequestDate,
                                FriendRequest = null
                            };

                            notifications.Add(notification);
                        }
                    }
                } //foreach writing in profile.ProfileWritings

                //handle all of the access requests connected to this profile
                if(profile.AccessPermission != null)
                {
                    foreach(var accessRequest in profile.AccessPermission.AccessRequests.Where(i => i.Resolved == false))
                    {
                        NotificationViewModel notification = new NotificationViewModel
                        {
                            AccessRequest = accessRequest,
                            NotificationDate = accessRequest.RequestDate,
                            FriendRequest = null
                        };

                        notifications.Add(notification);
                    }
                }

                //handle all of the friend requests connected to this profile
                foreach(var friendRequest in profile.FriendRequestsReceived.Where(i => i.Resolved == false))
                {
                    NotificationViewModel notification = new NotificationViewModel
                    {
                        FriendRequest = friendRequest,
                        NotificationDate = friendRequest.RequestDate,
                        AccessRequest = null
                    };

                    notifications.Add(notification);
                }

            } //foreach profile in user.PenProfiles

            if(notifications.Count > 0)
            {
                notifications = notifications.OrderByDescending(i => i.NotificationDate).ToList();
            }

            notificationsView = new NotificationsViewModel
            {
                Notifications = notifications,
                PenProfiles = _db.PenProfiles.ToList(),
                Writings = _db.Writings.ToList(),
                Count = notifications.Where(i => i.NotificationDate > user.LastNotificationViewDate).Count()
            };

            return View(notificationsView);
        }

        public string? GetUserName()
        {
            return User?.Identity?.Name;
        }

        protected string? GetUserId()
        {
            var username = GetUserName();

            if (username == null)
            {
                return null;
            }

            return _db.PenUsers.Where(i => i.NormalizedUserName == username.ToUpper()).FirstOrDefault()?.Id;
        }

        protected PenUser PopulatePenUser(PenUser user)
        {
            List<PenProfile> profiles;
            List<PenProfile> populatedProfiles = new List<PenProfile>();

            //Populate PenProfiles Stage 1
            if(user.PenProfiles.Count == 0)
            {
                profiles = _db.PenProfiles.Where(i => i.UserId == user.Id).ToList();
            }
            else
            {
                profiles = user.PenProfiles.ToList();
            }

            //Populate PenProfiles Stage 2
            foreach(var profile in profiles)
            {
                populatedProfiles.Add(PopulatePenProfile(profile));
            }

            //Populate PenProfiles Stage 3
            user.PenProfiles = populatedProfiles;

            return user;
        }

        protected PenUser PopulatePenUserForFriendRequest(PenUser user)
        {
            List<PenProfile> profiles;
            List<PenProfile> populatedProfiles = new List<PenProfile>();

            //Populate PenProfiles Stage 1
            if (user.PenProfiles.Count == 0)
            {
                profiles = _db.PenProfiles.Where(i => i.UserId == user.Id).ToList();
            }
            else
            {
                profiles = user.PenProfiles.ToList();
            }

            //Populate PenProfiles Stage 2
            foreach (var profile in profiles)
            {
                populatedProfiles.Add(PopulatePenProfileForFriendRequest(profile));
            }

            //Populate PenProfiles Stage 3
            user.PenProfiles = populatedProfiles;

            return user;
        }

        protected PenUser PopulatePenUserForAccessRequest(PenUser user)
        {
            List<PenProfile> profiles;
            List<PenProfile> populatedProfiles = new List<PenProfile>();

            //Populate PenProfiles Stage 1
            if (user.PenProfiles.Count == 0)
            {
                profiles = _db.PenProfiles.Where(i => i.UserId == user.Id).ToList();
            }
            else
            {
                profiles = user.PenProfiles.ToList();
            }

            //Populate PenProfiles Stage 2
            foreach (var profile in profiles)
            {
                populatedProfiles.Add(PopulatePenProfileForAccessRequest(profile));
            }

            //Populate PenProfiles Stage 3
            user.PenProfiles = populatedProfiles;

            return user;
        }

        protected PenProfile PopulatePenProfile(PenProfile profile)
        {
            List<WritingProfile> writingProfiles;
            List<WritingProfile> populatedWritingProfiles = new List<WritingProfile>();

            List<FriendRequest> friendRequests;
            List<FriendRequest> populatedFriendRequests = new List<FriendRequest>();

            //Populate PenRole
            if(profile.PenRole == null)
            {
                profile.PenRole = _db.ProfileRoles.Where(i => i.RoleId == profile.RoleId).FirstOrDefault();
            }

            //Populate AccessPermission Stage 1
            if(profile.AccessPermission == null)
            {
                profile.AccessPermission = _db.AccessPermissions.Where(i => i.AccessPermissionId == profile.AccessPermissionId && i.ProfileId.HasValue && i.ProfileId == profile.ProfileId).FirstOrDefault();
            }

            //Populate AccessPermission Stage 2
            if(profile.AccessPermission != null)
            {
                profile.AccessPermission = PopulateAccessPermission(profile.AccessPermission);
            }

            //Populate ProfileWritings Stage 1
            if(profile.ProfileWritings.Count == 0)
            {
                writingProfiles = _db.WritingProfiles.Where(i => i.ProfileId == profile.ProfileId).ToList();
            }
            else
            {
                writingProfiles = profile.ProfileWritings.ToList();
            }

            //Populate ProfileWritings Stage 2
            foreach(var writingProfile in writingProfiles)
            {
                Writing? writing;

                if(writingProfile.Writing == null)
                {
                    writing = _db.Writings.Where(i => i.WritingId == writingProfile.WritingId).FirstOrDefault();
                }
                else
                {
                    writing = writingProfile.Writing;
                }

                if(writing != null)
                {
                    writing = PopulateWriting(writing);
                    writingProfile.Writing = writing;
                    populatedWritingProfiles.Add(writingProfile);
                }
            }

            //Populate ProfileWritings Stage 3
            profile.ProfileWritings = populatedWritingProfiles;

            //Populate FolderOwners; TBD because this is not implemented yet

            //Populate SeriesOwners; TBD because this is not implemented yet

            //Populate FriendRequestsRecieved Stage 1
            if(profile.FriendRequestsReceived.Count == 0)
            {
                friendRequests = _db.FriendRequests.Where(i => i.RequesteeId == profile.ProfileId).ToList();
            }
            else
            {
                friendRequests = profile.FriendRequestsReceived.ToList();
            }

            //Populate FriendRequestsReceived Stage 2
            foreach(var friendRequest in friendRequests)
            {
                PenProfile? requester;

                if(friendRequest.Requester == null)
                {
                    requester = _db.PenProfiles.Where(i => i.ProfileId == friendRequest.RequesterId).FirstOrDefault();
                }
                else
                {
                    requester = friendRequest.Requester;
                }

                if(requester != null)
                {
                    if(requester.PenRole == null)
                    {
                        requester.PenRole = _db.ProfileRoles.Where(i => i.RoleId == requester.RoleId).FirstOrDefault();
                    }

                    friendRequest.Requester = requester;
                    populatedFriendRequests.Add(friendRequest);
                }
            }

            //Populate FriendRequestReceived Stage 3
            profile.FriendRequestsReceived = populatedFriendRequests;

            //Populate CommentsMade; TBD because this is not implemented yet

            //Populate CommentsReceived; TBD because this is not implemented yet

            //Populate LikesReceived; TBD because this is not implemented yet

            //Populate Followers; TBD because this is not implemented yet

            //may or may not need to add something for following so we can get notifications about the things we are following?

            return profile;
        }

        protected PenProfile PopulatePenProfileForFriendRequest(PenProfile profile)
        {
            List<Friendship> friends;
            List<Friendship> populatedFriends = new List<Friendship>();

            List<FriendRequest> friendRequestsReceived;
            List<FriendRequest> populatedFriendRequestsReceived = new List<FriendRequest>();

            List<FriendRequest> friendRequestsSent;
            List<FriendRequest> populatedFriendRequestsSent = new List<FriendRequest>();

            //Populate PenRole
            if (profile.PenRole == null)
            {
                profile.PenRole = _db.ProfileRoles.Where(i => i.RoleId == profile.RoleId).FirstOrDefault();
            }

            //Populate AccessPermission
            if (profile.AccessPermission == null)
            {
                profile.AccessPermission = _db.AccessPermissions.Where(i => i.AccessPermissionId == profile.AccessPermissionId && i.ProfileId.HasValue && i.ProfileId == profile.ProfileId).FirstOrDefault();
            }

            //Populate Friends Stage 1
            if (profile.Friends.Count == 0)
            {
                friends = _db.Friendships.Where(i => i.FirstFriendId == profile.ProfileId && i.Active).ToList();
            }
            else
            {
                friends = profile.Friends.Where(i => i.Active).ToList();
            }

            //Populate Friends Stage 2
            foreach (var friend in friends)
            {
                PenProfile? secondFriend;

                if (friend.SecondFriend == null)
                {
                    secondFriend = _db.PenProfiles.Where(i => i.ProfileId == friend.SecondFriendId).FirstOrDefault();
                }
                else
                {
                    secondFriend = friend.SecondFriend;
                }

                if (secondFriend != null)
                {
                    if (secondFriend.PenRole == null)
                    {
                        secondFriend.PenRole = _db.ProfileRoles.Where(i => i.RoleId == secondFriend.RoleId).FirstOrDefault();
                    }

                    if(secondFriend.AccessPermission == null)
                    {
                        secondFriend.AccessPermission = _db.AccessPermissions.Where(i => i.AccessPermissionId ==  secondFriend.AccessPermissionId).FirstOrDefault();
                    }

                    friend.SecondFriend = secondFriend;
                    populatedFriends.Add(friend);
                }
            }

            //Populate Friends Stage 3
            profile.Friends = populatedFriends;

            //Populate FriendRequestsRecieved Stage 1
            if (profile.FriendRequestsReceived.Count == 0)
            {
                friendRequestsReceived = _db.FriendRequests.Where(i => i.RequesteeId == profile.ProfileId && i.Resolved == false).ToList();
            }
            else
            {
                friendRequestsReceived = profile.FriendRequestsReceived.Where(i => i.Resolved == false).ToList();
            }

            //Populate FriendRequestsReceived Stage 2
            foreach (var friendRequest in friendRequestsReceived)
            {
                PenProfile? requester;

                if (friendRequest.Requester == null)
                {
                    requester = _db.PenProfiles.Where(i => i.ProfileId == friendRequest.RequesterId).FirstOrDefault();
                }
                else
                {
                    requester = friendRequest.Requester;
                }

                if (requester != null)
                {
                    if (requester.PenRole == null)
                    {
                        requester.PenRole = _db.ProfileRoles.Where(i => i.RoleId == requester.RoleId).FirstOrDefault();
                    }

                    friendRequest.Requester = requester;
                    populatedFriendRequestsReceived.Add(friendRequest);
                }
            }

            //Populate FriendRequestReceived Stage 3
            profile.FriendRequestsReceived = populatedFriendRequestsReceived;

            //Populate FriendRequestsSent Stage 1
            if (profile.FriendRequestsSent.Count == 0)
            {
                friendRequestsSent = _db.FriendRequests.Where(i => i.RequesterId == profile.ProfileId && i.Resolved == false).ToList();
            }
            else
            {
                friendRequestsSent = profile.FriendRequestsSent.Where(i => i.Resolved == false).ToList();
            }

            //Populate FriendRequestsSent Stage 2
            foreach (var friendRequest in friendRequestsSent)
            {
                PenProfile? requestee;

                if (friendRequest.Requester == null)
                {
                    requestee = _db.PenProfiles.Where(i => i.ProfileId == friendRequest.RequesteeId).FirstOrDefault();
                }
                else
                {
                    requestee = friendRequest.Requestee;
                }

                if (requestee != null)
                {
                    if (requestee.PenRole == null)
                    {
                        requestee.PenRole = _db.ProfileRoles.Where(i => i.RoleId == requestee.RoleId).FirstOrDefault();
                    }

                    friendRequest.Requestee = requestee;
                    populatedFriendRequestsSent.Add(friendRequest);
                }
            }

            //Populate FriendRequestSent Stage 3
            profile.FriendRequestsSent = populatedFriendRequestsSent;

            return profile;
        }

        protected PenProfile PopulatePenProfileForAccessRequest(PenProfile profile)
        {
            //Populate PenRole
            if (profile.PenRole == null)
            {
                profile.PenRole = _db.ProfileRoles.Where(i => i.RoleId == profile.RoleId).FirstOrDefault();
            }

            //Populate AccessPermission
            if (profile.AccessPermission == null)
            {
                profile.AccessPermission = _db.AccessPermissions.Where(i => i.AccessPermissionId == profile.AccessPermissionId && i.ProfileId.HasValue && i.ProfileId == profile.ProfileId).FirstOrDefault();
            }

            profile.AccessPermission = PopulateAccessPermission(profile.AccessPermission);

            return profile;
        }

        protected Writing PopulateWriting(Writing writing)
        {
            //populate AccessPermission
            if(writing.AccessPermission == null)
            {
                AccessPermission? accessPermission = _db.AccessPermissions.Where(i => i.AccessPermissionId == writing.AccessPermissionId && i.WritingId == writing.WritingId).FirstOrDefault();

                if(accessPermission != null)
                {
                    accessPermission = PopulateAccessPermission(accessPermission);
                }

                writing.AccessPermission = accessPermission;
            }
            else if(writing.AccessPermission != null)
            {
                writing.AccessPermission = PopulateAccessPermission(writing.AccessPermission);
            }

            //populate Likes; TBD because it's not implemented yet

            //populate Comments; TBD because it's not implemented yet

            //populate Critiques; TBD because it's not implemented yet

            return writing;
        }

        protected AccessPermission PopulateAccessPermission(AccessPermission accessPermission)
        {
            List<AccessRequest> accessRequests;
            List<AccessRequest> populatedAccessRequests = new List<AccessRequest>();

            List<IndividualAccessGrant> individualAccessGrants;
            List<IndividualAccessGrant> populatedIndividualAccessGrants = new List<IndividualAccessGrant>();

            List<IndividualAccessRevoke> individualAccessRevokes;
            List<IndividualAccessRevoke> populatedIndividualAccessRevokes = new List<IndividualAccessRevoke>();

            //Populate AccessRequests Stage 1
            if (accessPermission.AccessRequests.Count == 0)
            {
                accessRequests = _db.AccessRequests.Where(i => i.AccessPermissionId == accessPermission.AccessPermissionId).ToList();
            }
            else
            {
                accessRequests = accessPermission.AccessRequests.ToList();
            }

            //Populate AccessRequests Stage 2
            foreach(var accessRequest in accessRequests)
            {
                //Populate Requester
                if(accessRequest.Requester == null)
                {
                    accessRequest.Requester = _db.PenProfiles.Where(i => i.ProfileId == accessRequest.RequesterId).FirstOrDefault();

                    if(accessRequest.Requester != null && accessRequest.Requester.PenRole == null)
                    {
                        accessRequest.Requester.PenRole = _db.ProfileRoles.Where(i => i.RoleId == accessRequest.Requester.RoleId).FirstOrDefault();
                    }
                }
                else if(accessRequest.Requester.PenRole == null)
                {
                    accessRequest.Requester.PenRole = _db.ProfileRoles.Where(i => i.RoleId == accessRequest.Requester.RoleId).FirstOrDefault();
                }

                //Populate AccessPermission
                if(accessRequest.AccessPermission == null)
                {
                    accessRequest.AccessPermission = accessPermission;
                }

                populatedAccessRequests.Add(accessRequest);
            }

            //Populate AcccessRequests Stage 3
            accessPermission.AccessRequests = populatedAccessRequests;

            //Populate IndividualAccessGrants Stage 1
            if (accessPermission.IndividualAccessGrants.Count == 0)
            {
                individualAccessGrants = _db.IndividualAccessGrants.Where(i => i.AccessPermissionId == accessPermission.AccessPermissionId).ToList();
            }
            else
            {
                individualAccessGrants = accessPermission.IndividualAccessGrants.ToList();
            }

            //Populate IndividualAccessGrants Stage 2
            foreach (var individualAccessGrant in individualAccessGrants)
            {
                //Populate Grantee
                if (individualAccessGrant.Grantee == null)
                {
                    individualAccessGrant.Grantee = _db.PenProfiles.Where(i => i.ProfileId == individualAccessGrant.GranteeId).FirstOrDefault();

                    if (individualAccessGrant.Grantee != null && individualAccessGrant.Grantee.PenRole == null)
                    {
                        individualAccessGrant.Grantee.PenRole = _db.ProfileRoles.Where(i => i.RoleId == individualAccessGrant.Grantee.RoleId).FirstOrDefault();
                    }
                }
                else if (individualAccessGrant.Grantee.PenRole == null)
                {
                    individualAccessGrant.Grantee.PenRole = _db.ProfileRoles.Where(i => i.RoleId == individualAccessGrant.Grantee.RoleId).FirstOrDefault();
                }

                //Populate AccessPermission
                if (individualAccessGrant.AccessPermission == null)
                {
                    individualAccessGrant.AccessPermission = accessPermission;
                }

                populatedIndividualAccessGrants.Add(individualAccessGrant);
            }

            //Populate IndividualAccessGrants Stage 3
            accessPermission.IndividualAccessGrants = populatedIndividualAccessGrants;

            //Populate IndividualAccessRevokes Stage 1
            if (accessPermission.IndividualAccessRevokes.Count == 0)
            {
                individualAccessRevokes = _db.IndividualAccessRevokes.Where(i => i.AccessPermissionId == accessPermission.AccessPermissionId).ToList();
            }
            else
            {
                individualAccessRevokes = accessPermission.IndividualAccessRevokes.ToList();
            }

            //Populate IndividualAccessRevokes Stage 2
            foreach (var individualAccessRevoke in individualAccessRevokes)
            {
                //Populate Revokee
                if (individualAccessRevoke.Revokee == null)
                {
                    individualAccessRevoke.Revokee = _db.PenProfiles.Where(i => i.ProfileId == individualAccessRevoke.RevokeeId).FirstOrDefault();

                    if (individualAccessRevoke.Revokee != null && individualAccessRevoke.Revokee.PenRole == null)
                    {
                        individualAccessRevoke.Revokee.PenRole = _db.ProfileRoles.Where(i => i.RoleId == individualAccessRevoke.Revokee.RoleId).FirstOrDefault();
                    }
                }
                else if (individualAccessRevoke.Revokee.PenRole == null)
                {
                    individualAccessRevoke.Revokee.PenRole = _db.ProfileRoles.Where(i => i.RoleId == individualAccessRevoke.Revokee.RoleId).FirstOrDefault();
                }

                //Populate AccessPermission
                if (individualAccessRevoke.Revokee == null)
                {
                    individualAccessRevoke.AccessPermission = accessPermission;
                }

                populatedIndividualAccessRevokes.Add(individualAccessRevoke);
            }

            //Populate IndividualAccessRevokes Stage 3
            accessPermission.IndividualAccessRevokes = populatedIndividualAccessRevokes;

            return accessPermission;
        }

        protected bool UserIsMinor(PenUser user)
        {
            DateTime now = DateTime.Today;

            //if they have entered a birthday
            if (user.Birthdate != null)
            {
                //get their birthday
                DateTime birthdate = user.Birthdate.Value;

                //if they are 18 or older, they are not a minor
                if (now.Year - birthdate.Year > 18 || (now.Year - birthdate.Year == 18 && (now.Month > birthdate.Month || (now.Month == birthdate.Month && now.Day >= birthdate.Day))))
                {
                    return false;
                }
            }

            return true;
        }

        protected bool UserHasEnteredBirthdate(PenUser user)
        {
            if (user.Birthdate == null)
            {
                return false;
            }

            return true;
        }

        protected ICollection<PenProfile> GetVerifiedPublisherProfiles(string userId)
        {
            List<PenProfile> penProfiles = _db.PenProfiles.Where(i => i.UserId == userId && i.RoleId == 2 && i.Verified).ToList();
            List<PenProfile> profiles = new List<PenProfile>();

            for (int i = 0; i < penProfiles.Count; i++)
            {
                var penProfile = penProfiles.ElementAt(i);
                penProfile = PopulatePenProfile(penProfile);
                profiles.Add(penProfile);
            }

            return penProfiles.Equals(profiles) ? penProfiles : profiles;
        }

        protected bool UserHasPublisherProfile()
        {
            return _db.PenProfiles.Any(i => i.UserId == GetUserId() && i.RoleId == 2);
        }

        protected bool UserHasVerifiedPublisherProfile()
        {
            return _db.PenProfiles.Any(i => i.UserId == GetUserId() && i.RoleId == 2 && i.Verified);
        }

        protected bool IsAccessableByUser(int accessPermissionId, ref List<IdentityError> errors, string? action)
        {
            if (action == null)
            {
                action = "";
            }

            action = action.ToLower().Trim();

            //if the error list doesn't exist, create it
            if (errors == null)
            {
                errors = new List<IdentityError>();
            }

            string? userId = GetUserId();

            if (userId == null)
            {
                errors.Add(new IdentityError
                {
                    Description = "User not found."
                });

                return false;
            }

            PenUser user = _db.PenUsers.Where(i => i.Id == userId).First();

            //get a list of all of this user's profiles
            List<int> userProfileIds = user.PenProfiles.ToList().Select(i => i.ProfileId).ToList();
            //get the access permission from the database by id
            AccessPermission accessPermission = _db.AccessPermissions.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault();

            //if the access permission doesn't exist, return false
            if (accessPermission == null)
            {
                errors.Add(new IdentityError
                {
                    Description = "Request not found."
                });

                return false;
            }

            //a string to mark whether this access permission is for a profile, a folder, a series, or a piece of writing
            string accessType = accessPermission.ProfileId != null ? "profile" : accessPermission.FolderId != null ? "folder" : accessPermission.SeriesId != null ? "series" : accessPermission.WritingId != null ? "writing" : "";

            //if this AccessPermission was never assigned an id it is relating to, return false
            if (accessPermission.ProfileId is null && accessPermission.WritingId is null && accessPermission.FolderId is null && accessPermission.SeriesId is null)
            {
                errors.Add(new IdentityError
                {
                    Description = "Request not found."
                });

                return false;
            }

            //a user can always access their own content
            //if the AccessPermission is for a PenProfile owned by the user, return true
            if (_db.PenProfiles.Any(i => i.AccessPermissionId == accessPermissionId && i.UserId == userId))
            {
                return true;
            } //if it's a profile, they aren't allowed to edit it unless it's their own
            else if (accessType == "profile" && (action == "edit" || action == "delete"))
            {
                errors.Add(new IdentityError
                {
                    Description = "You are not allowed to " + action + " a profile you are not the owner of."
                });

                return false;
            } //if the AccessPermission is for a piece of writing
            else if (_db.Writings.Any(i => i.AccessPermissionId == accessPermissionId))
            {
                //get the writing
                Writing writing = _db.Writings.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault();

                //if there's no writing with that AccessPermissionId, return false
                if (writing == null)
                {
                    errors.Add(new IdentityError
                    {
                        Description = "Writing not found."
                    });

                    return false;
                }

                //if the Writing was created by the user, return true
                if (writing.UserId == userId)
                {
                    return true;
                } //if the writing was not created by the user and the action is delete
                else if (action == "delete")
                {
                    //writing may be edited by collaborators, but it can only be deleted by its owners
                    errors.Add(new IdentityError
                    {
                        Description = "You may not " + action + " a piece of writing that you do not own."
                    });

                    return false;
                } //if the writing was not created by the user
                else
                {
                    //get a list of all profiles that are connected to this piece of writing
                    List<WritingProfile> writingProfiles = _db.WritingProfiles.Where(i => i.WritingId == writing.WritingId).ToList();

                    foreach (var writingProfile in writingProfiles)
                    {
                        //if the user has a profile that is a collaborator on this piece of writing
                        if (userProfileIds.Contains(writingProfile.ProfileId))
                        {
                            return true;
                        }
                    }
                } //if the writing was not created by the user

                //if they have reached this point, they are not an owner or collaborator, so create an error message and return false
                if (action == "edit")
                {
                    errors.Add(new IdentityError
                    {
                        Description = "You may not " + action + " a piece of writing that you do not own or which you are not a collaborator on."
                    });

                    return false;
                }
            } //if the AccessPermission is for a Folder
            else if (_db.Folders.Any(i => i.AccessPermissionId == accessPermissionId))
            {
                //get the folder
                Folder folder = _db.Folders.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault();

                //if there's no folder with that AccessPermissionId, return false
                if (folder == null)
                {
                    errors.Add(new IdentityError
                    {
                        Description = "Folder not found."
                    });

                    return false;
                }

                //if the user created the folder, return true
                if (folder.CreatorId == userId)
                {
                    return true;
                } //if the folder was not created by the user and the action is delete
                else if (action == "delete")
                {
                    //folders may be edited by collaborators, but they can only be deleted by their owners
                    errors.Add(new IdentityError
                    {
                        Description = "You may not " + action + " a folder that you do not own."
                    });

                    return false;
                } //if the folder was not created by the user
                else
                {
                    //get a list of all the profiles connected to the folder
                    List<FolderOwner> folderOwners = _db.FolderOwners.Where(i => i.FolderId == folder.FolderId).ToList();

                    foreach (var folderOwner in folderOwners)
                    {
                        //if the user has a profile that is a contributor to this folder, return true
                        if (userProfileIds.Contains(folderOwner.OwnerId))
                        {
                            return true;
                        }
                    }
                } //if the folder was not created by the user

                //if they have reached this point, they are not an owner or a contributor, so create an error message and return false
                if (action == "edit")
                {
                    errors.Add(new IdentityError
                    {
                        Description = "You may not " + action + " a folder that you do not own or which you are not a contributor on."
                    });

                    return false;
                }
            } //if the AccessPermission matches to a Series
            else if (_db.Series.Any(i => i.AccessPermissionId == accessPermissionId))
            {
                //get the series
                Series series = _db.Series.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault();

                //if there's no series with that AccessPermissionId, return false
                if (series == null)
                {
                    errors.Add(new IdentityError
                    {
                        Description = "Series not found."
                    });

                    return false;
                }

                //if the user is the creator of this series, return true
                if (series.CreatorId == userId)
                {
                    return true;
                } //if the series was not created by the user and the action is delete
                else if (action == "delete")
                {
                    //series may be edited by collaborators, but they can only be deleted by their owners
                    errors.Add(new IdentityError
                    {
                        Description = "You may not " + action + " a series that you do not own."
                    });

                    return false;
                } //if the user is not the creator of the series
                else
                {
                    //get a list of profiles connected to the series
                    List<SeriesOwner> seriesOwners = _db.SeriesOwners.Where(i => i.SeriesId == series.SeriesId).ToList();

                    foreach (var seriesOwner in seriesOwners)
                    {
                        //if one of the profiles owned by the user is a contributor to this series, return true
                        if (userProfileIds.Contains(seriesOwner.OwnerId))
                        {
                            return true;
                        }
                    }
                } // if the user is not the creator of the series

                //if they have reached this point, they are not an owner or contributor, so create an error message and return false
                if (action == "edit")
                {
                    errors.Add(new IdentityError
                    {
                        Description = "You may not " + action + " a series that you do not own or which you are not a contributor on."
                    });

                    return false;
                }
            } //if the AccessPermission is for a Series

            //if the user is a minor and minor access has not been granted, we do not allow permission even in the case of an individual access grant having been given
            if (accessPermission.MinorAccess == false && UserIsMinor(user))
            {
                if (UserHasEnteredBirthdate(user))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "This " + accessType + " is not accessable by minors."
                    });
                }
                else
                {
                    errors.Add(new IdentityError
                    {
                        Description = "This " + accessType + " is not accessable by minors or those who have not entered their date of birth for their account. If you are not a minor, you may be able to access this " + accessType + " by editing your user account to include your birthday."
                    });
                }
                return false;
            }

            //get a list of user blocks for this particular user that are still active
            List<UserBlock> userBlocks = _db.UserBlocks.Where(i => i.BlockedUserId == userId && i.Active).ToList();

            //return false if the creator of the item connected to the access permission blocked the current user
            foreach (var block in userBlocks)
            {
                if (_db.PenProfiles.Any(i => i.AccessPermissionId == accessPermissionId && i.UserId == block.BlockingUserId)
                    || _db.Writings.Any(i => i.AccessPermissionId == accessPermissionId && i.UserId == block.BlockingUserId)
                    || _db.Folders.Any(i => i.AccessPermissionId == accessPermissionId && i.CreatorId == block.BlockingUserId)
                    || _db.Series.Any(i => i.AccessPermissionId == accessPermissionId && i.CreatorId == block.BlockingUserId))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "Your account has been blocked from accessing this " + accessType + " by its owner."
                    });

                    return false;
                }
            }

            //past this point, we know that a user has not been turned away because of being a minor or because they were blocked,
            //so now we check to see if the action being performed is a search
            if (action == "search" && accessPermission.ShowsUpInSearch)
            {
                return true;
            }

            //get a list of individual access grants for this AccessPermissionId where the grant is still active
            List<IndividualAccessGrant> individualAccessGrants = _db.IndividualAccessGrants.Where(i => i.AccessPermissionId == accessPermissionId && i.Active).ToList();
            //get a list of individual access revokes for this AccessPermissionId where the revoke is still active
            List<IndividualAccessRevoke> individualAccessRevokes = _db.IndividualAccessRevokes.Where(i => i.AccessPermissionId == accessPermissionId && i.Active).ToList();
            //get a list of all of this user's profiles so we can remove any ids that don't belong to them from the list of individual access grants and revokes
            List<int> overlappingProfileIds = new List<int>();

            //remove anything from the list of individual access grants that doesn't apply to us
            foreach (var item in individualAccessGrants)
            {
                //if the GranteeId isn't one of the user's profiles
                if (!userProfileIds.Contains(item.GranteeId))
                {
                    //remove the unrelated grant
                    individualAccessGrants.Remove(item);
                } //if the GranteeId is one of the user's profiles and there's also a revoke with a matching RevokeeId in the list of individual access revokes, add the ProfileId to overlappingProfileIds
                else if (individualAccessRevokes.Any(i => i.RevokeeId == item.GranteeId))
                {
                    overlappingProfileIds.Add(item.GranteeId);
                }
            }

            //remove anything from the list of individual access revokes that doesn't apply to us
            foreach (var item in individualAccessRevokes)
            {
                //if the RevokeeId isn't one of the user's profiles
                if (!userProfileIds.Contains(item.RevokeeId))
                {
                    //remove the unrelated revoke
                    individualAccessRevokes.Remove(item);
                }
            }

            IndividualAccessGrant? mostRecentGrant;
            IndividualAccessRevoke? mostRecentRevoke;

            //if there are any overlapping access grants and revokes, we need to settle them
            foreach (var item in overlappingProfileIds)
            {
                //get all the individual access grants for this ProfileId where the grant is active
                List<IndividualAccessGrant> grants = individualAccessGrants.Where(i => i.GranteeId == item && i.Active).ToList();
                //get all the individual access revokes for this ProfileId where the revoke is active
                List<IndividualAccessRevoke> revokes = individualAccessRevokes.Where(i => i.RevokeeId == item && i.Active).ToList();
                //get the most recent grant
                mostRecentGrant = grants.OrderByDescending(i => i.GrantDate).FirstOrDefault();
                //get the most recent revoke
                mostRecentRevoke = revokes.OrderByDescending(i => i.RevokeDate).FirstOrDefault();

                //if there is more than one active grant in the list, our first objective is to find the most recent grant and make all of the older grants inactive
                if (grants.Count > 1)
                {
                    //go through all of the grants
                    foreach (var grant in grants)
                    {
                        //if it's not the most recent grant, set active to false, update it in the database, and then remove it from the list
                        if (!grant.Equals(mostRecentGrant))
                        {
                            grant.Active = false;
                            _db.Entry(grant).State = EntityState.Modified;
                            _db.SaveChanges();
                            grants.Remove(grant);
                        }
                    }
                } //if there's more than one grant

                //if there is more than one active revoke in the list, our first objective is to find the most recent revoke and make all of the older revokes inactive
                if (revokes.Count > 1)
                {
                    //go through all the revokes
                    foreach (var revoke in revokes)
                    {
                        //if it's not the most recent revoke, set active to false, update it in the database, and then remove it from the list
                        if (!revoke.Equals(mostRecentRevoke))
                        {
                            revoke.Active = false;
                            _db.Entry(revoke).State = EntityState.Modified;
                            _db.SaveChanges();
                            revokes.Remove(revoke);
                        }
                    }
                } //if there's more than one revoke

                //there should now only be one revoke and one grant their respective lists
                //compare the dates and set the older one to no longer be active and then remove it from its list
                if (mostRecentGrant.GrantDate > mostRecentRevoke.RevokeDate)
                {
                    mostRecentRevoke.Active = false;
                    _db.Entry(mostRecentRevoke).State = EntityState.Modified;
                    _db.SaveChanges();
                    individualAccessRevokes.Remove(mostRecentRevoke);
                }
                else
                {
                    mostRecentGrant.Active = false;
                    _db.Entry(mostRecentGrant).State = EntityState.Modified;
                    _db.SaveChanges();
                    individualAccessGrants.Remove(mostRecentGrant);
                }

                overlappingProfileIds.Remove(item);
            } //foreach overlappingProfileId

            //there should no longer be any overlapping profile IDs, and the individualAccessGrants and individualAccessRevokes should only contain relevant information now
            //let's update the mostRecentGrant and mostRecentRevoke
            mostRecentGrant = individualAccessGrants.OrderByDescending(i => i.GrantDate).FirstOrDefault();
            mostRecentRevoke = individualAccessRevokes.OrderByDescending(i => i.RevokeDate).FirstOrDefault();

            //if they have individual access revokes but no grants, return false
            if (individualAccessRevokes.Count > 0 && individualAccessGrants.Count == 0)
            {
                errors.Add(new IdentityError
                {
                    Description = "You have had your access revoked by the owner of this " + accessType + "."
                });

                return false;
            } //if they have individual access grants and no revokes, return true
            else if (individualAccessGrants.Count > 0 && individualAccessRevokes.Count == 0)
            {
                return true;
            } //if there are both individual access grants and individual access revokes
            else if (individualAccessGrants.Count > 0 && individualAccessRevokes.Count > 0)
            {
                if (mostRecentGrant.GrantDate > mostRecentRevoke.RevokeDate)
                {
                    return true;
                }
                else
                {
                    errors.Add(new IdentityError
                    {
                        Description = "You have had your access revoked by the owner of this " + accessType + "."
                    });

                    return false;
                }
            }

            //if you made it this far, you no longer need to worry about user blocks, individual access revokes and grants, or minor access
            //if there is public access, return true
            if (accessPermission.PublicAccess)
            {
                return true;
            } //if there's not public access
            else
            {
                //if there's publisher access and the user has at least one verified publisher account, return true
                if (accessPermission.PublisherAccess && GetVerifiedPublisherProfiles(userId).Count > 0)
                {
                    return true;
                }

                //if there's friend access, let's check if we are friends
                if (accessPermission.FriendAccess)
                {
                    List<Friendship> friendships = new List<Friendship>();

                    foreach (var userProfileId in userProfileIds)
                    {
                        //get all the friendships for this profile id
                        List<Friendship> friends = _db.Friendships.Where(i => i.FirstFriendId == userProfileId && i.Active).ToList();

                        foreach (var friend in friends)
                        {
                            friendships.Add(friend);
                        }
                    }

                    //if this is the access permission for a profile and the user has a profile that has a friendship with this profile, return true
                    if (accessPermission.ProfileId != null && friendships.Any(i => i.SecondFriendId == accessPermission.ProfileId))
                    {
                        return true;
                    } //if the AccessPermission is for a piece of Writing
                    else if (accessPermission.WritingId != null)
                    {
                        List<PenProfile> writerProfiles = _db.Writings.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault().PenUser.PenProfiles.ToList();

                        foreach (var friendship in friendships)
                        {
                            if (writerProfiles.Any(i => i.ProfileId == friendship.SecondFriendId))
                            {
                                return true;
                            }
                        }
                    } //if the AccessPermission is for a Folder
                    else if (accessPermission.FolderId != null)
                    {
                        List<FolderOwner> folderOwners = _db.Folders.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault().Owners.ToList();

                        foreach (var friendship in friendships)
                        {
                            if (folderOwners.Any(i => i.OwnerId == friendship.SecondFriendId))
                            {
                                return true;
                            }
                        }
                    } //if the AccessPermission is for a Series
                    else if (accessPermission.SeriesId != null)
                    {
                        List<SeriesOwner> seriesOwners = _db.Series.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault().Owners.ToList();

                        foreach (var friendship in friendships)
                        {
                            if (seriesOwners.Any(i => i.OwnerId == friendship.SecondFriendId))
                            {
                                return true;
                            }
                        }
                    } //if it's a series
                } //if there's friend access

                if (accessPermission.PublisherAccess && accessPermission.FriendAccess)
                {
                    if (UserHasPublisherProfile() && !UserHasVerifiedPublisherProfile())
                    {
                        errors.Add(new IdentityError
                        {
                            Description = "This " + accessType + " is only accessable by verified publishers and friends. To gain access, have your publisher account verified, send a friend request, or request individual access."
                        });
                    }
                    else
                    {
                        errors.Add(new IdentityError
                        {
                            Description = "This " + accessType + " is only accessable by verified publishers and friends. To gain access, send a friend request or request individual access."
                        });
                    }
                }
                else if (accessPermission.PublisherAccess)
                {
                    if (UserHasPublisherProfile() && !UserHasVerifiedPublisherProfile())
                    {
                        errors.Add(new IdentityError
                        {
                            Description = "This " + accessType + " is only accessable by verified publishers. To gain access, have your publisher account verified or request individual access."
                        });
                    }
                    else
                    {
                        errors.Add(new IdentityError
                        {
                            Description = "This " + accessType + " is only accessable by verified publishers. To gain access, request individual access."
                        });
                    }
                }
                else if (accessPermission.FriendAccess)
                {
                    errors.Add(new IdentityError
                    {
                        Description = "This " + accessType + " is only accessable by friends. To gain access, send a friend request or request individual access."
                    });
                }
                else
                {
                    errors.Add(new IdentityError
                    {
                        Description = "This " + accessType + " is only accessable by owner permission. To gain access, request individual access."
                    });
                }
            } //if there's not public access

            //if you somehow manage to make it this far, return false
            return false;
        }
    }
}
