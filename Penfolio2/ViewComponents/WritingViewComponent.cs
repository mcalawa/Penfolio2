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

        public async Task<IViewComponentResult> InvokeAsync(string viewName = null)
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

            return View();
        }

        public IViewComponentResult Invoke(int id)
        {
            string username = User?.Identity?.Name;

            if (username != null)
            {
                username = username.ToUpper().Trim();
            }

            PenUser currentUser = _db.PenUsers.Where(i => i.NormalizedUserName == username).FirstOrDefault();
            Writing writing = _db.Writings.Where(i => i.WritingId == id).FirstOrDefault();

            if(writing == null || currentUser == null)
            {
                return View();
            }

            ViewBag.Author = false;

            if(currentUser.Id == writing.UserId)
            {
                ViewBag.Author = true;
            }
            else
            {
                List<WritingProfile> owners = _db.WritingProfiles.Where(i => i.WritingId == id).ToList();

                foreach(var owner in owners)
                {
                    if(owner.PenProfile == null)
                    {
                        owner.PenProfile = _db.PenProfiles.Where(i => i.ProfileId == owner.ProfileId).First();
                    }

                    if(owner.PenProfile.UserId == currentUser.Id)
                    {
                        ViewBag.Author = true;
                    }
                }
            }

            return View(writing);
        }
    }
}

