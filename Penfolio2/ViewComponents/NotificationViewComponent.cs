using Microsoft.AspNetCore.Mvc;
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

        public IViewComponentResult Invoke()
        {
            var userId = GetUserId();
            List<NotificationViewModel> notifications = new List<NotificationViewModel>();
            NotificationsViewModel notificationsView;

            if(userId == null)
            {
                return View(new NotificationsViewModel());
            }

            PenUser user = _db.PenUsers.Where(i => i.Id == userId).FirstOrDefault();

            if (user == null)
            {
                return View(new NotificationsViewModel());
            }

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

            return accessPermission;
        }
    }
}
