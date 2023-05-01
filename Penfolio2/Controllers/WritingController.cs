using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Penfolio2.Data;
using Penfolio2.Models;
using System.Text;

namespace Penfolio2.Controllers
{
    [Authorize]
    public class WritingController : AccessController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly IWebHostEnvironment environment;

        public WritingController(IWebHostEnvironment environment) : base()
        {
            this.environment = environment;
        }

        // GET: WritingController
        [Route("Writing/")]
        [Route("Writing/Index")]
        public ActionResult Index()
        {
            var writings = GetAllWritingAvailable();

            return View(writings);
        }

        public List<Writing> OrderByNewest(List<Writing> writings)
        {
            return writings.OrderByDescending(i => i.EditDate == null ? i.AddDate : i.EditDate).ToList();
        }

        public List<Writing> GetAllWritingAvailableForSearch()
        {
            var writings = db.Writings.ToList();

            foreach (var writing in writings)
            {
                List<IdentityError> errors = new List<IdentityError>();
                if (!IsAccessableByUser(writing.AccessPermissionId, ref errors, "search"))
                {
                    writings.Remove(writing);
                }
            }

            return OrderByNewest(writings);
        }

        public List<Writing> GetAllWritingAvailable()
        {
            var writings = db.Writings.ToList();

            foreach(var writing in writings)
            {
                List<IdentityError> errors = new List<IdentityError>();
                if(!IsAccessableByUser(writing.AccessPermissionId, ref errors))
                {
                    writings.Remove(writing);
                }
            }

            return OrderByNewest(writings);
        }

        // GET: WritingController/ViewWriting/5
        [Route("Writing/ViewWriting/{id}")]
        public ActionResult ViewWriting(int id)
        {
            Writing writing = db.Writings.Where(i => i.WritingId == id).FirstOrDefault();

            if(writing == null)
            {
                return RedirectToAction("NotFound");
            }

            if(writing != null)
            {
                writing = PopulateWriting(writing);
            }

            List<IdentityError> errors = new List<IdentityError>();

            //if the user is not allowed to access this, redirect to error (TBD)
            if(!IsAccessableByUser(writing.AccessPermissionId, ref errors))
            {
                return RedirectToAction("AccessDenied");
            }

            ViewBag.Author = false;

            if(IsAccessableByUser(writing.AccessPermissionId, ref errors, "edit"))
            {
                ViewBag.Author = true;
            }

            string document = HTMLByteArrayToString(writing.Document);

            ViewBag.Document = document;

            return View(writing);
        }

        // GET: WritingController/Create
        [Route("Writing/Create")]
        public ActionResult Create()
        {
            if(!UserHasWriterProfile())
            {
                return RedirectToAction("Create", "Profile");
            }

            var userId = GetUserId();

            if(userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var writingProfiles = GetWriterProfiles(userId);
            var formatTags = db.FormatTags.ToList();
            var genreTags = db.GenreTags.ToList();
            var formatCategories = db.FormatCategories.ToList();
            var genreCategories = db.GenreCategories.ToList();
            var genreFormats = db.GenreFormats.ToList();

            ViewBag.Profiles = String.Join(",", writingProfiles.Select(i => i.ProfileId));
            ViewBag.FormatTags = String.Join(",", formatTags.Select(i => i.FormatId));
            ViewBag.GenreTags = String.Join(",", genreTags.Select(i => i.GenreId));
            ViewBag.IsCreator = true;

            var model = new WritingViewModel
            {
                WritingProfiles = writingProfiles,
                GenreTags = genreTags,
                FormatTags = formatTags,
                GenreCategories = genreCategories,
                FormatCategories = formatCategories,
                GenreFormats = genreFormats,
                Description = null
            };

            return View(model);
        }

        // POST: WritingController/Create
        [Route("Writing/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WritingViewModel model)
        {
            var userId = GetUserId();

            if(userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if(ModelState.IsValid)
            {
                //format the string of selected profiles
                string[] profiles = model.SelectedProfiles.Split(",");

                //if none have been selected, return to the view with an error (TBD)
                if(profiles.Length < 1 || (profiles.Length == 1 && (profiles[0] == "" || profiles[0] == String.Empty))) 
                {
                    return View(model);
                }

                //now we need to create a list for the ProfileIds that SelectedProfiles represents
                List<int> profileIds = new List<int>();

                //for each of the selected profiles
                foreach(var profile in profiles)
                {
                    //if the string can be changed into an int
                    if(Int32.TryParse(profile, out int profileId))
                    {
                        //if the profileId is a writing profile that belongs to the current user, add it to the list
                        if(db.PenProfiles.Any(i => i.UserId == userId && i.ProfileId == profileId && i.RoleId == 1))
                        {
                            profileIds.Add(profileId);
                        } //if not, return the view with an error (TBD)
                        else
                        {
                            return View(model);
                        }
                    } //if it can't, return the view with an error (TBD)
                    else
                    {
                        return View(model);
                    }
                }

                //now do the same thing for the selected formats and selected genres
                //with fewer steps because checking for ownership or no selected tags is not required
                string[] formats = model.SelectedFormats.Split(",");
                List<int> formatIds = new List<int>();

                foreach(var format in formats)
                {
                    //if the string can be changed into an int, add it to the list
                    if(Int32.TryParse(format, out int formatId))
                    {
                        if(db.FormatTags.Any(i => i.FormatId == formatId))
                        {
                            formatIds.Add(formatId);
                        }
                    } //if not, return the view with an error (TBD)
                    else
                    {
                        return View(model);
                    }
                }

                string[] genres = model.SelectedGenres.Split(",");
                List<int> genreIds = new List<int>();

                foreach (var genre in genres)
                {
                    //if the string can be changed into an int, add it to the list
                    if (Int32.TryParse(genre, out int genreId))
                    {
                        if(db.GenreTags.Any(i => i.GenreId == genreId))
                        {
                            genreIds.Add(genreId);
                        }
                    } //if not, return the view with an error (TBD)
                    else
                    {
                        return View(model);
                    }
                }

                //now that we have all of the profiles and tags, it's time to move on and tackle access permission
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

                //create the writing
                Writing writing = new Writing
                {
                    UserId = userId,
                    AccessPermissionId = ap.AccessPermissionId,
                    Title = model.Title,
                    Description = model.Description,
                    AddDate = DateTime.Now,
                    EditDate = null,
                    Document = Encoding.Unicode.GetBytes(model.EditorContent)
                };
                db.Writings.Add(writing);
                db.SaveChanges();

                //update ap to have a matching WritingId
                ap.WritingId = writing.WritingId;
                db.Entry(ap).State = EntityState.Modified;
                db.SaveChanges();

                //create the connections between the writing and the profiles
                foreach(var profileId in profileIds)
                {
                    WritingProfile writingProfile = new WritingProfile
                    {
                        WritingId = writing.WritingId,
                        ProfileId = profileId
                    };
                    db.WritingProfiles.Add(writingProfile);
                    db.SaveChanges();
                }

                //create the connection between the writing and the format tags
                foreach(var formatId in formatIds)
                {
                    WritingFormat writingFormat = new WritingFormat
                    {
                        WritingId = writing.WritingId,
                        FormatId = formatId
                    };
                    db.WritingFormats.Add(writingFormat);
                    db.SaveChanges();
                }

                //create the connection between the writing and the genre tags
                foreach(var genreId in genreIds)
                {
                    WritingGenre writingGenre = new WritingGenre
                    {
                        WritingId = writing.WritingId,
                        GenreId = genreId
                    };
                    db.WritingGenres.Add(writingGenre);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            } //if ModelState.IsValid

            return View(model);
        }

        // GET: WritingController/Edit/5
        [Route("Writing/Edit/{id}")]
        public ActionResult Edit(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Writing writing = db.Writings.Where(i => i.WritingId ==  id).FirstOrDefault();

            if(writing == null)
            {
                return RedirectToAction("NotFound");
            }
            else
            {
                writing = PopulateWriting(writing);
            }

            List<IdentityError> errors = new List<IdentityError>();
            if(!IsAccessableByUser(writing.AccessPermissionId, ref errors, "edit"))
            {
                return RedirectToAction("EditAccessDenied", new { id = writing.AccessPermissionId });
            }

            var writingProfiles = GetWriterProfiles(userId);

            foreach(var wp in writing.WritingProfiles.Select(i => i.PenProfile).Where(i => i.UserId != userId).ToList())
            {
                writingProfiles.Add(wp);
            }

            var formatTags = db.FormatTags.ToList();
            var genreTags = db.GenreTags.ToList();
            var formatCategories = db.FormatCategories.ToList();
            var genreCategories = db.GenreCategories.ToList();
            var genreFormats = db.GenreFormats.ToList();

            ViewBag.Profiles = String.Join(",", writingProfiles.Select(i => i.ProfileId));
            ViewBag.FormatTags = String.Join(",", formatTags.Select(i => i.FormatId));
            ViewBag.GenreTags = String.Join(",", genreTags.Select(i => i.GenreId));

            List<int> selectedProfileIds = new List<int>();

            foreach(var wp in writing.WritingProfiles.ToList())
            {
                selectedProfileIds.Add(wp.ProfileId);
            }

            List<int> selectedFormatIds = new List<int>();

            foreach(var wf in writing.WritingFormats.ToList())
            {
                selectedFormatIds.Add(wf.FormatId);
            }

            List<int> selectedGenreIds = new List<int>();

            foreach(var wg in writing.WritingGenres.ToList())
            {
                selectedGenreIds.Add(wg.GenreId);
            }

            var model = new WritingViewModel
            {
                Title = writing.Title,
                Description = writing.Description,
                EditorContent = HTMLByteArrayToString(writing.Document),
                PublicAccess = writing.AccessPermission.PublicAccess,
                FriendAccess = writing.AccessPermission.FriendAccess,
                PublisherAccess = writing.AccessPermission.PublisherAccess,
                MinorAccess = writing.AccessPermission.MinorAccess,
                ShowsUpInSearch = writing.AccessPermission.ShowsUpInSearch,
                WritingProfiles = writingProfiles,
                FormatTags = formatTags,
                GenreTags = genreTags,
                FormatCategories = formatCategories,
                GenreCategories = genreCategories,
                GenreFormats = genreFormats,
                SelectedProfileIds = selectedProfileIds,
                SelectedFormatIds = selectedFormatIds,
                SelectedGenreIds = selectedGenreIds
            };

            ViewBag.WritingId = writing.WritingId;

            return View(model);
        }

        // POST: WritingController/Edit/5
        [Route("Writing/Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, WritingViewModel model)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Writing writing = db.Writings.Where(i => i.WritingId == id).FirstOrDefault();

            if (writing == null)
            {
                return RedirectToAction("NotFound");
            }
            else
            {
                writing = PopulateWriting(writing);
            }

            List<IdentityError> errors = new List<IdentityError>();
            if (!IsAccessableByUser(writing.AccessPermissionId, ref errors, "edit"))
            {
                return RedirectToAction("EditAccessDenied", new { id = writing.AccessPermissionId });
            }

            if(ModelState.IsValid)
            {
                //format the string of selected profiles
                string[] profiles = model.SelectedProfiles.Split(",");

                //if none have been selected, return to the view with an error (TBD)
                if (profiles.Length < 1 || (profiles.Length == 1 && (profiles[0] == "" || profiles[0] == String.Empty)))
                {
                    return View(model);
                }

                //now we need to create a list for the ProfileIds that SelectedProfiles represents
                List<int> selectedProfileIds = new List<int>();

                //for each of the selected profiles
                foreach (var profile in profiles)
                {
                    //if the string can be changed into an int
                    if (Int32.TryParse(profile, out int profileId))
                    {
                        var p = db.PenProfiles.Where(i => i.ProfileId == profileId).FirstOrDefault();

                        //if there's not profile connected to that profileId, return the view with an error (TBD)
                        if(p == null)
                        {
                            return View(model);
                        } //if there is a connected profile
                        else
                        {
                            //populate the profile
                            p = PopulatePenProfile(p);

                            //if it's not a writing profile, return with an error (TBD)
                            if(p.RoleId != 1)
                            {
                                return View(model);
                            } //if the writing wasn't created by the user and the profile isn't among the current selected profiles, return with an error (TBD)
                            else if(writing.UserId != userId && !writing.WritingProfiles.Select(i => i.ProfileId).ToList().Contains(profileId))
                            {
                                return View(model);
                            }

                            selectedProfileIds.Add(profileId);
                        }
                    } //if it can't, return the view with an error (TBD)
                    else
                    {
                        return View(model);
                    }
                } //for each of the selected profiles

                //create a list of the profiles that are being added
                List<int> profileIdsToAdd = new List<int>();

                foreach (var profileId in selectedProfileIds)
                {
                    if(!writing.WritingProfiles.Select(i => i.ProfileId).ToList().Contains(profileId))
                    {
                        profileIdsToAdd.Add(profileId);
                    }
                } //for each of the selected profiles

                //create a list for the profiles that are being removed
                List<int> profileIdsToRemove = new List<int>();

                foreach(var profile in writing.WritingProfiles)
                {
                    //if this is a profile that's being removed
                    if(!selectedProfileIds.Contains(profile.ProfileId))
                    {
                        var p = db.PenProfiles.Where(i => i.ProfileId == profile.ProfileId).FirstOrDefault();

                        //if the profile doesn't exist, return the view with an error (TBD)
                        if(p == null)
                        {
                            return View(model);
                        }
                        else
                        {
                            p = PopulatePenProfile(p);
                        }

                        //if the writing wasn't created by the user and the profile they are trying to remove doesn't belong to them, return the view with an error (TBD)
                        if(writing.UserId != userId && p.UserId != userId)
                        {
                            return View(model);
                        }

                        profileIdsToRemove.Add(profile.ProfileId);
                    }
                } //for each of the current writing profiles

                //we have the profiles to remove and the profiles that need to be added, and they are all valid, so we can now move on to the other parts of the edit
            } //if the model state is valid

            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WritingController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        [Route("Writing/AccessDenied/{id}")]
        public ActionResult AccessDenied(int id)
        {
            List<IdentityError> errors = new List<IdentityError>();
            IsAccessableByUser(id, ref errors);

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

        [Route("Writing/EditAccessDenied")]
        public ActionResult EditAccessDenied()
        {
            ViewBag.ErrorString = "You are not allowed to edit a writing you are not the owner of or a collaborator on.";

            return View();
        }

        [Route("Writing/EditAccessDenied/{id}")]
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

        [Route("Writing/DeleteAccessDenied")]
        public ActionResult DeleteAccessDenied()
        {
            ViewBag.ErrorString = "You are not allowed to delete a writing you are not the owner of.";

            return View();
        }

        [Route("Writing/DeleteAccessDenied/{id}")]
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

        [Route("Writing/NotFound")]
        public new ActionResult NotFound()
        {
            return View();
        }

        // POST: WritingController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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

        public string HTMLByteArrayToString(byte[] input)
        {
            if (input == null)
            {
                return null;
            }

            string output = Encoding.Unicode.GetString(input);
            output = output.Replace("&lt;", "<").Replace("&gt;", ">").Replace("'", "&#39;");

            return output;
        }
    }
}
