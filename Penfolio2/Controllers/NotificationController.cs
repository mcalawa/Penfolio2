using Microsoft.AspNetCore.Identity;
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
                        secondFriendship.OtherFriendshipId = friendship.FriendshipId;
                        db.Entry(secondFriendship).State = EntityState.Modified;
                        db.SaveChanges();

                        friendship.OtherFriendshipId = secondFriendship.FriendshipId;
                        db.Entry(friendship).State = EntityState.Modified;
                        db.SaveChanges();
                    } //if the second friendship just doesn't exist 
                    else
                    {
                        var newFriendship = new Friendship
                        {
                            FirstFriendId = friendRequest.RequesteeId,
                            SecondFriendId = friendRequest.RequesterId,
                            Active = true,
                            AcceptDate = friendship.AcceptDate,
                            OtherFriendshipId = friendship.FriendshipId
                        };

                        db.Friendships.Add(newFriendship);
                        db.SaveChanges();

                        friendship.OtherFriendshipId = newFriendship.FriendshipId;
                        db.Entry(friendship).State = EntityState.Modified;
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
                        secondFriendship.OtherFriendshipId = friendship.OtherFriendshipId;
                        db.Entry(secondFriendship).State = EntityState.Modified;
                        db.SaveChanges();

                        friendship.OtherFriendshipId = secondFriendship.FriendshipId;
                        db.Entry(friendship).State = EntityState.Modified;
                        db.SaveChanges();
                    } //if the second friendship just doesn't exist
                    else
                    {
                        var newFriendship = new Friendship
                        {
                            FirstFriendId = friendRequest.RequesterId,
                            SecondFriendId = friendRequest.RequesteeId,
                            Active = true,
                            AcceptDate = friendship.AcceptDate,
                            OtherFriendshipId = friendship.FriendshipId
                        };

                        db.Friendships.Add(newFriendship);
                        db.SaveChanges();

                        friendship.OtherFriendshipId = newFriendship.FriendshipId;
                        db.Entry(friendship).State = EntityState.Modified;
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

                firstFriendship.OtherFriendshipId = secondFriendship.FriendshipId;
                db.Entry(firstFriendship).State = EntityState.Modified;
                db.SaveChanges();

                secondFriendship.OtherFriendshipId = firstFriendship.FriendshipId;
                db.Entry(secondFriendship).State = EntityState.Modified;
            }

            user.LastNotificationViewDate = DateTime.Now;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            friendRequest.Resolved = true;
            db.Entry(friendRequest).State = EntityState.Modified;
            db.SaveChanges();

            var profile = db.PenProfiles.Where(i => i.ProfileId == friendRequest.RequesterId).First();

            return RedirectToAction("Index", "Profile", new { id = profile.UrlString });
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

            return RedirectToAction("Index", "Profile");
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
                accessRequest.AccessPermission = db.AccessPermissions.Where(i => i.AccessPermissionId == accessRequest.AccessPermissionId).FirstOrDefault();
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

                return RedirectToAction("Index", "Profile", new { id = profile.UrlString });
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
                accessRequest.AccessPermission = db.AccessPermissions.Where(i => i.AccessPermissionId == accessRequest.AccessPermissionId).FirstOrDefault();
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

                return RedirectToAction("Index", "Profile", new { id = profile.UrlString });
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

        public ActionResult AcceptRepresentationRequest(int id)
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

            var representationRequest = db.RepresentationRequests.Where(i => i.RepresentationRequestId == id).FirstOrDefault();

            if (representationRequest == null)
            {
                return RedirectToAction("NotificationError");
            }

            if(representationRequest.Requester == null)
            {
                representationRequest.Requester = db.PenProfiles.Where(i => i.ProfileId == representationRequest.RequesterId).FirstOrDefault();
            }

            if (representationRequest.Requester == null)
            {
                return RedirectToAction("NotificationError");
            }

            if (representationRequest.Requestee == null)
            {
                representationRequest.Requestee = db.PenProfiles.Where(i => i.ProfileId == representationRequest.RequesteeId).FirstOrDefault();
            }

            if (representationRequest.Requestee == null)
            {
                return RedirectToAction("NotificationError");
            }

            if(representationRequest.Requestee.RoleId == representationRequest.Requester.RoleId)
            {
                return RedirectToAction("NotificationError");
            }

            if(representationRequest.Requestee.UserId != userId)
            {
                return RedirectToAction("NotificationError");
            }

            //if the one accepting the request is a writer profile
            if(representationRequest.Requestee.RoleId == 1)
            {
                if(!representationRequest.Requester.Verified)
                {
                    return RedirectToAction("NotificationError");
                }

                //check and see if there's already an active relationship
                if(db.PublisherWriters.Any(i => i.Active && i.WriterId == representationRequest.RequesteeId && i.PublisherId == representationRequest.RequesterId))
                {
                    List<PublisherWriter> activePublisherWriters = db.PublisherWriters.Where(i => i.Active && i.WriterId == representationRequest.RequesteeId && i.PublisherId == representationRequest.RequesterId).ToList();
                    PublisherWriter mostRecentActivePublisherWriter = activePublisherWriters.First();

                    foreach(var publisherWriter in activePublisherWriters)
                    {
                        if(publisherWriter != mostRecentActivePublisherWriter)
                        {
                            publisherWriter.Active = false;
                            db.Entry(publisherWriter).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                } //if there isn't an active relationship, we have to create one
                else
                {
                    PublisherWriter publisherWriter = new PublisherWriter
                    {
                        PublisherId = representationRequest.RequesterId,
                        WriterId = representationRequest.RequesteeId,
                        Active = true,
                        AcceptDate = DateTime.Now
                    };

                    db.PublisherWriters.Add(publisherWriter);
                    db.SaveChanges();
                }
            } //if the one accepting the request is a publisher profile
            else if(representationRequest.Requestee.RoleId == 2)
            {
                if (!representationRequest.Requestee.Verified)
                {
                    return RedirectToAction("NotificationError");
                }

                //check and see if there's already an active relationship
                if (db.PublisherWriters.Any(i => i.Active && i.WriterId == representationRequest.RequesterId && i.PublisherId == representationRequest.RequesteeId))
                {
                    List<PublisherWriter> activePublisherWriters = db.PublisherWriters.Where(i => i.Active && i.WriterId == representationRequest.RequesterId && i.PublisherId == representationRequest.RequesteeId).ToList();
                    PublisherWriter mostRecentActivePublisherWriter = activePublisherWriters.First();

                    foreach (var publisherWriter in activePublisherWriters)
                    {
                        if (publisherWriter != mostRecentActivePublisherWriter)
                        {
                            publisherWriter.Active = false;
                            db.Entry(publisherWriter).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                } //if there isn't an active relationship, we have to create one
                else
                {
                    PublisherWriter publisherWriter = new PublisherWriter
                    {
                        PublisherId = representationRequest.RequesteeId,
                        WriterId = representationRequest.RequesterId,
                        Active = true,
                        AcceptDate = DateTime.Now
                    };

                    db.PublisherWriters.Add(publisherWriter);
                    db.SaveChanges();
                }
            }

            user.LastNotificationViewDate = DateTime.Now;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            representationRequest.Resolved = true;
            db.Entry(representationRequest).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "Profile", new { id = representationRequest.Requestee.UrlString });
        }

        public ActionResult DeclineRepresentationRequest(int id)
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

            var representationRequest = db.RepresentationRequests.Where(i => i.RepresentationRequestId == id).FirstOrDefault();
            var accessRequest = db.AccessRequests.Where(i => i.AccessRequestId == id).FirstOrDefault();

            if (representationRequest == null)
            {
                return RedirectToAction("NotificationError");
            }

            if (representationRequest.Requester == null)
            {
                representationRequest.Requester = db.PenProfiles.Where(i => i.ProfileId == representationRequest.RequesterId).FirstOrDefault();
            }

            if (representationRequest.Requester == null)
            {
                return RedirectToAction("NotificationError");
            }

            if (representationRequest.Requestee == null)
            {
                representationRequest.Requestee = db.PenProfiles.Where(i => i.ProfileId == representationRequest.RequesteeId).FirstOrDefault();
            }

            if (representationRequest.Requestee == null)
            {
                return RedirectToAction("NotificationError");
            }

            if (representationRequest.Requestee.RoleId == representationRequest.Requester.RoleId)
            {
                return RedirectToAction("NotificationError");
            }

            if(representationRequest.Requestee.UserId != userId)
            {
                return RedirectToAction("NotificationError");
            }

            //if the one declining the request is the writer
            if(representationRequest.Requestee.RoleId == 1)
            {
                //check and see if there are any active PublisherWriters
                if(db.PublisherWriters.Any(i => i.Active && i.WriterId == representationRequest.RequesteeId && i.PublisherId == representationRequest.RequesterId))
                {
                    List<PublisherWriter> activePublisherWriters = db.PublisherWriters.Where(i => i.Active && i.WriterId == representationRequest.RequesteeId && i.PublisherId == representationRequest.RequesterId).ToList();

                    foreach(var activePublisherWriter in activePublisherWriters)
                    {
                        activePublisherWriter.Active = false;
                        db.Entry(activePublisherWriter).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            } //if the one decling the request is the publisher
            else if(representationRequest.Requestee.RoleId == 2)
            {
                //check and see if there are any active PublisherWriters
                if (db.PublisherWriters.Any(i => i.Active && i.WriterId == representationRequest.RequesterId && i.PublisherId == representationRequest.RequesteeId))
                {
                    List<PublisherWriter> activePublisherWriters = db.PublisherWriters.Where(i => i.Active && i.WriterId == representationRequest.RequesterId && i.PublisherId == representationRequest.RequesteeId).ToList();

                    foreach (var activePublisherWriter in activePublisherWriters)
                    {
                        activePublisherWriter.Active = false;
                        db.Entry(activePublisherWriter).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            user.LastNotificationViewDate = DateTime.Now;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            representationRequest.Resolved = true;
            db.Entry(representationRequest).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "Profile", new { id = representationRequest.Requestee.UrlString });
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
                if(db.PenProfiles.Any(i => i.ProfileId ==  grant.AccessPermission.ProfileId.Value))
                {
                    var profile = db.PenProfiles.Where(i => i.ProfileId == grant.AccessPermission.ProfileId.Value).First();

                    return RedirectToAction("Index", "Profile", new { id = profile.UrlString });
                }

                return RedirectToAction("Index", "Profile");
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
                if (db.PenProfiles.Any(i => i.ProfileId == revoke.AccessPermission.ProfileId.Value))
                {
                    var profile = db.PenProfiles.Where(i => i.ProfileId == revoke.AccessPermission.ProfileId.Value).First();

                    return RedirectToAction("Index", "Profile", new { id = profile.UrlString });
                }

                return RedirectToAction("Index", "Profile");
            }
            else
            {
                return RedirectToAction("NotificationError");
            }
        }

        public ActionResult RemoveRepresentation(int id)
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

            var publisherWriter = db.PublisherWriters.Where(i => i.PublisherWriterId == id).FirstOrDefault();

            if (publisherWriter == null)
            {
                return RedirectToAction("NotificationError");
            }

            if (!db.PenProfiles.Any(i => i.ProfileId == publisherWriter.WriterId) || !db.PenProfiles.Any(i => i.ProfileId == publisherWriter.PublisherId))
            {
                return RedirectToAction("NotificationError");
            }

            PenProfile publisher = db.PenProfiles.Where(i => i.ProfileId == publisherWriter.PublisherId).First();
            PenProfile writer = db.PenProfiles.Where(i => i.ProfileId == publisherWriter.WriterId).First();

            if(user.PenProfiles.Select(i => i.ProfileId).ToList().Contains(publisher.ProfileId))
            {
                List<PublisherWriter> activePublisherWriters = db.PublisherWriters.Where(i => i.Active && i.WriterId == writer.ProfileId && i.PublisherId == publisher.ProfileId).ToList();

                foreach(var activePublisherWriter in activePublisherWriters)
                {
                    activePublisherWriter.Active = false;
                    db.Entry(activePublisherWriter).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "Profile", new { id = publisher.UrlString });
            }
            else if(user.PenProfiles.Select(i => i.ProfileId).ToList().Contains(writer.ProfileId))
            {
                List<PublisherWriter> activePublisherWriters = db.PublisherWriters.Where(i => i.Active && i.WriterId == writer.ProfileId && i.PublisherId == publisher.ProfileId).ToList();

                foreach (var activePublisherWriter in activePublisherWriters)
                {
                    activePublisherWriter.Active = false;
                    db.Entry(activePublisherWriter).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "Profile", new { id = writer.UrlString });
            }
            else
            {
                return RedirectToAction("NotificationError");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendFriendRequest(RequestFriendViewModel model)
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

            if(user.PenProfiles.Count == 0)
            {
                user.PenProfiles = db.PenProfiles.Where(i => i.UserId == userId).ToList();
            }

            if (ModelState.IsValid && model.SenderProfileId != null)
            {
                if(!user.PenProfiles.Select(i => i.ProfileId).ToList().Contains(model.SenderProfileId.Value))
                {
                    return RedirectToAction("Index", "Home");
                }

                if(db.PenProfiles.Any(i => i.ProfileId == model.SenderProfileId) && db.PenProfiles.Any(i => i.ProfileId == model.ReceiverProfileId) && db.AccessPermissions.Any(i => i.AccessPermissionId == model.AccessPermissionId))
                {
                    AccessPermission accessPermission = db.AccessPermissions.Where(i => i.AccessPermissionId == model.AccessPermissionId).First();

                    if(!db.Friendships.Any(i => i.Active && i.FirstFriendId == model.SenderProfileId.Value && i.SecondFriendId == model.ReceiverProfileId) && !db.Friendships.Any(i => i.Active && i.FirstFriendId == model.ReceiverProfileId && i.SecondFriendId == model.SenderProfileId.Value))
                    {
                        if(!db.FriendRequests.Any(i => i.Resolved != true && i.RequesterId == model.ReceiverProfileId && i.RequesteeId == model.SenderProfileId.Value) && !db.FriendRequests.Any(i => i.Resolved != true && i.RequesterId == model.SenderProfileId.Value && i.RequesteeId == model.ReceiverProfileId))
                        {
                            FriendRequest friendRequest = new FriendRequest
                            {
                                RequesterId = model.SenderProfileId.Value,
                                RequesteeId = model.ReceiverProfileId,
                                RequestDate = DateTime.Now,
                                Resolved = false
                            };

                            db.FriendRequests.Add(friendRequest);
                            db.SaveChanges();
                        } //if there's not already an unresolved friend request
                    } //if there's not already a friendship here

                    if(accessPermission.ProfileId != null)
                    {
                        var profile = db.PenProfiles.Where(i => i.ProfileId == accessPermission.ProfileId.Value).FirstOrDefault();

                        if(profile != null)
                        {
                            return RedirectToAction("Index", "Profile", new { id = profile.UrlString });
                        }
                    }
                    else if(accessPermission.WritingId != null)
                    {
                        return RedirectToAction("ViewWriting", "Writing", new { id = accessPermission.WritingId.Value });
                    }
                } //if the profiles and the accessPermission exist
            } //if ModelState.IsValid

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendAccessRequest(RequestAccessViewModel model)
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

            if(ModelState.IsValid && model.ProfileId != null)
            {
                if(db.PenProfiles.Any(i => i.ProfileId == model.ProfileId.Value) && db.AccessPermissions.Any(i => i.AccessPermissionId == model.AccessPermissionId))
                {
                    if(!user.PenProfiles.Select(i => i.ProfileId).ToList().Contains(model.ProfileId.Value))
                    {
                        return RedirectToAction("NotificationError");
                    }

                    AccessPermission accessPermission = db.AccessPermissions.Where(i => i.AccessPermissionId == model.AccessPermissionId).First();

                    if(!db.IndividualAccessGrants.Any(i => i.AccessPermissionId == model.AccessPermissionId && i.Active && i.GranteeId == model.ProfileId.Value) &&  !db.IndividualAccessRevokes.Any(i => i.Active && i.AccessPermissionId == model.AccessPermissionId && i.RevokeeId == model.ProfileId.Value) && !db.AccessRequests.Any(i => i.Resolved == false && i.AccessPermissionId == model.AccessPermissionId && i.RequesterId == model.ProfileId.Value))
                    {
                        AccessRequest accessRequest = new AccessRequest
                        {
                            AccessPermissionId = model.AccessPermissionId,
                            RequesterId = model.ProfileId.Value,
                            RequestDate = DateTime.Now,
                            Resolved = false
                        };

                        db.AccessRequests.Add(accessRequest);
                        db.SaveChanges();
                    }

                    if (accessPermission.ProfileId != null)
                    {
                        var profile = db.PenProfiles.Where(i => i.ProfileId == accessPermission.ProfileId.Value).FirstOrDefault();

                        if (profile != null)
                        {
                            return RedirectToAction("Index", "Profile", new { id = profile.UrlString });
                        }

                        return RedirectToAction("Index", "Profile");
                    }
                    else if (accessPermission.WritingId != null)
                    {
                        return RedirectToAction("ViewWriting", "Writing", new { id = accessPermission.WritingId });
                    }
                }
            }

            return RedirectToAction("NotificationError");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendRepresentationRequest(RequestRepresentationViewModel model)
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

            if (ModelState.IsValid && model.SenderProfileId != null)
            {
                if(db.PenProfiles.Any(i => i.ProfileId == model.SenderProfileId.Value) && db.PenProfiles.Any(i => i.ProfileId == model.ReceiverProfileId))
                {
                    PenProfile sender = db.PenProfiles.Where(i => i.ProfileId == model.SenderProfileId.Value).First();
                    PenProfile receiver = db.PenProfiles.Where(i => i.ProfileId == model.ReceiverProfileId).First();

                    if(sender.UserId != userId)
                    {
                        return RedirectToAction("NotificationError");
                    }

                    if(sender.RoleId == 2 && !sender.Verified)
                    {
                        return RedirectToAction("NotificationError");
                    }

                    if(receiver.RoleId == 2 && !receiver.Verified)
                    {
                        return RedirectToAction("NotificationError");
                    }

                    //if there isn't already an unresolved representation request, create one
                    if(!db.RepresentationRequests.Any(i => i.Resolved == false && i.RequesterId == sender.ProfileId && i.RequesteeId == receiver.ProfileId) && !db.RepresentationRequests.Any(i => i.Resolved == false && i.RequesterId == receiver.ProfileId && i.RequesteeId == sender.ProfileId))
                    {
                        RepresentationRequest representationRequest = new RepresentationRequest
                        {
                            RequesterId = sender.ProfileId,
                            RequesteeId = receiver.ProfileId,
                            Resolved = false,
                            RequestDate = DateTime.Now
                        };

                        db.RepresentationRequests.Add(representationRequest);
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index", "Profile", new { id = receiver.UrlString });
                }
            }

            return RedirectToAction("NotificationError");
        }

        public ActionResult NotificationError()
        {
            return View();
        }
    }
}
