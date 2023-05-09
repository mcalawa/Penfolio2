﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Penfolio2.Data;
using Penfolio2.Models;

namespace Penfolio2.Controllers
{
    public class NotificationController : AccessController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly IWebHostEnvironment environment;

        public NotificationController(IWebHostEnvironment environment) : base()
        {
            this.environment = environment;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AcceptFriendRequest(int id)
        {
            var userId = GetUserId();

            if(userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = GetUserById(userId);

            if(user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if(user.PenProfiles.Count == 0)
            {
                user.PenProfiles = db.PenProfiles.Where(i => i.UserId == userId).ToList();
            }

            var friendRequest = db.FriendRequests.Where(i => i.FriendRequestId == id).FirstOrDefault();

            if(friendRequest == null)
            {
                return RedirectToAction("NotificationError");
            }

            //if the requestee is not one of the user's profiles
            if(!user.PenProfiles.Select(i => i.ProfileId).ToList().Contains(friendRequest.RequesteeId))
            {
                return RedirectToAction("NotificationError");
            }

            //if the requester doesn't exist 
            if(!db.PenProfiles.Any(i => i.ProfileId == friendRequest.RequesterId))
            {
                return RedirectToAction("NotificationError");
            }

            //if there's already an active friendship for these two in the database
            if(db.Friendships.Any(i => i.Active && ((i.FirstFriendId == friendRequest.RequesteeId && i.SecondFriendId == friendRequest.RequesterId) || (i.FirstFriendId == friendRequest.RequesterId && i.SecondFriendId == friendRequest.RequesterId))))
            {
                //if there's only a relationship one way
                if(!db.Friendships.Any(i => i.Active && i.FirstFriendId == friendRequest.RequesteeId && i.SecondFriendId == friendRequest.RequesterId))
                {
                    //get the firendship that's in the database
                    List<Friendship> friendships = db.Friendships.Where(i => i.Active && i.FirstFriendId == friendRequest.RequesterId && i.SecondFriendId == friendRequest.RequesteeId).OrderByDescending(i => i.AcceptDate).ToList();
                    Friendship friendship = friendships.First();

                    foreach(var f in friendships)
                    {
                        if(f != friendship)
                        {
                            f.Active = false;
                            db.Entry(f).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    //if there's a second friendship for the two, but it isn't active
                    if(db.Friendships.Any(i => i.FirstFriendId == friendRequest.RequesteeId && i.SecondFriendId == friendRequest.RequesterId && i.AcceptDate == friendship.AcceptDate))
                    {
                        Friendship secondFriendship = db.Friendships.Where(i => i.FirstFriendId == friendRequest.RequesteeId && i.SecondFriendId == friendRequest.RequesterId && i.AcceptDate == friendship.AcceptDate).First();

                        secondFriendship.Active = true;
                        db.Entry(secondFriendship).State = EntityState.Modified;
                        db.SaveChanges();
                    } //if the second friendship just doesn't exist 
                    else
                    {
                        var newFriendship = new Friendship
                        {
                            FirstFriendId = friendRequest.RequesteeId,
                            SecondFriendId = friendRequest.RequesterId,
                            Active = true,
                            AcceptDate = friendship.AcceptDate
                        };

                        db.Friendships.Add(newFriendship);
                        db.SaveChanges();
                    }
                } //if there's only a relationship one way, but it's the opposite way
                else if(!db.Friendships.Any(i => i.Active && i.FirstFriendId == friendRequest.RequesterId && i.SecondFriendId == friendRequest.RequesteeId))
                {
                    //get the friendship that's in the database
                    List<Friendship> friendships = db.Friendships.Where(i => i.Active && i.FirstFriendId == friendRequest.RequesteeId && i.SecondFriendId == friendRequest.RequesterId).ToList();
                    Friendship friendship = friendships.First();

                    foreach(var f in friendships)
                    {
                        if(f != friendship)
                        {
                            f.Active = false;
                            db.Entry(f).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    //if there's a second friendship for the two, but it isn't active
                    if(db.Friendships.Any(i => i.FirstFriendId == friendRequest.RequesterId && i.SecondFriendId == friendRequest.RequesteeId && i.AcceptDate == friendship.AcceptDate))
                    {
                        Friendship secondFriendship = db.Friendships.Where(i => i.FirstFriendId == friendRequest.RequesterId && i.SecondFriendId == friendRequest.RequesteeId && i.AcceptDate == friendship.AcceptDate).First();
                        secondFriendship.Active = true;
                        db.Entry(secondFriendship).State = EntityState.Modified;
                        db.SaveChanges();
                    } //if the second friendship just doesn't exist
                    else
                    {
                        var newFriendship = new Friendship
                        {
                            FirstFriendId = friendRequest.RequesterId,
                            SecondFriendId = friendRequest.RequesteeId,
                            Active = true,
                            AcceptDate = friendship.AcceptDate
                        };

                        db.Friendships.Add(newFriendship);
                        db.SaveChanges();
                    }
                }
            } //if there isn't an active friendship for these two in the database
            else
            {
                Friendship firstFriendship = new Friendship
                {
                    FirstFriendId = friendRequest.RequesterId,
                    SecondFriendId = friendRequest.RequesteeId,
                    Active = true,
                    AcceptDate = DateTime.Now
                };

                Friendship secondFriendship = new Friendship
                {
                    FirstFriendId = friendRequest.RequesteeId,
                    SecondFriendId = friendRequest.RequesterId,
                    Active = true,
                    AcceptDate = firstFriendship.AcceptDate
                };

                db.Friendships.Add(firstFriendship);
                db.SaveChanges();

                db.Friendships.Add(secondFriendship);
                db.SaveChanges();
            }

            user.LastNotificationViewDate = DateTime.Now;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            friendRequest.Resolved = true;
            db.Entry(friendRequest).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "Profile", new { id = friendRequest.RequesteeId });
        }

        public ActionResult DeclineFriendRequest(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = GetUserById(userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (user.PenProfiles.Count == 0)
            {
                user.PenProfiles = db.PenProfiles.Where(i => i.UserId == userId).ToList();
            }

            var friendRequest = db.FriendRequests.Where(i => i.FriendRequestId == id).FirstOrDefault();

            if (friendRequest == null)
            {
                return RedirectToAction("NotificationError");
            }

            //if the requestee is not one of the user's profiles
            if (!user.PenProfiles.Select(i => i.ProfileId).ToList().Contains(friendRequest.RequesteeId))
            {
                return RedirectToAction("NotificationError");
            }

            //if there's already a friendship for these two in the database
            if(db.Friendships.Any(i => i.Active && ((i.FirstFriendId == friendRequest.RequesteeId && i.SecondFriendId == friendRequest.RequesterId) || (i.FirstFriendId == friendRequest.RequesterId && i.SecondFriendId == friendRequest.RequesteeId))))
            {
                //if it's only on one side
                if(!db.Friendships.Any(i => i.Active && i.FirstFriendId == friendRequest.RequesteeId && i.SecondFriendId == friendRequest.RequesterId))
                {
                    //get the active friendship
                    List<Friendship> activeFriendships = db.Friendships.Where(i => i.Active && i.FirstFriendId == friendRequest.RequesterId && i.SecondFriendId == friendRequest.RequesteeId).ToList();

                    foreach(var activeFriendship in activeFriendships)
                    {
                        activeFriendship.Active = false;
                        db.Entry(activeFriendship).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                } //if it's only on one side, but it's on the other side
                else if(!db.Friendships.Any(i => i.Active && i.FirstFriendId == friendRequest.RequesterId && i.SecondFriendId == friendRequest.RequesteeId))
                {
                    //get the active friendship
                    List<Friendship> activeFriendships = db.Friendships.Where(i => i.Active && i.FirstFriendId == friendRequest.RequesteeId && i.SecondFriendId == friendRequest.RequesterId).ToList();

                    foreach (var activeFriendship in activeFriendships)
                    {
                        activeFriendship.Active = false;
                        db.Entry(activeFriendship).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                } //if there are entries for both
                else
                {
                    //get the first active friendship
                    List<Friendship> firstFriendships = db.Friendships.Where(i => i.Active && i.FirstFriendId == friendRequest.RequesterId && i.SecondFriendId == friendRequest.RequesteeId).ToList();

                    foreach(var firstFriendship in firstFriendships)
                    {
                        firstFriendship.Active = false;
                        db.Entry(firstFriendship).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    //get the second active friendship
                    List<Friendship> secondFriendships = db.Friendships.Where(i => i.Active && i.FirstFriendId == friendRequest.RequesteeId && i.SecondFriendId == friendRequest.RequesterId).ToList();

                    foreach(var  secondFriendship in secondFriendships)
                    {
                        secondFriendship.Active = false;
                        db.Entry(secondFriendship).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            user.LastNotificationViewDate = DateTime.Now;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            friendRequest.Resolved = true;
            db.Entry(friendRequest).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "Profile", new { id = friendRequest.RequesteeId });
        }

        public ActionResult GrantAccessRequest(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = GetUserById(userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (user.PenProfiles.Count == 0)
            {
                user.PenProfiles = db.PenProfiles.Where(i => i.UserId == userId).ToList();
            }

            var accessRequest = db.AccessRequests.Where(i => i.AccessRequestId == id).FirstOrDefault();

            if(accessRequest == null)
            {
                return RedirectToAction("NotificationError");
            }

            if(accessRequest.AccessPermission == null)
            {
                accessRequest.AccessPermission = db.AccessPermissions.Where(i => i.AccessPermissionId == id).FirstOrDefault();
            }

            if(accessRequest.AccessPermission == null)
            {
                return RedirectToAction("NotificationError");
            }

            if(!db.PenProfiles.Any(i => i.ProfileId == accessRequest.RequesterId))
            {
                return RedirectToAction("NotificationError");
            }

            //if this is for a profile
            if (accessRequest.AccessPermission.ProfileId != null)
            {
                var profile = db.PenProfiles.Where(i => i.ProfileId == accessRequest.AccessPermission.ProfileId.Value).FirstOrDefault();

                if (profile == null)
                {
                    return RedirectToAction("NotificationError");
                }

                if (profile.UserId != userId)
                {
                    return RedirectToAction("NotificationError");
                }

                //check and see if there are any active IndividualAccessRevokes
                if (db.IndividualAccessRevokes.Any(i => i.Active && i.AccessPermissionId == accessRequest.AccessPermissionId && i.RevokeeId == accessRequest.RequesterId))
                {
                    List<IndividualAccessRevoke> activeRevokes = db.IndividualAccessRevokes.Where(i => i.Active && i.AccessPermissionId == accessRequest.AccessPermissionId && i.RevokeeId == accessRequest.RequesterId).ToList();

                    foreach (var activeRevoke in activeRevokes)
                    {
                        activeRevoke.Active = false;
                        db.Entry(activeRevoke).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                //check and see if there are any active IndividualAccessGrants
                if (db.IndividualAccessGrants.Any(i => i.Active && i.AccessPermissionId == accessRequest.AccessPermissionId && i.GranteeId == accessRequest.RequesterId))
                {
                    List<IndividualAccessGrant> activeGrants = db.IndividualAccessGrants.Where(i => i.Active && i.AccessPermissionId == accessRequest.AccessPermissionId && i.GranteeId == accessRequest.RequesterId).OrderByDescending(i => i.GrantDate).ToList();
                    IndividualAccessGrant mostRecentActiveGrant = activeGrants.First();

                    foreach (var activeGrant in activeGrants)
                    {
                        if (activeGrant != mostRecentActiveGrant)
                        {
                            activeGrant.Active = false;
                            db.Entry(activeGrant).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                } //if there isn't an active IndividualAccessGrant, we have to create one
                else
                {
                    IndividualAccessGrant accessGrant = new IndividualAccessGrant
                    {
                        AccessPermissionId = accessRequest.AccessPermissionId,
                        Active = true,
                        GranteeId = accessRequest.RequesterId,
                        GrantDate = DateTime.Now
                    };

                    db.IndividualAccessGrants.Add(accessGrant);
                    db.SaveChanges();
                }

                user.LastNotificationViewDate = DateTime.Now;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                accessRequest.Resolved = true;
                db.Entry(accessRequest).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", "Profile", new { id = accessRequest.AccessPermission.ProfileId });
            } //if this is for a piece of writing
            else if(accessRequest.AccessPermission.WritingId != null)
            {
                var writing = db.Writings.Where(i => i.WritingId == accessRequest.AccessPermission.WritingId.Value).FirstOrDefault();

                if(writing == null)
                {
                    return RedirectToAction("NotificationError");
                }

                List<IdentityError> errors = new List<IdentityError>();
                bool canGrantAccess = IsAccessableByUser(accessRequest.AccessPermissionId, ref errors, "edit");

                if(!canGrantAccess)
                {
                    return RedirectToAction("NotificationError");
                }

                //check and see if there are any active IndividualAccessRevokes
                if (db.IndividualAccessRevokes.Any(i => i.Active && i.AccessPermissionId == accessRequest.AccessPermissionId && i.RevokeeId == accessRequest.RequesterId))
                {
                    List<IndividualAccessRevoke> activeRevokes = db.IndividualAccessRevokes.Where(i => i.Active && i.AccessPermissionId == accessRequest.AccessPermissionId && i.RevokeeId == accessRequest.RequesterId).ToList();

                    foreach (var activeRevoke in activeRevokes)
                    {
                        activeRevoke.Active = false;
                        db.Entry(activeRevoke).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                //check and see if there are any active IndividualAccessGrants
                if (db.IndividualAccessGrants.Any(i => i.Active && i.AccessPermissionId == accessRequest.AccessPermissionId && i.GranteeId == accessRequest.RequesterId))
                {
                    List<IndividualAccessGrant> activeGrants = db.IndividualAccessGrants.Where(i => i.Active && i.AccessPermissionId == accessRequest.AccessPermissionId && i.GranteeId == accessRequest.RequesterId).OrderByDescending(i => i.GrantDate).ToList();
                    IndividualAccessGrant mostRecentActiveGrant = activeGrants.First();

                    foreach (var activeGrant in activeGrants)
                    {
                        if (activeGrant != mostRecentActiveGrant)
                        {
                            activeGrant.Active = false;
                            db.Entry(activeGrant).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                } //if there isn't an active IndividualAccessGrant, we have to create one
                else
                {
                    IndividualAccessGrant accessGrant = new IndividualAccessGrant
                    {
                        AccessPermissionId = accessRequest.AccessPermissionId,
                        Active = true,
                        GranteeId = accessRequest.RequesterId,
                        GrantDate = DateTime.Now
                    };

                    db.IndividualAccessGrants.Add(accessGrant);
                    db.SaveChanges();
                }

                user.LastNotificationViewDate = DateTime.Now;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                accessRequest.Resolved = true;
                db.Entry(accessRequest).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("ViewWriting", "Writing", new { id = accessRequest.AccessPermission.WritingId });
            }
            //TBD: folder and series are not implemented yet, but should they be, they will go here.

            //if you get this far, something has gone wrong
            return RedirectToAction("NotificationError");
        }

        public ActionResult DeclineAccessRequest(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = GetUserById(userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (user.PenProfiles.Count == 0)
            {
                user.PenProfiles = db.PenProfiles.Where(i => i.UserId == userId).ToList();
            }

            var accessRequest = db.AccessRequests.Where(i => i.AccessRequestId == id).FirstOrDefault();

            if (accessRequest == null)
            {
                return RedirectToAction("NotificationError");
            }

            if (accessRequest.AccessPermission == null)
            {
                accessRequest.AccessPermission = db.AccessPermissions.Where(i => i.AccessPermissionId == id).FirstOrDefault();
            }

            if (accessRequest.AccessPermission == null)
            {
                return RedirectToAction("NotificationError");
            }

            if (!db.PenProfiles.Any(i => i.ProfileId == accessRequest.RequesterId))
            {
                return RedirectToAction("NotificationError");
            }

            //if this is for a profile
            if (accessRequest.AccessPermission.ProfileId != null)
            {
                var profile = db.PenProfiles.Where(i => i.ProfileId == accessRequest.AccessPermission.ProfileId.Value).FirstOrDefault();

                if (profile == null)
                {
                    return RedirectToAction("NotificationError");
                }

                if (profile.UserId != userId)
                {
                    return RedirectToAction("NotificationError");
                }

                //check and see if there are any active IndividualAccessRevokes
                if (db.IndividualAccessRevokes.Any(i => i.Active && i.AccessPermissionId == accessRequest.AccessPermissionId && i.RevokeeId == accessRequest.RequesterId))
                {
                    List<IndividualAccessRevoke> activeRevokes = db.IndividualAccessRevokes.Where(i => i.Active && i.AccessPermissionId == accessRequest.AccessPermissionId && i.RevokeeId == accessRequest.RequesterId).OrderByDescending(i => i.RevokeDate).ToList();
                    IndividualAccessRevoke firstActiveRevoke = activeRevokes.First();

                    foreach (var activeRevoke in activeRevokes)
                    {
                        if(activeRevoke != firstActiveRevoke)
                        {
                            activeRevoke.Active = false;
                            db.Entry(activeRevoke).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }

                //check and see if there are any active IndividualAccessGrants
                if (db.IndividualAccessGrants.Any(i => i.Active && i.AccessPermissionId == accessRequest.AccessPermissionId && i.GranteeId == accessRequest.RequesterId))
                {
                    List<IndividualAccessGrant> activeGrants = db.IndividualAccessGrants.Where(i => i.Active && i.AccessPermissionId == accessRequest.AccessPermissionId && i.GranteeId == accessRequest.RequesterId).ToList();

                    foreach (var activeGrant in activeGrants)
                    {
                        activeGrant.Active = false;
                        db.Entry(activeGrant).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                } 

                user.LastNotificationViewDate = DateTime.Now;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                accessRequest.Resolved = true;
                db.Entry(accessRequest).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", "Profile", new { id = accessRequest.AccessPermission.ProfileId });
            } //if this is for a piece of writing
            else if (accessRequest.AccessPermission.WritingId != null)
            {
                var writing = db.Writings.Where(i => i.WritingId == accessRequest.AccessPermission.WritingId.Value).FirstOrDefault();

                if (writing == null)
                {
                    return RedirectToAction("NotificationError");
                }

                List<IdentityError> errors = new List<IdentityError>();
                bool canGrantAccess = IsAccessableByUser(accessRequest.AccessPermissionId, ref errors, "edit");

                if (!canGrantAccess)
                {
                    return RedirectToAction("NotificationError");
                }

                //check and see if there are any active IndividualAccessRevokes
                if (db.IndividualAccessRevokes.Any(i => i.Active && i.AccessPermissionId == accessRequest.AccessPermissionId && i.RevokeeId == accessRequest.RequesterId))
                {
                    List<IndividualAccessRevoke> activeRevokes = db.IndividualAccessRevokes.Where(i => i.Active && i.AccessPermissionId == accessRequest.AccessPermissionId && i.RevokeeId == accessRequest.RequesterId).OrderByDescending(i => i.RevokeDate).ToList();
                    IndividualAccessRevoke firstActiveRevoke = activeRevokes.First();

                    foreach (var activeRevoke in activeRevokes)
                    {
                        if (activeRevoke != firstActiveRevoke)
                        {
                            activeRevoke.Active = false;
                            db.Entry(activeRevoke).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }

                //check and see if there are any active IndividualAccessGrants
                if (db.IndividualAccessGrants.Any(i => i.Active && i.AccessPermissionId == accessRequest.AccessPermissionId && i.GranteeId == accessRequest.RequesterId))
                {
                    List<IndividualAccessGrant> activeGrants = db.IndividualAccessGrants.Where(i => i.Active && i.AccessPermissionId == accessRequest.AccessPermissionId && i.GranteeId == accessRequest.RequesterId).ToList();

                    foreach (var activeGrant in activeGrants)
                    {
                        activeGrant.Active = false;
                        db.Entry(activeGrant).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                user.LastNotificationViewDate = DateTime.Now;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                accessRequest.Resolved = true;
                db.Entry(accessRequest).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("ViewWriting", "Writing", new { id = accessRequest.AccessPermission.WritingId });
            }
            //TBD: folder and series are not implemented yet, but should they be, they will go here.

            //if you get this far, something has gone wrong
            return RedirectToAction("NotificationError");
        }

        public ActionResult RemoveFriend(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = GetUserById(userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (user.PenProfiles.Count == 0)
            {
                user.PenProfiles = db.PenProfiles.Where(i => i.UserId == userId).ToList();
            }

            var friendship = db.Friendships.Where(i => i.FriendshipId == id).FirstOrDefault();

            if (friendship == null)
            {
                return RedirectToAction("NotificationError");
            }

            if(!user.PenProfiles.Select(i => i.ProfileId).ToList().Contains(friendship.FirstFriendId))
            {
                return RedirectToAction("NotificationError");
            }

            List<Friendship> firstFriendships = db.Friendships.Where(i => i.Active && i.FirstFriendId == friendship.FirstFriendId && i.SecondFriendId == friendship.SecondFriendId).ToList();

            foreach(var firstFriendship in  firstFriendships)
            {
                firstFriendship.Active = false;
                db.Entry(firstFriendship).State = EntityState.Modified;
                db.SaveChanges();
            }

            List<Friendship> secondFriendships = db.Friendships.Where(i => i.Active && i.FirstFriendId == friendship.SecondFriendId && i.SecondFriendId == friendship.FirstFriendId).ToList();

            foreach(var secondFriendship in secondFriendships)
            {
                secondFriendship.Active = false;
                db.Entry(secondFriendship).State = EntityState.Modified;
                db.SaveChanges();
            }

            if(friendship.FirstFriend == null)
            {
                friendship.FirstFriend = db.PenProfiles.Where(i => i.ProfileId == friendship.FirstFriendId).First();
            }

            return RedirectToAction("Index", "Profile", new { id = friendship.FirstFriend.UrlString });
        }

        public ActionResult RemoveIndividualAccessGrant(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = GetUserById(userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (user.PenProfiles.Count == 0)
            {
                user.PenProfiles = db.PenProfiles.Where(i => i.UserId == userId).ToList();
            }

            var grant = db.IndividualAccessGrants.Where(i => i.IndividualAccessGrantId == id).FirstOrDefault();

            if (grant == null)
            {
                return RedirectToAction("NotificationError");
            }

            if(grant.AccessPermission == null)
            {
                grant.AccessPermission = db.AccessPermissions.Where(i => i.AccessPermissionId == grant.AccessPermissionId).FirstOrDefault();
            }

            if(grant.AccessPermission == null)
            {
                return RedirectToAction("NotificationError");
            }

            if(grant.AccessPermission.ProfileId != null && !user.PenProfiles.Select(i => i.ProfileId).ToList().Contains(grant.AccessPermission.ProfileId.Value))
            {
                return RedirectToAction("NotificationError");
            }
            else if(grant.AccessPermission.WritingId != null)
            {
                List<WritingProfile> writingProfiles = db.WritingProfiles.Where(i => i.WritingId == grant.AccessPermission.WritingId.Value).ToList();

                foreach(var writingProfile in writingProfiles)
                {
                    if(writingProfile.PenProfile == null)
                    {
                        writingProfile.PenProfile = db.PenProfiles.Where(i => i.ProfileId == writingProfile.ProfileId).FirstOrDefault();
                    }
                }

                if(!writingProfiles.Where(i => i.PenProfile != null).Select(i => i.PenProfile.UserId).ToList().Contains(userId))
                {
                    return RedirectToAction("NotificationError");
                }
            }

            List<IndividualAccessGrant> activeGrants = db.IndividualAccessGrants.Where(i => i.Active && i.AccessPermissionId == grant.AccessPermissionId && i.GranteeId ==  grant.GranteeId).ToList();

            foreach(var activeGrant in activeGrants)
            {
                activeGrant.Active = false;
                db.Entry(activeGrant).State = EntityState.Modified;
                db.SaveChanges();
            }

            if(grant.AccessPermission.WritingId != null)
            {
                return RedirectToAction("ViewWriting", "Writing", new { id =  grant.AccessPermission.WritingId });
            }
            else if(grant.AccessPermission.ProfileId != null)
            {
                return RedirectToAction("Index", "Profile", new { id = grant.AccessPermission.ProfileId  });
            }
            else
            {
                return RedirectToAction("NotificationError");
            }
        }

        public ActionResult RemoveIndividualAccessRevoke(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = GetUserById(userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (user.PenProfiles.Count == 0)
            {
                user.PenProfiles = db.PenProfiles.Where(i => i.UserId == userId).ToList();
            }

            var revoke = db.IndividualAccessRevokes.Where(i => i.IndividualAccessRevokeId == id).FirstOrDefault();

            var friendship = db.Friendships.Where(i => i.FriendshipId == id).FirstOrDefault();

            if (revoke == null)
            {
                return RedirectToAction("NotificationError");
            }

            if (revoke.AccessPermission == null)
            {
                revoke.AccessPermission = db.AccessPermissions.Where(i => i.AccessPermissionId == revoke.AccessPermissionId).FirstOrDefault();
            }

            if (revoke.AccessPermission == null)
            {
                return RedirectToAction("NotificationError");
            }

            if (revoke.AccessPermission.ProfileId != null && !user.PenProfiles.Select(i => i.ProfileId).ToList().Contains(revoke.AccessPermission.ProfileId.Value))
            {
                return RedirectToAction("NotificationError");
            }
            else if (revoke.AccessPermission.WritingId != null)
            {
                List<WritingProfile> writingProfiles = db.WritingProfiles.Where(i => i.WritingId == revoke.AccessPermission.WritingId.Value).ToList();

                foreach (var writingProfile in writingProfiles)
                {
                    if (writingProfile.PenProfile == null)
                    {
                        writingProfile.PenProfile = db.PenProfiles.Where(i => i.ProfileId == writingProfile.ProfileId).FirstOrDefault();
                    }
                }

                if (!writingProfiles.Where(i => i.PenProfile != null).Select(i => i.PenProfile.UserId).ToList().Contains(userId))
                {
                    return RedirectToAction("NotificationError");
                }
            }

            List<IndividualAccessRevoke> activeRevokes = db.IndividualAccessRevokes.Where(i => i.Active && i.AccessPermissionId == revoke.AccessPermissionId && i.RevokeeId == revoke.RevokeeId).ToList();

            foreach (var activeRevoke in activeRevokes)
            {
                activeRevoke.Active = false;
                db.Entry(activeRevoke).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (revoke.AccessPermission.WritingId != null)
            {
                return RedirectToAction("ViewWriting", "Writing", new { id = revoke.AccessPermission.WritingId });
            }
            else if (revoke.AccessPermission.ProfileId != null)
            {
                return RedirectToAction("Index", "Profile", new { id = revoke.AccessPermission.ProfileId });
            }
            else
            {
                return RedirectToAction("NotificationError");
            }
        }

        public ActionResult NotificationError()
        {
            return View();
        }
    }
}
