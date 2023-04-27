using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Penfolio2.Data;
using Penfolio2.Models;

namespace Penfolio2.ViewComponents
{
    public class WritingViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public WritingViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? id = null, string viewName = null)
        {
            if(!string.IsNullOrEmpty(viewName))
            {
                if(viewName.CompareTo("SelectFormat")  == 0)
                {
                    var model = _db.FormatTags.ToList();

                    return View(viewName, model);
                }
                else if(viewName.CompareTo("SelectGenre") == 0)
                {
                    var model = _db.GenreTags.ToList();

                    return View(viewName, model);
                }
            }

            if(id == null)
            {
                return View();
            }

            string username = User?.Identity?.Name;

            if (username != null)
            {
                username = username.ToUpper().Trim();
            }

            PenUser currentUser = _db.PenUsers.Where(i => i.NormalizedUserName == username).FirstOrDefault();
            Writing writing = _db.Writings.Where(i => i.WritingId == id.Value).FirstOrDefault();

            if (writing == null || currentUser == null)
            {
                return View();
            }

            ViewBag.Author = false;

            if (currentUser.Id == writing.UserId)
            {
                ViewBag.Author = true;
            }
            else
            {
                List<WritingProfile> owners = _db.WritingProfiles.Where(i => i.WritingId == id).ToList();

                foreach (var owner in owners)
                {
                    if (owner.PenProfile == null)
                    {
                        owner.PenProfile = _db.PenProfiles.Where(i => i.ProfileId == owner.ProfileId).First();
                    }

                    if (owner.PenProfile.UserId == currentUser.Id)
                    {
                        ViewBag.Author = true;
                    }
                }

                if (writing.WritingFormats == null || writing.WritingFormats.Count == 0)
                {
                    writing.WritingFormats = _db.WritingFormats.Where(i => i.WritingId == writing.WritingId).ToList();

                    foreach (var format in writing.WritingFormats)
                    {
                        if (format.FormatTag == null)
                        {
                            format.FormatTag = _db.FormatTags.Where(i => i.FormatId == format.FormatId).FirstOrDefault();

                            if (format.FormatTag.AltFormatNames == null || format.FormatTag.AltFormatNames.Count == 0)
                            {
                                format.FormatTag.AltFormatNames = _db.AltFormatNames.Where(i => i.FormatId == format.FormatId).ToList();
                            }
                        }
                    }
                }

                if (writing.WritingGenres == null || writing.WritingGenres.Count == 0)
                {
                    writing.WritingGenres = _db.WritingGenres.Where(i => i.WritingId == writing.WritingId).ToList();

                    foreach (var genre in writing.WritingGenres)
                    {
                        if (genre.GenreTag == null)
                        {
                            genre.GenreTag = _db.GenreTags.Where(i => i.GenreId == genre.GenreId).FirstOrDefault();

                            if (genre.GenreTag.AltGenreNames == null || genre.GenreTag.AltGenreNames.Count == 0)
                            {
                                genre.GenreTag.AltGenreNames = _db.AltGenreNames.Where(i => i.GenreId == genre.GenreId).ToList();
                            }
                        }
                    }
                }
            }

            return View(writing);
        }
    }
}

