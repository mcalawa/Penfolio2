using Microsoft.AspNetCore.Mvc;
using Penfolio2.Data;
using Penfolio2.Models;


namespace Penfolio2.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
