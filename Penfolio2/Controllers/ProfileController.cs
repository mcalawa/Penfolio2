using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Penfolio2.Data;
using Penfolio2.Models;

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
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
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
                    return View(penProfile);
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
        public ActionResult Edit(string id, IFormCollection collection)
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

        // GET: ProfileController/Delete/betty-suarez
        [Route("Profile/Delete/{id}")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
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
