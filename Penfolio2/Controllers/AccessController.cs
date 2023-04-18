using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Penfolio2.Areas.Identity.Pages.Account;
using Penfolio2.Data;
using Penfolio2.Models;

namespace Penfolio2.Controllers
{
    [Authorize]
    public class AccessController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly SignInManager<PenUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public AccessController(
        ApplicationUserManager userManager,
        SignInManager<PenUser> signInManager,
        IEmailSender emailSender,
        ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = loggerFactory.CreateLogger<AccessController>();
        }

        public AccessController(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        public AccessController()
        {

        }

        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsAuthenticated()
        {
            return User.Identity != null ? User.Identity.IsAuthenticated : false;
        }

        public bool CheckLogin()
        {
            if (User.Identity != null ? User.Identity.IsAuthenticated : false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected string GetUserId()
        {
            return db.PenUsers.Where(i => i.NormalizedUserName == User.Identity.Name.ToUpper()).FirstOrDefault().Id;
        }

        public string? GetUserName()
        {
            return User?.Identity?.Name;
        }

        protected PenUser GetUserById(string id)
        {
            var user = db.PenUsers.Where(i => i.Id == id).First();

            user = PopulatePenUser(user);

            return user;
        }

        protected PenUser? GetUserByEmail(string email)
        {
            string normalizedEmail = CustomUsernameEmailPolicy.NormalizeEmail(email);
            var user = db.PenUsers.Where(e => e.NormalizedEmail == normalizedEmail).FirstOrDefault();

            if (user != null)
            {
                user = PopulatePenUser(user);
            }

            return user;
        }

        protected PenProfile? GetProfileById(int id)
        {
            var penProfile = db.PenProfiles.Where(i => i.ProfileId == id).FirstOrDefault();

            if (penProfile != null)
            {
                penProfile = PopulatePenProfile(penProfile);
            }

            return penProfile;
        }

        protected bool UserHasProfile()
        {
            return db.PenProfiles.Any(i => i.UserId == GetUserId());
        }

        protected bool UserHasWriterProfile()
        {
            return db.PenProfiles.Any(i => i.UserId == GetUserId() && i.RoleId == 1);
        }

        protected bool UserHasPublisherProfile()
        {
            return db.PenProfiles.Any(i => i.UserId == GetUserId() && i.RoleId == 2);
        }

        protected bool UserHasVerifiedPublisherProfile()
        {
            return db.PenProfiles.Any(i => i.UserId == GetUserId() && i.RoleId == 2 && i.Verified);
        }

        protected bool ProfileBelongsToUser(int id)
        {
            var penProfile = GetProfileById(id);

            if (penProfile != null && penProfile.UserId == GetUserId())
            {
                return true;
            }

            return false;
        }

        protected bool ProfileExistsAtUrlString(string urlString)
        {
            urlString = urlString.ToLower();
            return db.PenProfiles.Any(i => i.UrlString == urlString);
        }

        protected PenProfile? GetProfileFromUrlString(string urlString)
        {
            if (urlString == null)
            {
                return null;
            }

            urlString = urlString.ToLower();
            var penProfile = db.PenProfiles.Where(i => i.UrlString == urlString).FirstOrDefault();

            if (penProfile != null)
            {
                penProfile = PopulatePenProfile(penProfile);
            }

            return penProfile;
        }

        protected int GetProfileIdFromUrlString(string urlString)
        {
            urlString = urlString.ToLower();
            return ProfileExistsAtUrlString(urlString) ? db.PenProfiles.Where(i => i.UrlString == urlString).FirstOrDefault().ProfileId : 0;
        }

        protected bool UserHasEnteredBirthdate()
        {
            string userId = GetUserId();
            PenUser user = GetUserById(userId);

            if(user.Birthdate == null)
            {
                return false;
            }

            return true;
        }

        protected bool UserHasEnteredBirthdate(string userId)
        {
            PenUser user = GetUserById(userId);

            if (user.Birthdate == null)
            {
                return false;
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

        protected bool UserIsMinor()
        {
            string userId = GetUserId();
            PenUser user = GetUserById(userId);
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

        protected bool UserIsMinor(string userId)
        {
            PenUser user = GetUserById(userId);
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

        //maybe modify to return errors? TBD
        protected bool IsAccessableByUser(int accessPermissionId, ref List<IdentityError> errors)
        {
            //if the error list doesn't exist, create it
            if(errors == null)
            {
                errors = new List<IdentityError>();
            }

            string userId = GetUserId();
            PenUser user = GetUserById(userId);
            //get a list of all of this user's profiles
            List<int> userProfileIds = user.PenProfiles.ToList().Select(i => i.ProfileId).ToList();
            //get the access permission from the database by id
            AccessPermission accessPermission = db.AccessPermissions.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault();

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
            if (db.PenProfiles.Any(i => i.AccessPermissionId == accessPermissionId && i.UserId == userId))
            {
                return true;
            } //if the AccessPermission is for a piece of writing
            else if (db.Writings.Any(i => i.AccessPermissionId == accessPermissionId))
            {
                //get the writing
                Writing writing = db.Writings.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault();

                //if the Writing was created by the user, return true
                if (writing.UserId == userId)
                {
                    return true;
                } //if the writing was not created by the user
                else
                {
                    //get a list of all profiles that are connected to this piece of writing
                    List<WritingProfile> writingProfiles = db.WritingProfiles.Where(i => i.WritingId == writing.WritingId).ToList();

                    foreach (var writingProfile in writingProfiles)
                    {
                        //if the user has a profile that is a collaborator on this piece of writing
                        if (userProfileIds.Contains(writingProfile.ProfileId))
                        {
                            return true;
                        }
                    }
                } //if the writing was not created by the user
            } //if the AccessPermission is for a Folder
            else if (db.Folders.Any(i => i.AccessPermissionId == accessPermissionId))
            {
                //get the folder
                Folder folder = db.Folders.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault();

                //if the user created the folder, return true
                if (folder.CreatorId == userId)
                {
                    return true;
                } //if the folder was not created by the user
                else
                {
                    //get a list of all the profiles connected to the folder
                    List<FolderOwner> folderOwners = db.FolderOwners.Where(i => i.FolderId == folder.FolderId).ToList();

                    foreach (var folderOwner in folderOwners)
                    {
                        //if the user has a profile that is a contributor to this folder, return true
                        if (userProfileIds.Contains(folderOwner.OwnerId))
                        {
                            return true;
                        }
                    }
                } //if the folder was not created by the user
            } //if the AccessPermission matches to a Series
            else if (db.Series.Any(i => i.AccessPermissionId == accessPermissionId))
            {
                //get the series
                Series series = db.Series.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault();

                //if the user is the creator of this series, return true
                if (series.CreatorId == userId)
                {
                    return true;
                } //if the user is not the creator of the series
                else
                {
                    //get a list of profiles connected to the series
                    List<SeriesOwner> seriesOwners = db.SeriesOwners.Where(i => i.SeriesId == series.SeriesId).ToList();

                    foreach (var seriesOwner in seriesOwners)
                    {
                        //if one of the profiles owned by the user is a contributor to this series, return true
                        if (userProfileIds.Contains(seriesOwner.OwnerId))
                        {
                            return true;
                        }
                    }
                } // if the user is not the creator of the series
            } //if the AccessPermission is for a Series

            //if the user is a minor and minor access has not been granted, we do not allow permission even in the case of an individual access grant having been given
            if (accessPermission.MinorAccess == false && UserIsMinor(user))
            {
                if(UserHasEnteredBirthdate(user))
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
            List<UserBlock> userBlocks = db.UserBlocks.Where(i => i.BlockedUserId == userId && i.Active).ToList();

            //return false if the creator of the item connected to the access permission blocked the current user
            foreach (var block in userBlocks)
            {
                if (db.PenProfiles.Any(i => i.AccessPermissionId == accessPermissionId && i.UserId == block.BlockingUserId)
                    || db.Writings.Any(i => i.AccessPermissionId == accessPermissionId && i.UserId == block.BlockingUserId)
                    || db.Folders.Any(i => i.AccessPermissionId == accessPermissionId && i.CreatorId == block.BlockingUserId)
                    || db.Series.Any(i => i.AccessPermissionId == accessPermissionId && i.CreatorId == block.BlockingUserId))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "Your account has been blocked from accessing this " + accessType + " by its owner."
                    });

                    return false;
                }
            }

            //get a list of individual access grants for this AccessPermissionId where the grant is still active
            List<IndividualAccessGrant> individualAccessGrants = db.IndividualAccessGrants.Where(i => i.AccessPermissionId == accessPermissionId && i.Active).ToList();
            //get a list of individual access revokes for this AccessPermissionId where the revoke is still active
            List<IndividualAccessRevoke> individualAccessRevokes = db.IndividualAccessRevokes.Where(i => i.AccessPermissionId == accessPermissionId && i.Active).ToList();
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
                            db.Entry(grant).State = EntityState.Modified;
                            db.SaveChanges();
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
                            db.Entry(revoke).State = EntityState.Modified;
                            db.SaveChanges();
                            revokes.Remove(revoke);
                        }
                    }
                } //if there's more than one revoke

                //there should now only be one revoke and one grant their respective lists
                //compare the dates and set the older one to no longer be active and then remove it from its list
                if (mostRecentGrant.GrantDate > mostRecentRevoke.RevokeDate)
                {
                    mostRecentRevoke.Active = false;
                    db.Entry(mostRecentRevoke).State = EntityState.Modified;
                    db.SaveChanges();
                    individualAccessRevokes.Remove(mostRecentRevoke);
                }
                else
                {
                    mostRecentGrant.Active = false;
                    db.Entry(mostRecentGrant).State = EntityState.Modified;
                    db.SaveChanges();
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
                        List<Friendship> friends = db.Friendships.Where(i => i.FirstFriendId == userProfileId && i.Active).ToList();

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
                        List<PenProfile> writerProfiles = db.Writings.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault().PenUser.PenProfiles.ToList();

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
                        List<FolderOwner> folderOwners = db.Folders.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault().Owners.ToList();

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
                        List<SeriesOwner> seriesOwners = db.Series.Where(i => i.AccessPermissionId == accessPermissionId).FirstOrDefault().Owners.ToList();

                        foreach (var friendship in friendships)
                        {
                            if (seriesOwners.Any(i => i.OwnerId == friendship.SecondFriendId))
                            {
                                return true;
                            }
                        }
                    } //if it's a series
                } //if there's friend access

                if(accessPermission.PublisherAccess && accessPermission.FriendAccess)
                {
                    if(UserHasVerifiedPublisherProfile() && !UserHasVerifiedPublisherProfile())
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
                else if(accessPermission.PublisherAccess)
                {
                    if (UserHasVerifiedPublisherProfile() && !UserHasVerifiedPublisherProfile())
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
                else if(accessPermission.FriendAccess)
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

        protected PenProfile? GetMainProfile()
        {
            var penProfile = db.PenProfiles.Where(i => i.UserId == GetUserId() && i.IsMainProfile).FirstOrDefault();

            if (penProfile != null)
            {
                penProfile = PopulatePenProfile(penProfile);
            }

            return penProfile;
        }

        protected ICollection<PenProfile> GetPenProfiles()
        {
            List<PenProfile> penProfiles = db.PenProfiles.Where(i => i.UserId == GetUserId()).ToList();
            List<PenProfile> profiles = new List<PenProfile>();

            for (int i = 0; i < penProfiles.Count; i++)
            {
                var penProfile = penProfiles.ElementAt(i);
                penProfile = PopulatePenProfile(penProfile);
                profiles.Add(penProfile);
            }

            return penProfiles.Equals(profiles) ? penProfiles : profiles;
        }

        protected ICollection<PenProfile> GetPenProfiles(string userId)
        {
            List<PenProfile> penProfiles = db.PenProfiles.Where(i => i.UserId == userId).ToList();
            List<PenProfile> profiles = new List<PenProfile>();

            for (int i = 0; i < penProfiles.Count; i++)
            {
                var penProfile = penProfiles.ElementAt(i);
                penProfile = PopulatePenProfile(penProfile);
                profiles.Add(penProfile);
            }

            return penProfiles.Equals(profiles) ? penProfiles : profiles;
        }

        protected ICollection<PenProfile> GetWriterProfiles()
        {
            List<PenProfile> penProfiles = db.PenProfiles.Where(i => i.UserId == GetUserId() && i.RoleId == 1).ToList();
            List<PenProfile> profiles = new List<PenProfile>();

            for (int i = 0; i < penProfiles.Count; i++)
            {
                var penProfile = penProfiles.ElementAt(i);
                penProfile = PopulatePenProfile(penProfile);
                profiles.Add(penProfile);
            }

            return penProfiles.Equals(profiles) ? penProfiles : profiles;
        }

        protected ICollection<PenProfile> GetWriterProfiles(string userId)
        {
            List<PenProfile> penProfiles = db.PenProfiles.Where(i => i.UserId == userId && i.RoleId == 1).ToList();
            List<PenProfile> profiles = new List<PenProfile>();

            for (int i = 0; i < penProfiles.Count; i++)
            {
                var penProfile = penProfiles.ElementAt(i);
                penProfile = PopulatePenProfile(penProfile);
                profiles.Add(penProfile);
            }

            return penProfiles.Equals(profiles) ? penProfiles : profiles;
        }

        protected ICollection<PenProfile> GetPublisherProfiles()
        {
            List<PenProfile> penProfiles = db.PenProfiles.Where(i => i.UserId == GetUserId() && i.RoleId == 2).ToList();
            List<PenProfile> profiles = new List<PenProfile>();

            for (int i = 0; i < penProfiles.Count; i++)
            {
                var penProfile = penProfiles.ElementAt(i);
                penProfile = PopulatePenProfile(penProfile);
                profiles.Add(penProfile);
            }

            return penProfiles.Equals(profiles) ? penProfiles : profiles;
        }

        protected ICollection<PenProfile> GetPublisherProfiles(string userId)
        {
            List<PenProfile> penProfiles = db.PenProfiles.Where(i => i.UserId == userId && i.RoleId == 2).ToList();
            List<PenProfile> profiles = new List<PenProfile>();

            for (int i = 0; i < penProfiles.Count; i++)
            {
                var penProfile = penProfiles.ElementAt(i);
                penProfile = PopulatePenProfile(penProfile);
                profiles.Add(penProfile);
            }

            return penProfiles.Equals(profiles) ? penProfiles : profiles;
        }

        protected ICollection<PenProfile> GetVerifiedPublisherProfiles()
        {
            List<PenProfile> penProfiles = db.PenProfiles.Where(i => i.UserId == GetUserId() && i.RoleId == 2 && i.Verified).ToList();
            List<PenProfile> profiles = new List<PenProfile>();

            for (int i = 0; i < penProfiles.Count; i++)
            {
                var penProfile = penProfiles.ElementAt(i);
                penProfile = PopulatePenProfile(penProfile);
                profiles.Add(penProfile);
            }

            return penProfiles.Equals(profiles) ? penProfiles : profiles;
        }

        protected ICollection<PenProfile> GetVerifiedPublisherProfiles(string userId)
        {
            List<PenProfile> penProfiles = db.PenProfiles.Where(i => i.UserId == userId && i.RoleId == 2 && i.Verified).ToList();
            List<PenProfile> profiles = new List<PenProfile>();

            for (int i = 0; i < penProfiles.Count; i++)
            {
                var penProfile = penProfiles.ElementAt(i);
                penProfile = PopulatePenProfile(penProfile);
                profiles.Add(penProfile);
            }

            return penProfiles.Equals(profiles) ? penProfiles : profiles;
        }

        protected PenProfile PopulatePenProfile(PenProfile penProfile)
        {
            //if PenUser isn't populated, populate it
            if (penProfile.PenUser == null)
            {
                penProfile.PenUser = db.PenUsers.Where(i => i.Id == penProfile.UserId).FirstOrDefault();
            }

            //if AccessPermission isn't populated, populate it
            if (penProfile.AccessPermission == null)
            {
                penProfile.AccessPermission = db.AccessPermissions.Where(i => i.AccessPermissionId == penProfile.AccessPermissionId).FirstOrDefault();
            }

            //if PenRole isn't populated, populate it
            if (penProfile.PenRole == null)
            {
                penProfile.PenRole = db.ProfileRoles.Where(i => i.RoleId == penProfile.RoleId).FirstOrDefault();
            }

            return penProfile;
        }

        protected PenUser PopulatePenUser(PenUser user)
        {
            //if the PenProfiles haven't been populated, populate them
            if (user.PenProfiles.Count == 0)
            {
                user.PenProfiles = db.PenProfiles.Where(i => i.UserId == user.Id).ToList();

                for (int i = 0; i < user.PenProfiles.Count; i++)
                {
                    var penProfile = user.PenProfiles.ElementAt(i);
                    penProfile = PopulatePenProfile(penProfile);
                    user.PenProfiles.ElementAt(i).AccessPermission = penProfile.AccessPermission;
                    user.PenProfiles.ElementAt(i).PenUser = user;
                    user.PenProfiles.ElementAt(i).PenRole = penProfile.PenRole;
                }
            }

            return user;
        }

        private Task<PenUser?> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
