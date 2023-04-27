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
            return writings.OrderByDescending(i => i.EditDate == null ? i.EditDate : i.AddDate).ToList();
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

        // GET: WritingController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: WritingController/Create
        public ActionResult Create()
        {
            if(!UserHasWriterProfile())
            {
                return RedirectToAction("Create", "Profile");
            }

            var userId = GetUserId();

            if(userId == null)
            {
                return View();
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

            var model = new CreateWritingViewModel
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateWritingViewModel model)
        {
            var userId = GetUserId();

            if(userId == null)
            {
                return View(model);
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
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: WritingController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: WritingController/Delete/5
        public ActionResult Delete(int id)
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

        public List<SelectFormatTagViewModel> PopulateFormatTags()
        {
            List<SelectFormatTagViewModel> formatTagsForView = new List<SelectFormatTagViewModel>();
            List<FormatTag> formatTags = db.FormatTags.ToList();

            foreach(var tag in formatTags)
            {
                var formatTagViewModel = PopulateSingleFormatTag(tag);

                formatTagsForView.Add( formatTagViewModel );
            }

            return formatTagsForView;
        }

        public SelectFormatTagViewModel PopulateSingleFormatTag(FormatTag tag)
        {
            var formatTagViewModel = new SelectFormatTagViewModel
            {
                FormatId = tag.FormatId,
                FormatName = tag.FormatName,
                Explanation = tag.Explanation == null ? "" : tag.Explanation,
                IsFictionOnly = tag.IsFictionOnly,
                IsNonfictionOnly = tag.IsNonfictionOnly,
                AltNames = new List<string>(),
                Parents = new List<Tuple<SelectFormatTagViewModel, SelectFormatTagViewModel>>()
            };

            if (db.AltFormatNames.Any(i => i.FormatId == tag.FormatId))
            {
                var altFormatNames = db.AltFormatNames.Where(i => i.FormatId == tag.FormatId).ToList();

                foreach (var name in altFormatNames)
                {
                    formatTagViewModel.AltNames.Add(name.AltName);
                }
            }

            if (db.FormatCategories.Any(i => i.FormatId == tag.FormatId))
            {
                var formatCategories = db.FormatCategories.Where(i => i.FormatId == tag.FormatId).ToList();

                foreach (var category in formatCategories)
                {
                    var firstParent = db.FormatTags.Where(i => i.FormatId == category.ParentId).FirstOrDefault();
                    var secondParent = category.SecondaryParentId == null ? null : db.FormatTags.Where(i => i.FormatId == category.SecondaryParentId).FirstOrDefault();
                    var fp = PopulateSingleFormatTag(firstParent);
                    var sp = secondParent == null ? null : PopulateSingleFormatTag(secondParent);

                    var setOfParents = Tuple.Create(fp, sp);

                    formatTagViewModel.Parents.Add(setOfParents);
                }
            }

            return formatTagViewModel;
        }
    }
}
