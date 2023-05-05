using Microsoft.AspNetCore.Mvc;
using Penfolio2.Data;
using Penfolio2.Models;


namespace Penfolio2.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public NotificationViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public IViewComponentResult Invoke()
        {
            var userId = GetUserId();
            List<NotificationViewComponent> notifications = new List<NotificationViewComponent>();

            if(userId == null)
            {
                return View(notifications);
            }

            PenUser user = _db.PenUsers.Where(i => i.Id == userId).FirstOrDefault();

            if (user == null)
            {
                return View(notifications);
            }


            return View();
        }

        public string? GetUserName()
        {
            return User?.Identity?.Name;
        }

        protected string? GetUserId()
        {
            var username = GetUserName();

            if (username == null)
            {
                return null;
            }

            return _db.PenUsers.Where(i => i.NormalizedUserName == username.ToUpper()).FirstOrDefault()?.Id;
        }
    }
}
