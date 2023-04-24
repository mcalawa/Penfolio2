using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Penfolio2.Data;
using Penfolio2.Models;

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
            return View();
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

            return writings;
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
                Description = null
            };

            return View(model);
        }

        // POST: WritingController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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
    }
}
