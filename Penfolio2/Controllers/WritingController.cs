using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var writings = GetAllWritingAvailableForSearch();

            return View(writings);
        }

        [Route("Writing/MyWriting")]
        public ActionResult MyWriting()
        {
            List<Writing> myWriting = new List<Writing>();
            var userId = GetUserId();

            if(userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if(!db.Users.Any(i => i.Id == userId))
            {
                return RedirectToAction("Index", "Home");
            }

            var user = GetUserById(userId);

            foreach(var profile in user.PenProfiles.Where(i => i.RoleId == 1))
            {
                List<WritingProfile> writingProfiles = db.WritingProfiles.Where(i => i.ProfileId == profile.ProfileId).ToList();

                foreach(var writingProfile in writingProfiles)
                {
                    var writing = db.Writings.Where(i => i.WritingId == writingProfile.WritingId).FirstOrDefault();

                    if(writing != null)
                    {
                        myWriting.Add(writing);
                    }
                }
            }

            myWriting = OrderByNewest(myWriting.Distinct().ToList());

            return View(myWriting);
        }

        public List<Writing> OrderByNewest(List<Writing> writings)
        {
            return writings.OrderByDescending(i => i.EditDate == null ? i.AddDate : i.EditDate).ToList();
        }

        public List<Writing> GetAllWritingAvailableForSearch()
        {
            var writings = db.Writings.ToList();
            List<Writing> availableWritings = new List<Writing>();

            foreach (var writing in writings)
            {
                List<IdentityError> errors = new List<IdentityError>();
                if (IsAccessableByUser(writing.AccessPermissionId, ref errors, "search"))
                {
                    availableWritings.Add(writing);
                }
            }

            return OrderByNewest(availableWritings);
        }

        public List<Writing> GetAllWritingAvailableForSearch(List<Writing> writings)
        {
            List<Writing> availableWritings = new List<Writing>();

            foreach(var writing in writings)
            {
                List<IdentityError> errors = new List<IdentityError>();
                if (IsAccessableByUser(writing.AccessPermissionId, ref errors, "search"))
                {
                    availableWritings.Add(writing);
                }
            }

            return OrderByNewest(availableWritings);
        }

        public List<Writing> GetAllWritingAvailable()
        {
            var writings = db.Writings.ToList();
            List<Writing> availableWritings = new List<Writing>();

            foreach(var writing in writings)
            {
                List<IdentityError> errors = new List<IdentityError>();
                if(IsAccessableByUser(writing.AccessPermissionId, ref errors))
                {
                    availableWritings.Add(writing);
                }
            }

            return OrderByNewest(availableWritings);
        }

        // GET: WritingController/SearchByFormatTag/Rhyming%20poetry
        [Route("Writing/SearchByFormatTag/{id}")]
        public ActionResult SearchByFormatTag(string id)
        {
            id = Uri.UnescapeDataString(id);
            int formatId = 0;
            string hashtag = "#" + id;
            if(db.FormatTags.Any(i => i.FormatName.ToLower() ==  id.ToLower()))
            {
                formatId = db.FormatTags.Where(i => i.FormatName.ToLower() == id.ToLower()).First().FormatId;
            }
            else if(db.AltFormatNames.Any(i => i.AltName.ToLower() == id.ToLower()))
            {
                formatId = db.AltFormatNames.Where(i => i.AltName.ToLower() == id.ToLower()).First().FormatId;
            }

            List<Writing> writings = new List<Writing>();
            var writingFormats = db.WritingFormats.Where(i => i.FormatId == formatId).ToList();

            foreach(var writingFormat in writingFormats)
            {
                var writing = db.Writings.Where(i => i.WritingId == writingFormat.WritingId).FirstOrDefault();

                if(writing != null)
                {
                    writings.Add(writing);
                }
            }

            writings = GetAllWritingAvailableForSearch(writings);
            ViewBag.Hashtag = hashtag;

            return View(writings);
        }

        // GET: WritingController/SearchByGenreTag/Suspense
        [Route("Writing/SearchByGenreTag/{id}")]
        public ActionResult SearchByGenreTag(string id)
        {
            id = Uri.UnescapeDataString(id);
            int genreId = 0;
            string hashtag = "#" + id;
            if(db.GenreTags.Any(i => i.GenreName.ToLower() == id.ToLower()))
            {
                genreId = db.GenreTags.Where(i => i.GenreName.ToLower() == id.ToLower()).First().GenreId;
            }
            else if(db.AltGenreNames.Any(i => i.AltName.ToLower() == id.ToLower()))
            {
                genreId = db.AltGenreNames.Where(i => i.AltName.ToLower() == id.ToLower()).First().GenreId;
            }

            List<Writing> writings = new List<Writing>();
            var writingGenres = db.WritingGenres.Where(i => i.GenreId == genreId).ToList();

            foreach(var writingGenre in writingGenres)
            {
                var writing = db.Writings.Where(i => i.WritingId == writingGenre.WritingId).FirstOrDefault();

                if(writing != null)
                {
                    writings.Add(writing);
                }
            }

            writings = GetAllWritingAvailableForSearch(writings);

            ViewBag.Hashtag = hashtag;

            return View(writings);
        }

        // GET: WritingController/ViewWriting/5
        [Route("Writing/ViewWriting/{id}")]
        public ActionResult ViewWriting(int id)
        {
            Writing? writing = db.Writings.Where(i => i.WritingId == id).FirstOrDefault();

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
            if(writing != null && !IsAccessableByUser(writing.AccessPermissionId, ref errors))
            {
                return RedirectToAction("AccessDenied", new { id = writing.AccessPermissionId });
            }

            ViewBag.Author = false;

            if(writing != null && IsAccessableByUser(writing.AccessPermissionId, ref errors, "edit"))
            {
                ViewBag.Author = true;
            }

            ViewBag.Document = "";

            if(writing != null)
            {
                string document = HTMLByteArrayToString(writing.Document);

                ViewBag.Document = document;
            }

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
                            model.WritingProfiles = writingProfiles;
                            model.FormatTags = formatTags;
                            model.GenreTags = genreTags;
                            model.FormatCategories = formatCategories;
                            model.GenreCategories = genreCategories;
                            model.GenreFormats = genreFormats;

                            return View(model);
                        }
                    } //if it can't, return the view with an error (TBD)
                    else
                    {
                        model.WritingProfiles = writingProfiles;
                        model.FormatTags = formatTags;
                        model.GenreTags = genreTags;
                        model.FormatCategories = formatCategories;
                        model.GenreCategories = genreCategories;
                        model.GenreFormats = genreFormats;

                        return View(model);
                    }
                }

                //now do the same thing for the selected formats and selected genres
                //with fewer steps because checking for ownership or no selected tags is not required
                string[] formats = model.SelectedFormats != null ? model.SelectedFormats.Split(",") : new string[0];
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
                        model.WritingProfiles = writingProfiles;
                        model.FormatTags = formatTags;
                        model.GenreTags = genreTags;
                        model.FormatCategories = formatCategories;
                        model.GenreCategories = genreCategories;
                        model.GenreFormats = genreFormats;

                        return View(model);
                    }
                }

                string[] genres = model.SelectedGenres != null ? model.SelectedGenres.Split(",") : new string[0];
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
                        model.WritingProfiles = writingProfiles;
                        model.FormatTags = formatTags;
                        model.GenreTags = genreTags;
                        model.FormatCategories = formatCategories;
                        model.GenreCategories = genreCategories;
                        model.GenreFormats = genreFormats;

                        return View(model);
                    }
                }

                //now that we have all of the profiles and tags, it's time to move on and tackle access permission
                AccessPermission ap = new AccessPermission()
                {
                    PublicAccess = model.PublicAccess,
                    FriendAccess = model.FriendAccess,
                    PublisherAccess = model.PublisherAccess,
                    MyAgentAccess = model.MyAgentAccess,
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

            model.WritingProfiles = writingProfiles;
            model.FormatTags = formatTags;
            model.GenreTags = genreTags;
            model.FormatCategories = formatCategories;
            model.GenreCategories = genreCategories;
            model.GenreFormats = genreFormats;

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

            Writing? writing = db.Writings.Where(i => i.WritingId ==  id).FirstOrDefault();

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

            foreach(var writingProfile in writing.WritingProfiles)
            {
                if(writingProfile.PenProfile == null)
                {
                    writingProfile.PenProfile = db.PenProfiles.Where(i => i.ProfileId == writingProfile.ProfileId).First();
                }
            }

            foreach(var wp in writing.WritingProfiles.Select(i => i.PenProfile).Except(writingProfiles).ToList())
            {
                if(wp != null)
                {
                    writingProfiles.Add(wp);
                }
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
                PublicAccess = writing.AccessPermission != null ? writing.AccessPermission.PublicAccess : false,
                FriendAccess = writing.AccessPermission != null ? writing.AccessPermission.FriendAccess : false,
                PublisherAccess = writing.AccessPermission != null ? writing.AccessPermission.PublisherAccess : false,
                MyAgentAccess = writing.AccessPermission != null ? writing.AccessPermission.MyAgentAccess : false,
                MinorAccess = writing.AccessPermission != null ? writing.AccessPermission.MinorAccess : false,
                ShowsUpInSearch = writing.AccessPermission != null ? writing.AccessPermission.ShowsUpInSearch : false,
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

            Writing? writing = db.Writings.Where(i => i.WritingId == id).FirstOrDefault();
            AccessPermission? accessPermission = null;

            if (writing == null)
            {
                return RedirectToAction("NotFound");
            }
            else
            {
                //WritingProfiles
                if(writing.WritingProfiles.Count == 0)
                {
                    writing.WritingProfiles = db.WritingProfiles.Where(i => i.WritingId == id).ToList();
                }
                
                //WritingFormats
                if(writing.WritingFormats.Count == 0)
                {
                    writing.WritingFormats = db.WritingFormats.Where(i => i.WritingId == id).ToList();
                }

                //WritingGenres
                if(writing.WritingGenres.Count == 0)
                {
                    writing.WritingGenres = db.WritingGenres.Where(i => i.WritingId == id).ToList();
                }

                accessPermission = db.AccessPermissions.Where(i => i.AccessPermissionId == writing.AccessPermissionId && i.WritingId != null && i.WritingId.Value == id).FirstOrDefault();
            }

            if(accessPermission == null)
            {
                return RedirectToAction("NotFound");
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
                List<PenProfile> selectedPenProfiles = new List<PenProfile>();

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
                            selectedPenProfiles.Add(p);
                        }
                    } //if it can't, return the view with an error (TBD)
                    else
                    {
                        return View(model);
                    }
                } //for each of the selected profiles

                //if they are the creator of the writing and they are removing all of their profiles as authors, 
                //return to the view with an error saying they need to transfer ownership first (TBD)
                if(writing.UserId == userId && !selectedPenProfiles.Select(i => i.UserId).ToList().Contains(userId))
                {
                    return View(model);
                }

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
                        } //if the writing wasn't created by the user and the profile they are trying to remove doesn't belong to them, return the view with an error (TBD)
                        else if(writing.UserId != userId && p.UserId != userId)
                        {
                            return View(model);
                        }

                        profileIdsToRemove.Add(profile.ProfileId);
                    }
                } //for each of the current writing profiles

                //we have the profiles to remove and the profiles that need to be added, and they are all valid, so we can now move on to the other parts of the edit
                //format the string of selected format tags
                string[] formatTags = model.SelectedFormats != null ? model.SelectedFormats.Split(",") : new string[0];

                //create a list for the FormatIds that SelectedFormats represents
                List<int> selectedFormatIds = new List<int>();

                //for each of the selected formats
                foreach(var format in formatTags)
                {
                    //if the string can be changed into an int
                    if (Int32.TryParse(format, out int formatId))
                    {
                        var tag = db.FormatTags.Where(i => i.FormatId == formatId).FirstOrDefault();

                        //if there's not a format tag connected to that formatId, return the view with an error (TBD)
                        if (tag == null)
                        {
                            return View(model);
                        } //if there is a connected format tag
                        else
                        {
                            selectedFormatIds.Add(formatId);
                        }
                    } //if it can't, return the view with an error (TBD)
                    else
                    {
                        return View(model);
                    }
                }

                //create a list of the format tags that are being added
                List<int> formatIdsToAdd = new List<int>();

                foreach (var formatId in selectedFormatIds)
                {
                    if (!writing.WritingFormats.Select(i => i.FormatId).ToList().Contains(formatId))
                    {
                        formatIdsToAdd.Add(formatId);
                    }
                } //for each of the selected format tags

                //create a list for each of the FormatIds being removed
                List<int> formatIdsToRemove = new List<int>();

                foreach(var format in writing.WritingFormats)
                {
                    //if this is a format tag that's being removed
                    if (!selectedFormatIds.Contains(format.FormatId))
                    {
                        var tag = db.FormatTags.Where(i => i.FormatId == format.FormatId).FirstOrDefault();

                        //if the format tag doesn't exist, return the view with an error (TBD)
                        if (tag == null)
                        {
                            return View(model);
                        }

                        formatIdsToRemove.Add(format.FormatId);
                    }
                } //for each of the current format tags

                //format the string of selected genre tags
                string[] genreTags = model.SelectedGenres != null ? model.SelectedGenres.Split(",") : new string[0];

                //create a list for the GenreIds that SelectedGenres represents
                List<int> selectedGenreIds = new List<int>();

                //for each of the selected genres
                foreach (var genre in genreTags)
                {
                    //if the string can be changed into an int
                    if (Int32.TryParse(genre, out int genreId))
                    {
                        var tag = db.GenreTags.Where(i => i.GenreId == genreId).FirstOrDefault();

                        //if there's not a genre tag connected to that genreId, return the view with an error (TBD)
                        if (tag == null)
                        {
                            return View(model);
                        } //if there is a connected genre tag
                        else
                        {
                            selectedGenreIds.Add(genreId);
                        }
                    } //if it can't, return the view with an error (TBD)
                    else
                    {
                        return View(model);
                    }
                }

                //create a list of the genre tags that are being added
                List<int> genreIdsToAdd = new List<int>();

                foreach (var genreId in selectedGenreIds)
                {
                    if (!writing.WritingGenres.Select(i => i.GenreId).ToList().Contains(genreId))
                    {
                        genreIdsToAdd.Add(genreId);
                    }
                } //for each of the selected genre tags

                //create a list for each of the GenreIds being removed
                List<int> genreIdsToRemove = new List<int>();

                foreach (var genre in writing.WritingGenres)
                {
                    //if this is a genre tag that's being removed
                    if (!selectedGenreIds.Contains(genre.GenreId))
                    {
                        var tag = db.GenreTags.Where(i => i.GenreId == genre.GenreId).FirstOrDefault();

                        //if the genre tag doesn't exist, return the view with an error (TBD)
                        if (tag == null)
                        {
                            return View(model);
                        }

                        genreIdsToRemove.Add(genre.GenreId);
                    }
                } //for each of the current genre tags

                //now we can actually start making the changes

                //update public access
                if(accessPermission.PublicAccess != model.PublicAccess)
                {
                    accessPermission.PublicAccess = model.PublicAccess;
                    db.Entry(accessPermission).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //update friend access
                if(accessPermission.FriendAccess != model.FriendAccess)
                {
                    accessPermission.FriendAccess = model.FriendAccess;
                    db.Entry(accessPermission).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //update publisher access
                if(accessPermission.PublisherAccess != model.PublisherAccess)
                {
                    accessPermission.PublisherAccess = model.PublisherAccess;
                    db.Entry(accessPermission).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //update my agent access
                if(accessPermission.MyAgentAccess != model.MyAgentAccess)
                {
                    accessPermission.MyAgentAccess = model.MyAgentAccess;
                    db.Entry(accessPermission).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //update minor access
                if(accessPermission.MinorAccess != model.MinorAccess)
                {
                    accessPermission.MinorAccess = model.MinorAccess;
                    db.Entry(accessPermission).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //update shows up in search
                if(accessPermission.ShowsUpInSearch != model.ShowsUpInSearch)
                {
                    accessPermission.ShowsUpInSearch = model.ShowsUpInSearch;
                    db.Entry(accessPermission).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //add new writing profiles
                foreach(var profileId in profileIdsToAdd)
                {
                    WritingProfile writingProfile = new WritingProfile
                    {
                        WritingId = id,
                        ProfileId = profileId
                    };

                    db.WritingProfiles.Add(writingProfile);
                    db.SaveChanges();
                }

                //remove old writing profiles
                foreach(var profileId in profileIdsToRemove)
                {
                    WritingProfile? writingProfile = db.WritingProfiles.Where(i => i.WritingId == id && i.ProfileId == profileId).FirstOrDefault();

                    if(writingProfile != null)
                    {
                        db.WritingProfiles.Remove(writingProfile);
                        db.SaveChanges();
                    }
                }

                //add new writing formats
                foreach(var formatId in formatIdsToAdd)
                {
                    WritingFormat writingFormat = new WritingFormat
                    {
                        WritingId = id,
                        FormatId = formatId
                    };

                    db.WritingFormats.Add(writingFormat);
                    db.SaveChanges();
                }

                //remove old writing formats
                foreach(var formatId in formatIdsToRemove)
                {
                    WritingFormat? writingFormat = db.WritingFormats.Where(i => i.WritingId == id && i.FormatId == formatId).FirstOrDefault();

                    if(writingFormat != null)
                    {
                        db.WritingFormats.Remove(writingFormat);
                        db.SaveChanges();
                    }
                }

                //add new writing genres
                foreach(var genreId in genreIdsToAdd)
                {
                    WritingGenre writingGenre = new WritingGenre
                    {
                        WritingId = id,
                        GenreId = genreId,
                    };

                    db.WritingGenres.Add(writingGenre);
                    db.SaveChanges();
                }

                //remove old writing genres
                foreach(var genreId in genreIdsToRemove)
                {
                    WritingGenre? writingGenre = db.WritingGenres.Where(i => i.WritingId == id && i.GenreId == genreId).FirstOrDefault();

                    if(writingGenre != null)
                    {
                        db.WritingGenres.Remove(writingGenre);
                        db.SaveChanges();
                    }
                }

                //update title
                if(writing.Title != model.Title)
                {
                    writing.Title = model.Title;
                    db.Entry(writing).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //update description
                if(writing.Description != model.Description)
                {
                    writing.Description = model.Description;
                    db.Entry(writing).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //update document
                if(writing.Document != Encoding.Unicode.GetBytes(model.EditorContent))
                {
                    writing.Document = Encoding.Unicode.GetBytes(model.EditorContent);
                    db.Entry(writing).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //update edit date
                writing.EditDate = DateTime.Now;
                db.Entry(writing).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("ViewWriting", new { id = id });
            } //if the model state is valid

            return View(model);
        }

        // GET: WritingController/Delete/5
        [Route("Writing/Delete/{id}")]
        public ActionResult Delete(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Writing? writing = db.Writings.Where(i => i.WritingId == id).FirstOrDefault();
            AccessPermission? accessPermission = null;

            if (writing == null)
            {
                return RedirectToAction("NotFound");
            }
            else
            {
                accessPermission = db.AccessPermissions.Where(i => i.AccessPermissionId == writing.AccessPermissionId && i.WritingId == id).FirstOrDefault();
            }

            if(accessPermission == null)
            {
                return RedirectToAction("NotFound");
            }

            List<IdentityError> errors = new List<IdentityError>();
            if(!IsAccessableByUser(accessPermission.AccessPermissionId, ref errors, "delete"))
            {
                return RedirectToAction("DeleteAccessDenied", new { id = accessPermission.AccessPermissionId });
            }

            ViewBag.WritingId = id;

            return View();
        }

        // POST: WritingController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Writing/Delete/{id}")]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Writing? writing = db.Writings.Where(i => i.WritingId == id).FirstOrDefault();
            AccessPermission? accessPermission = null;

            if (writing == null)
            {
                return RedirectToAction("NotFound");
            }
            else
            {
                accessPermission = db.AccessPermissions.Where(i => i.AccessPermissionId == writing.AccessPermissionId && i.WritingId == id).FirstOrDefault();
            }

            if (accessPermission == null)
            {
                return RedirectToAction("NotFound");
            }

            List<IdentityError> errors = new List<IdentityError>();
            if (!IsAccessableByUser(accessPermission.AccessPermissionId, ref errors, "delete"))
            {
                return RedirectToAction("DeleteAccessDenied", new { id = accessPermission.AccessPermissionId });
            }

            //WritingFormat, WritingGenre, and WritingProfile all cascade on Writing delete
            //Writing, IndividualAccessGrant, and IndividualAccessRevoke cascade on AccessPermission delete
            //Critiques, CritiqueRequest, Comments, CommentFlags, CommentReplies, Likes, WritingFolder, and WritingSeries need to be handled seperately 

            //handle Critiques
            List<int> critiqueIds = db.Critiques.Where(i => i.WritingId == id).ToList().Select(i => i.CritiqueId).ToList();

            foreach(var critiqueId in critiqueIds)
            {
                var critique = db.Critiques.Where(i => i.CritiqueId == critiqueId).FirstOrDefault();

                if(critique != null)
                {
                    db.Critiques.Remove(critique);
                    db.SaveChanges();
                }
            }

            //handle CritiqueRequest
            var critiqueRequest = db.CritiqueRequests.Where(i => i.WritingId == id).FirstOrDefault();

            if(critiqueRequest != null)
            {
                db.CritiqueRequests.Remove(critiqueRequest);
                db.SaveChanges();
            }

            //handle Comments, CommentFlags, and CommentReplies
            List<Comment> comments = new List<Comment>();
            List<CommentReply> commentReplies = new List<CommentReply>();
            List<CommentFlag> commentFlags = new List<CommentFlag>();

            GetCommentsForWritingId(id, ref comments, ref commentReplies, ref commentFlags);

            List<int> commentIds = comments.Count > 0 ? comments.Select(i => i.CommentId).ToList() : new List<int>();
            List<int> commentReplyIds = commentReplies.Count > 0 ? commentReplies.Select(i => i.ReplyId).ToList() : new List<int>();
            List<int> commentFlagIds = commentFlags.Count > 0 ? commentFlags.Select(i => i.CommentFlagId).ToList() : new List<int>();

            //now, do the actual deleting
            //remove comment replies for comments connected to this writing
            foreach (var commentReplyId in commentReplyIds)
            {
                var commentReply = db.CommentReplies.Where(i => i.ReplyId == commentReplyId).FirstOrDefault();

                if (commentReply != null)
                {
                    db.CommentReplies.Remove(commentReply);
                    db.SaveChanges();
                }
            }

            //remove comment flags for comments connected to this writing
            foreach (var commentFlagId in commentFlagIds)
            {
                var commentFlag = db.CommentFlags.Where(i => i.CommentFlagId == commentFlagId).FirstOrDefault();

                if (commentFlag != null)
                {
                    db.CommentFlags.Remove(commentFlag);
                    db.SaveChanges();
                }
            }

            //remove comments connected to this writing
            foreach (var commentId in commentIds)
            {
                var comment = db.Comments.Where(i => i.CommentId == commentId).FirstOrDefault();

                if (comment != null)
                {
                    db.Comments.Remove(comment);
                    db.SaveChanges();
                }
            }

            //handle Likes
            List<int> likeIds = db.Likes.Where(i => i.WritingId == id).ToList().Select(i => i.LikeId).ToList();

            foreach(var likeId in likeIds)
            {
                var like = db.Likes.Where(i => i.LikeId == likeId).FirstOrDefault();

                if(like != null)
                {
                    db.Likes.Remove(like);
                    db.SaveChanges();
                }
            }

            //handle WritingFolders
            List<int> writingFolderIds = db.WritingFolders.Where(i => i.WritingId == id).ToList().Select(i => i.FolderId).ToList();

            //delete writing folders connected to this writing
            foreach(var writingFolderId in writingFolderIds)
            {
                var  writingFolder = db.WritingFolders.Where(i => i.WritingId == id && i.FolderId == writingFolderId).FirstOrDefault();

                if(writingFolder !=  null)
                {
                    db.WritingFolders.Remove(writingFolder);
                    db.SaveChanges();
                }
            }

            //handle WritingSeries
            List<int> writingSeriesIds = db.WritingSeries.Where(i => i.WritingId == id).ToList().Select(i => i.WritingSeriesId).ToList();
            List<WritingSeries> nextWritingSeries = db.WritingSeries.Where(i => i.NextWritingId == id).ToList();
            List<WritingSeries> previousWritingSeries = db.WritingSeries.Where(i => i.PreviousWritingId == id).ToList();

            //update the writing series where this is the next writing
            foreach(var writingSeries in nextWritingSeries)
            {
                var nextWriting = db.WritingSeries.Where(i => i.SeriesId == writingSeries.SeriesId && i.WritingId == writing.WritingId).FirstOrDefault();

                writingSeries.NextWritingId = nextWriting != null ? nextWriting.NextWritingId : null;
                db.Entry(writingSeries).State = EntityState.Modified;
                db.SaveChanges();
            }

            //update the writing series where this is the previous writing
            foreach(var writingSeries in previousWritingSeries)
            {
                var previousWriting = db.WritingSeries.Where(i => i.SeriesId == writingSeries.SeriesId && i.WritingId == writing.WritingId).FirstOrDefault();

                writingSeries.PreviousWritingId = previousWriting != null ? previousWriting.PreviousWritingId : null;
                db.Entry(writingSeries).State = EntityState.Modified;
                db.SaveChanges();
            }

            //delete writing series connected to this writing
            foreach(var writingSeriesId in writingSeriesIds)
            {
                var writingSeries = db.WritingSeries.Where(i => i.WritingSeriesId == writingSeriesId).FirstOrDefault();

                if(writingSeries != null)
                {
                    db.WritingSeries.Remove(writingSeries);
                    db.SaveChanges();
                }
            }

            db.AccessPermissions.Remove(accessPermission);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Route("Writing/AccessDenied/{id}")]
        public ActionResult AccessDenied(int id)
        {
            List<IdentityError> errors = new List<IdentityError>();
            IsAccessableByUser(id, ref errors);

            ViewBag.AccessPermissionId = id;

            if (errors.Any(i => i.Description == "Request not found."))
            {
                return RedirectToAction("NotFound");
            }

            string errorString = "";

            foreach (IdentityError error in errors)
            {
                errorString += error.Description + " ";
            }

            ViewBag.VerifiedPublisher = UserHasVerifiedPublisherProfile();
            ViewBag.RepresentationRequest = false;

            if(errorString.Contains("represent the owner"))
            {
                ViewBag.RepresentationRequest = true;
            }

            ViewBag.FriendRequest = false;

            if (errorString.Contains("send a friend request"))
            {
                ViewBag.FriendRequest = true;
            }

            ViewBag.AccessRequest = false;

            if (errorString.Contains("request individual access"))
            {
                ViewBag.AccessRequest = true;
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

        public string HTMLByteArrayToString(byte[] input)
        {
            if (input == null)
            {
                return "";
            }

            string output = Encoding.Unicode.GetString(input);
            output = output.Replace("&lt;", "<").Replace("&gt;", ">").Replace("'", "&#39;");

            return output;
        }
    }
}
