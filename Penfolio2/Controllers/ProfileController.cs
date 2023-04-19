﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Penfolio2.Data;
using Penfolio2.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Penfolio2.Controllers
{
    [Authorize]
    public class ProfileController : AccessController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly IWebHostEnvironment environment;

        //lists for creating random URL strings if the user doesn't specify a custom URL
        private static List<int> numbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
        private static List<char> chars = new List<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '-', '_' };

        public ProfileController(IWebHostEnvironment environment) : base()
        {
            this.environment = environment;
        }

        // GET: ProfileController
        public ActionResult Index()
        {
            if (UserHasProfile())
            {
                PenProfile mainProfile = GetMainProfile();

                if (mainProfile == null)
                {
                    List<PenProfile> penProfiles = GetPenProfiles().ToList();
                    penProfiles.OrderBy(i => i.ProfileId).ToList();

                    mainProfile = penProfiles.FirstOrDefault();
                    mainProfile.IsMainProfile = true;
                    db.Entry(mainProfile).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
                ViewBag.OwnProfile = "true";

                return View(mainProfile);
            }

            return RedirectToAction("Create");
        }

        // GET: ProfileController/UrlString
        //[HttpGet]
        [Route("Profile/{id}")]
        [Route("Profile/Index/{id}")]
        public ActionResult Index(string id)
        {
            //try to get a profile matching this url string
            PenProfile? penProfile = GetProfileFromUrlString(id);

            //if there's no profile, return a view with an error message
            if (penProfile == null)
            {
                return RedirectToAction("NotFound");
            }
            else
            {
                if (ProfileBelongsToUser(penProfile.ProfileId))
                {
                    ViewBag.OwnProfile = "true";
                }
                else
                {
                    ViewBag.OwnProfile = "false";
                }

                List<IdentityError> errors = new List<IdentityError>();
                bool isAccessable = IsAccessableByUser(penProfile.AccessPermissionId, ref errors);

                if (isAccessable)
                {
                    return View(penProfile);
                }
                else
                {
                    return RedirectToAction("AccessDenied", new { id = penProfile.AccessPermissionId });
                }
            }
        }

        // GET: ProfileController/Create
        [Route("Profile/Create")]
        public ActionResult Create()
        {
            if (UserHasProfile())
            {
                ViewBag.HasProfile = "true";
            }
            else
            {
                ViewBag.HasProfile = "false";
            }

            return View();
        }

        // POST: ProfileController/Create
        [Route("Profile/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateProfileViewModel model)
        {
            string userId = GetUserId();
            if (ModelState.IsValid)
            {
                //Create Access Permission
                AccessPermission ap = new AccessPermission()
                {
                    PublicAccess = model.PublicAccess,
                    FriendAccess = model.FriendAccess,
                    PublisherAccess = model.PublisherAccess,
                    MinorAccess = model.MinorAccess,
                    ShowsUpInSearch = model.ShowsUpInSearch
                };
                db.AccessPermissions.Add(ap);
                db.SaveChanges();

                //if we are changing which profile is the main profile, make that update first
                if (UserHasProfile() && model.IsMainProfile)
                {
                    PenProfile mainProfile = db.PenProfiles.Where(i => i.UserId == userId && i.IsMainProfile).FirstOrDefault();
                    mainProfile.IsMainProfile = false;
                    db.Entry(mainProfile).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //Create Profile
                PenProfile penProfile = new PenProfile()
                {
                    UserId = userId,
                    DisplayName = model.DisplayName,
                    RoleId = model.RoleType == 3 ? 2 : model.RoleType,
                    AccessPermissionId = ap.AccessPermissionId,
                    UseSecondaryRoleName = model.RoleType == 3 ? true : false,
                    UrlString = model.UrlString == null ? CreateUrlString() : model.UrlString.ToLower().Trim(),
                    IsMainProfile = model.IsMainProfile,
                    ProfileDescription = model.ProfileDescription,
                    ProfileImage = model.ProfileImage != null ? CreateProfileImage(model.ProfileImage) : CreateProfileImage(),
                    Verified = false
                };

                db.PenProfiles.Add(penProfile);
                db.SaveChanges();

                int profileId = penProfile.ProfileId;
                ap.ProfileId = profileId;
                db.Entry(ap).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", new { id = penProfile.UrlString });
            }

            return View(model);
        }

        // GET: ProfileController/Edit/betty-suarez
        [Route("Profile/Edit/{id}")]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id) != true)
            {
                PenProfile? penProfile = db.PenProfiles.Where(i => i.UrlString == id).FirstOrDefault();

                if(penProfile == null)
                {
                    return RedirectToAction("NotFound");
                }

                AccessPermission accessPermission = db.AccessPermissions.Where(i => i.AccessPermissionId == penProfile.AccessPermissionId).FirstOrDefault();

                if(accessPermission == null)
                {
                    return RedirectToAction("NotFound");
                }

                List<IdentityError> errors = new List<IdentityError>();
                bool isAccessable = IsAccessableByUser(penProfile.AccessPermissionId, ref errors, "edit");

                if (isAccessable)
                {
                    string name = penProfile.DisplayName;
                    PenRole role = db.ProfileRoles.Where(i => i.RoleId == penProfile.RoleId).FirstOrDefault();

                    if(role != null)
                    {
                        if(penProfile.UseSecondaryRoleName)
                        {
                            name += " (" + role.SecondaryRoleName + ")";
                        }
                        else
                        {
                            name += " (" + role.RoleName + ")";
                        }
                    }

                    ViewBag.Image = penProfile.ProfileImage;
                    ViewBag.ProfileName = name;

                    EditProfileViewModel viewModel = new EditProfileViewModel()
                    {
                        ProfileId = penProfile.ProfileId,
                        DisplayName = penProfile.DisplayName,
                        IsMainProfile = penProfile.IsMainProfile,
                        ProfileDescription = penProfile.ProfileDescription,
                        UrlString = penProfile.UrlString,
                        ProfileImage = null,
                        PublicAccess = accessPermission.PublicAccess,
                        FriendAccess = accessPermission.FriendAccess,
                        PublisherAccess = accessPermission.PublisherAccess,
                        MinorAccess = accessPermission.MinorAccess,
                        ShowsUpInSearch = accessPermission.ShowsUpInSearch
                    };

                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("EditAccessDenied", new { id = penProfile.AccessPermissionId });
                }
            }

            return RedirectToAction("NotFound");
        }

        // POST: ProfileController/Edit/betty-suarez
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Profile/Edit/{id}")]
        public ActionResult Edit(string id, EditProfileViewModel model)
        {
            string userId = GetUserId();
            PenProfile? penProfile = db.PenProfiles.Where(i => i.UrlString.ToUpper().Trim() == id.ToUpper().Trim()).FirstOrDefault();

            if(penProfile == null)
            {
                return RedirectToAction("NotFound");
            }

            AccessPermission? accessPermission = db.AccessPermissions.Where(i => i.ProfileId == model.ProfileId).FirstOrDefault();
            AccessPermission? ap = db.AccessPermissions.Where(i => i.AccessPermissionId == penProfile.AccessPermissionId).FirstOrDefault();

            //if the profile doesn't belong to the user
            if(penProfile.ProfileId != model.ProfileId)
            {
                return RedirectToAction("EditAccessDenied");
            }
            else if(penProfile.UserId != userId)
            {
                //if accessPermission isn't null and it matches ap
                if(accessPermission != null && accessPermission == ap)
                {
                    return RedirectToAction("EditAccessDenied", new { id = accessPermission.AccessPermissionId });
                } //if both are null
                else if(accessPermission == null && ap == null)
                {
                    AccessPermission access = new AccessPermission
                    {
                        PublicAccess = false,
                        FriendAccess = false,
                        PublisherAccess = false,
                        MinorAccess = false,
                        ShowsUpInSearch = false,
                        ProfileId = penProfile.ProfileId
                    };
                    db.AccessPermissions.Add(access);
                    db.SaveChanges();

                    penProfile.AccessPermissionId = access.AccessPermissionId;
                    db.Entry(penProfile).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("NotFound");
                } //if accessPermission is not null but penProfile is pointing to the wrong AccessPermissionId
                else if(accessPermission != null &&  ap == null)
                {
                    //if there is no conflict, just fix what penProfile is pointing to
                    if(accessPermission.FolderId == null && accessPermission.WritingId == null && accessPermission.SeriesId == null)
                    {
                        penProfile.AccessPermissionId = accessPermission.AccessPermissionId;
                        db.Entry(penProfile).State = EntityState.Modified;
                        db.SaveChanges();
                    } //if there is a conflict, create a new AccessPermission with accessPermission's value and penProfile's id and have penProfile point to that
                    else
                    {
                        accessPermission.ProfileId = null;
                        db.Entry(accessPermission).State = EntityState.Modified;
                        db.SaveChanges();

                        ap = new AccessPermission
                        {
                            ProfileId = penProfile.ProfileId,
                            PublicAccess = accessPermission.PublicAccess,
                            PublisherAccess = accessPermission.PublisherAccess,
                            FriendAccess = accessPermission.FriendAccess,
                            MinorAccess = accessPermission.MinorAccess,
                            ShowsUpInSearch = accessPermission.ShowsUpInSearch
                        };
                        db.AccessPermissions.Add(ap);
                        db.SaveChanges();

                        penProfile.AccessPermissionId = ap.AccessPermissionId;
                        db.Entry(penProfile).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    return RedirectToAction("EditAccessDenied", new { id = penProfile.AccessPermissionId });
                } //if there are no AccessPermission with penProfile's id but penProfile is pointing to ap's AccessPermissionId
                else if(accessPermission == null && ap != null)
                {
                    //if ap isn't assigned to anything else, assign penProfile's id to its ProfileId 
                    if(ap.ProfileId == null && ap.FolderId == null && ap.WritingId == null && ap.SeriesId == null)
                    {
                        ap.ProfileId = penProfile.ProfileId;
                        db.Entry(ap).State = EntityState.Modified;
                        db.SaveChanges();
                    } //if ap is assigned to something else, create a new AccessPermission with ap's values and penProfile's id and have penProfile point to that
                    else
                    {
                        accessPermission = new AccessPermission
                        {
                            ProfileId = penProfile.ProfileId,
                            PublicAccess = ap.PublicAccess,
                            PublisherAccess = ap.PublisherAccess,
                            FriendAccess = ap.FriendAccess,
                            MinorAccess = ap.MinorAccess,
                            ShowsUpInSearch = ap.ShowsUpInSearch
                        };
                        db.AccessPermissions.Add(accessPermission); 
                        db.SaveChanges();

                        penProfile.AccessPermissionId = accessPermission.AccessPermissionId;
                        db.Entry(penProfile).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    return RedirectToAction("EditAccessDenied", new { id = penProfile.AccessPermissionId });
                } //if both accessPermission and ap aren't null but they don't match
                else
                {
                    //if ap's ProfileId matches penProfile's id and there are no other conflicts, set accessPermission to no longer reference penProfile's id
                    if(penProfile.ProfileId == ap.ProfileId && ap.WritingId == null && ap.FolderId == null && ap.SeriesId == null)
                    {
                        accessPermission.ProfileId = null;
                        db.Entry(accessPermission).State = EntityState.Modified;
                        db.SaveChanges();
                    } //if ap matches penProfile's id and has conflicts but accessPermission doesn't, set ap to no longer reference penProfile and update penProfile to point to accessPermission instead
                    else if(penProfile.ProfileId == ap.ProfileId && accessPermission.FolderId == null && accessPermission.WritingId == null && accessPermission.SeriesId == null)
                    {
                        ap.ProfileId = null;
                        db.Entry(ap).State = EntityState.Modified;
                        db.SaveChanges();

                        penProfile.AccessPermissionId = accessPermission.AccessPermissionId;
                        db.Entry(penProfile).State = EntityState.Modified;
                        db.SaveChanges();
                    } //if ap references penProfile but both ap and accessPermission have conflicts, there's something weird going on, just redirect
                    else if(penProfile.ProfileId == ap.ProfileId)
                    {
                        return RedirectToAction("EditAccessDenied");
                    } //if ap doesn't reference penProfile and accessPermission has no conflicts, update penProfile to point to accessPermission instead
                    else if(penProfile.ProfileId != ap.ProfileId && accessPermission.FolderId == null && accessPermission.WritingId == null && accessPermission.SeriesId == null)
                    {
                        penProfile.AccessPermissionId = accessPermission.AccessPermissionId;
                        db.Entry(penProfile).State = EntityState.Modified;
                        db.SaveChanges();
                    } //if ap doesn't reference penProfile and accessPermission has conflicts
                    else
                    {
                        accessPermission.ProfileId = null;
                        db.Entry(accessPermission).State = EntityState.Modified;
                        db.SaveChanges();

                        AccessPermission access = new AccessPermission
                        {
                            ProfileId = penProfile.ProfileId,
                            PublicAccess = accessPermission.PublicAccess == ap.PublicAccess ? ap.PublicAccess : false,
                            PublisherAccess = accessPermission.PublisherAccess == ap.PublisherAccess ? ap.PublisherAccess : false,
                            FriendAccess = accessPermission.FriendAccess == ap.FriendAccess ? ap.FriendAccess : false,
                            MinorAccess = accessPermission.MinorAccess == ap.MinorAccess ? ap.MinorAccess : false,
                            ShowsUpInSearch = accessPermission.ShowsUpInSearch == ap.ShowsUpInSearch ? ap.ShowsUpInSearch : false
                        };
                        db.AccessPermissions.Add(access);
                        db.SaveChanges();

                        penProfile.AccessPermissionId = access.AccessPermissionId;
                        db.Entry(penProfile).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    return RedirectToAction("EditAccessDenied", new { id = penProfile.AccessPermissionId });
                }
            }

            if(ModelState.IsValid)
            {
                //take care of any changes to AccessPermission
                //if there is no access permission connected to this profile, fix that first
                if(accessPermission == null && ap == null)
                {
                    accessPermission = new AccessPermission
                    {
                        ProfileId = model.ProfileId,
                        PublicAccess = model.PublicAccess,
                        FriendAccess = model.FriendAccess,
                        PublisherAccess = model.PublisherAccess,
                        MinorAccess = model.MinorAccess,
                        ShowsUpInSearch = model.ShowsUpInSearch
                    };
                    db.AccessPermissions.Add(accessPermission);
                    db.SaveChanges();

                    penProfile.AccessPermissionId = accessPermission.AccessPermissionId;
                    db.Entry(penProfile).State = EntityState.Modified;
                } //if there's no access permission with this ProfileId, but penProfile has an AccessPermissionId
                else if(accessPermission == null && ap != null)
                {
                    //check first to see if ap is pointing to something else
                    //if it's not, we're golden
                    if(ap.ProfileId == null && ap.WritingId == null && ap.FolderId == null && ap.SeriesId == null)
                    {
                        ap.ProfileId = model.ProfileId;
                        ap.PublicAccess = model.PublicAccess;
                        ap.FriendAccess = model.FriendAccess;
                        ap.PublisherAccess = model.PublisherAccess;
                        ap.MinorAccess = model.MinorAccess;
                        ap.ShowsUpInSearch = model.ShowsUpInSearch;
                        db.Entry(ap).State = EntityState.Modified;
                    } //otherwise, we have to fix some things
                    else
                    {
                        accessPermission = new AccessPermission
                        {
                            ProfileId = model.ProfileId,
                            PublicAccess = model.PublicAccess,
                            FriendAccess = model.FriendAccess,
                            PublisherAccess = model.PublisherAccess,
                            MinorAccess = model.MinorAccess,
                            ShowsUpInSearch = model.ShowsUpInSearch
                        };
                        db.AccessPermissions.Add(accessPermission);
                        db.SaveChanges();

                        penProfile.AccessPermissionId = accessPermission.AccessPermissionId;
                        db.Entry(penProfile).State = EntityState.Modified;
                    }
                } //if there's an access permission with this ProfileId, but penProfile's AccessPermissionId isn't pointing to anything
                else if(accessPermission != null && ap == null)
                {
                    accessPermission.PublicAccess = model.PublicAccess;
                    accessPermission.FriendAccess = model.FriendAccess;
                    accessPermission.PublisherAccess = model.PublisherAccess;
                    accessPermission.MinorAccess = model.MinorAccess;
                    accessPermission.ShowsUpInSearch = model.ShowsUpInSearch;
                    db.Entry(accessPermission).State = EntityState.Modified;
                    db.SaveChanges();

                    penProfile.AccessPermissionId = accessPermission.AccessPermissionId;
                    db.Entry(penProfile).State = EntityState.Modified;
                } //if the access permission pointing to this profile doesn't match the AccessPermissionId of penProfile
                else if(accessPermission != ap)
                {
                    //if accessPermission only has one thing it is pointing to
                    if(accessPermission.FolderId == null && accessPermission.WritingId == null && accessPermission.SeriesId == null)
                    {
                        penProfile.AccessPermissionId = accessPermission.AccessPermissionId;
                        db.Entry(penProfile).State = EntityState.Modified;
                        db.SaveChanges();

                        accessPermission.PublicAccess = model.PublicAccess;
                        accessPermission.FriendAccess = model.FriendAccess;
                        accessPermission.PublisherAccess = model.PublisherAccess;
                        accessPermission.MinorAccess = model.MinorAccess;
                        accessPermission.ShowsUpInSearch = model.ShowsUpInSearch;
                        db.Entry(accessPermission).State = EntityState.Modified;
                    } //if accessPermission is pointing to something else as well as ProfileId and ap is not pointing to anything at all
                    else if(ap.ProfileId == null && ap.WritingId == null && ap.FolderId == null && ap.SeriesId == null)
                    {
                        accessPermission.ProfileId = null;
                        db.Entry(accessPermission).State = EntityState.Modified;
                        db.SaveChanges();

                        ap.ProfileId = penProfile.ProfileId;
                        ap.PublicAccess = model.PublicAccess;
                        ap.FriendAccess = model.FriendAccess;
                        ap.PublisherAccess = model.PublisherAccess;
                        ap.MinorAccess = model.MinorAccess;
                        ap.ShowsUpInSearch = model.ShowsUpInSearch;
                        db.Entry(ap).State = EntityState.Modified;
                    } //if accessPermission is pointing to something else as well as ProfileId and ap is also pointing to something else
                    else
                    {
                        accessPermission.ProfileId = null;
                        db.Entry(accessPermission).State = EntityState.Modified;
                        db.SaveChanges();

                        AccessPermission access = new AccessPermission
                        {
                            ProfileId = penProfile.ProfileId,
                            PublicAccess = model.PublicAccess,
                            FriendAccess = model.FriendAccess,
                            PublisherAccess = model.PublisherAccess,
                            MinorAccess = model.MinorAccess,
                            ShowsUpInSearch = model.ShowsUpInSearch
                        };
                        db.Add(access);
                        db.SaveChanges();

                        penProfile.AccessPermissionId = access.AccessPermissionId;
                        db.Entry(penProfile).State = EntityState.Modified;
                    }
                }
                else
                {
                    accessPermission.PublicAccess = model.PublicAccess;
                    accessPermission.FriendAccess = model.FriendAccess;
                    accessPermission.PublisherAccess = model.PublisherAccess;
                    accessPermission.MinorAccess = model.MinorAccess;
                    accessPermission.ShowsUpInSearch = model.ShowsUpInSearch;
                    db.Entry(accessPermission).State = EntityState.Modified;
                } //AccessPermission stuff
                db.SaveChanges();

                //check if this profile has been changed to their main profile
                if(model.IsMainProfile && !penProfile.IsMainProfile)
                {
                    //get the current main profile
                    PenProfile? mainProfile = db.PenProfiles.Where(i => i.UserId == penProfile.UserId && i.IsMainProfile).FirstOrDefault();

                    //if there is a current main profile, it needs to be changed to no longer be the main profile
                    if(mainProfile != null)
                    {
                        mainProfile.IsMainProfile = false;
                        db.Entry(mainProfile).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    //change the profile we are editing to be the main profile
                    penProfile.IsMainProfile = true;
                    db.Entry(penProfile).State = EntityState.Modified;
                    db.SaveChanges();
                }

                penProfile.ProfileDescription = model.ProfileDescription;
                penProfile.DisplayName = model.DisplayName;
                penProfile.UrlString = model.UrlString;
                db.Entry(penProfile).State = EntityState.Modified;
                db.SaveChanges();

                if(model.ProfileImage != null)
                {
                    penProfile.ProfileImage = CreateProfileImage(model.ProfileImage);
                    db.Entry(penProfile).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
                return RedirectToAction("Index", new { id = penProfile.UrlString });
            }

            return View(model);
        }

        // GET: ProfileController/Delete/betty-suarez
        [Route("Profile/Delete/{id}")]
        public ActionResult Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                PenProfile? penProfile = db.PenProfiles.Where(i => i.UrlString == id).FirstOrDefault();

                if (penProfile == null)
                {
                    return RedirectToAction("NotFound");
                }

                AccessPermission accessPermission = db.AccessPermissions.Where(i => i.AccessPermissionId == penProfile.AccessPermissionId).FirstOrDefault();

                if (accessPermission == null)
                {
                    return RedirectToAction("NotFound");
                }

                List<IdentityError> errors = new List<IdentityError>();
                bool isAccessable = IsAccessableByUser(penProfile.AccessPermissionId, ref errors, "delete");

                if (isAccessable)
                {
                    return View(penProfile);
                }
                else
                {
                    return RedirectToAction("DeleteAccessDenied", new { id = penProfile.AccessPermissionId });
                }
            }

            return RedirectToAction("NotFound");
        }

        // POST: ProfileController/Delete/betty-suarez
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Route("Profile/AccessDenied/{id}")]
        public ActionResult AccessDenied(int id)
        {
            List<IdentityError> errors = new List<IdentityError>();
            IsAccessableByUser(id, ref errors);

            if(errors.Any(i => i.Description == "Request not found."))
            {
                return RedirectToAction("NotFound");
            }

            string errorString = "";

            foreach (IdentityError error in errors)
            {
                errorString += error.Description + " ";
            }

            ViewBag.ErrorString = errorString;

            return View();
        }

        [Route("Profile/EditAccessDenied")]
        public ActionResult EditAccessDenied()
        {
            ViewBag.ErrorString = "You are not allowed to edit a profile you are not the owner of.";

            return View();
        }

        [Route("Profile/EditAccessDenied/{id}")]
        public ActionResult EditAccessDenied(int id)
        {
            List<IdentityError> errors = new List<IdentityError>();
            IsAccessableByUser(id, ref errors, "edit");

            if (errors.Any(i => i.Description == "Request not found."))
            {
                return RedirectToAction("NotFound");
            }

            string errorString = "";

            foreach (IdentityError error in errors)
            {
                errorString += error.Description + " ";
            }

            ViewBag.ErrorString = errorString;

            return View();
        }

        [Route("Profile/DeleteAccessDenied/{id}")]
        public ActionResult DeleteAccessDenied(int id)
        {
            List<IdentityError> errors = new List<IdentityError>();
            IsAccessableByUser(id, ref errors, "delete");

            if (errors.Any(i => i.Description == "Request not found."))
            {
                return RedirectToAction("NotFound");
            }

            string errorString = "";

            foreach (IdentityError error in errors)
            {
                errorString += error.Description + " ";
            }

            ViewBag.ErrorString = errorString;

            return View();
        }

        [Route("Profile/NotFound")]
        public ActionResult NotFound()
        {
            return View();
        }

        public string CreateUrlString()
        {
            string url = "";
            Random rand = new Random();

            //run the loop to get a string of 10 characters
            for (int i = 0; i < 11; i++)
            {
                //determine if a character or number will be selected
                int random = rand.Next(0, 3);
                //get a number
                if (random == 1)
                {
                    random = rand.Next(0, numbers.Count);
                    url += numbers[random].ToString();
                }
                else //get a character
                {
                    random = rand.Next(0, chars.Count);
                    url += chars[random].ToString();
                }
            }

            //check to make sure that the url is unique
            //if the url is not unique, call the method again to get a new url
            if (db.PenProfiles.Any(i => i.UrlString == url))
            {
                CreateUrlString();
            }

            return url;
        } //CreateUrlString

        public byte[] CreateProfileImage(IFormFile file)
        {
            var dataStream = new MemoryStream();
            byte[] profileImage;

            using (dataStream)
            {
                file.CopyToAsync(dataStream);
                profileImage = dataStream.ToArray();
            }

            return profileImage;
        }

        public byte[] CreateProfileImage()
        {
            Stream stream = environment.WebRootFileProvider.GetFileInfo("images/defaultprofileicon.png").CreateReadStream();
            byte[] profileImage = null;
            using(BinaryReader reader = new BinaryReader(stream))
            {
                profileImage = reader.ReadBytes(Convert.ToInt32(stream.Length));
                reader.Close();
            }
            
            return profileImage;
        }
    }
}
