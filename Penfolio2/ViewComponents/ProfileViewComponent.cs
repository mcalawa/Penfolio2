using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Penfolio2.Data;
using Penfolio2.Models;

namespace Penfolio2.ViewComponents
{
    public class ProfileViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public ProfileViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public IViewComponentResult Invoke(string urlString)
        {
            if (urlString != null)
            {
                urlString = urlString.ToLower().Trim();
            }

            var penProfile = _db.PenProfiles.Where(i => i.UrlString == urlString).FirstOrDefault();
            List<PenProfile> penProfiles = new List<PenProfile>();
            string? username = User?.Identity?.Name;

            if (username != null)
            {
                username = username.ToUpper().Trim();
            }

            PenUser? currentUser = _db.PenUsers.Where(i => i.NormalizedUserName == username).FirstOrDefault();

            if (currentUser != null && penProfile != null)
            {
                var userId = penProfile.UserId;

                if (currentUser.Id == userId)
                {
                    List<PenProfile> userProfiles = _db.PenProfiles.Where(i => i.UserId == userId).ToList();

                    penProfiles.Add(penProfile);
                    userProfiles.Remove(penProfile);

                    if (!penProfile.IsMainProfile)
                    {
                        var mainProfile = _db.PenProfiles.Where(i => i.IsMainProfile && i.UserId == userId).FirstOrDefault();

                        if(mainProfile != null)
                        {
                            penProfiles.Add(mainProfile);
                            userProfiles.Remove(mainProfile);
                        }
                    }

                    foreach (var profile in userProfiles)
                    {
                        penProfiles.Add(profile);
                    }

                    foreach (var profile in penProfiles)
                    {
                        if (profile.PenRole == null)
                        {
                            profile.PenRole = _db.ProfileRoles.Where(i => i.RoleId == profile.RoleId).FirstOrDefault();
                        }
                    }

                    return View(penProfiles);
                }
            }

            return View();
        }
    }
}
