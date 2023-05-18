using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Penfolio2.Data;
using Penfolio2.Models;

namespace Penfolio2.Controllers
{
    public class SearchController : AccessController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly IWebHostEnvironment environment;

        public SearchController(IWebHostEnvironment environment) : base()
        {
            this.environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
