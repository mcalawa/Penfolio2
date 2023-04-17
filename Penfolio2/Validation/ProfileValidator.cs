using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Penfolio2.Data;
using Penfolio2.Models;
using Penfolio2.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Penfolio2.Validation
{
    public class ProfileValidator
    {
        public class DisplayNameForCreateAttribute : ValidationAttribute
        {
            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                var profile = (CreateProfileViewModel)validationContext.ObjectInstance;
                List<IdentityError> errors = new List<IdentityError>();
                DbContextOptions<ApplicationDbContext> options = new DbContextOptions<ApplicationDbContext>();
                ApplicationDbContext db = new ApplicationDbContext(options);
                AccessController ac = new AccessController();
                ClaimsPrincipal cp = new ClaimsPrincipal();
                string? otherUsername = cp?.Identity?.Name;
                string? username = ac.GetUserName();
                PenUser user;

                if (profile == null)
                {
                    errors.Add(new IdentityError()
                    {
                        Description = "Profile fields must be completed."
                    });
                }
                else if (username == null)
                {
                    errors.Add(new IdentityError()
                    {
                        Description = "Profiles can only be created by registered users. If you have an account, please log in."
                    });
                }
                else
                {
                    user = db.PenUsers.Where(i => i.NormalizedUserName == username.ToUpper()).FirstOrDefault();

                    if (user == null)
                    {
                        errors.Add(new IdentityError()
                        {
                            Description = "Profiles can only be created by registered users. If you have an account, please log in."
                        });
                    }
                    else
                    {
                        //populate the user
                        if (user.PenProfiles.Count == 0)
                        {
                            user.PenProfiles = db.PenProfiles.Where(i => i.UserId == user.Id).ToList();
                        }

                        foreach (var penProfile in user.PenProfiles)
                        {
                            if (penProfile.PenRole == null)
                            {
                                penProfile.PenRole = db.ProfileRoles.Where(i => i.RoleId == penProfile.RoleId).FirstOrDefault();
                            }
                        }

                        IsValidDisplayName(profile.DisplayName, user, null, profile.RoleType == 3 ? 2 : profile.RoleType, profile.RoleType == 3 ? true : false, ref errors);
                    } //if user is not null
                }

                string errorString = "";

                foreach (IdentityError error in errors)
                {
                    errorString += error.Description + " ";
                }

                return errors.Count == 0 ? ValidationResult.Success : new ValidationResult(errorString);
            }
        }

        public class DisplayNameForEditAttribute : ValidationAttribute
        {
            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                var profile = (EditProfileViewModel)validationContext.ObjectInstance;
                List<IdentityError> errors = new List<IdentityError>();
                DbContextOptions<ApplicationDbContext> options = new DbContextOptions<ApplicationDbContext>();
                ApplicationDbContext db = new ApplicationDbContext(options);
                AccessController ac = new AccessController();
                PenProfile currentProfile;
                string? username = ac.GetUserName();
                PenUser user;

                if (profile == null)
                {
                    errors.Add(new IdentityError()
                    {
                        Description = "Profile fields must be completed."
                    });
                }
                else if (username == null)
                {
                    errors.Add(new IdentityError()
                    {
                        Description = "Profiles can only be created by registered users. If you have an account, please log in."
                    });
                }
                else
                {
                    user = db.PenUsers.Where(i => i.NormalizedUserName == username.ToUpper()).FirstOrDefault();
                    currentProfile = db.PenProfiles.Where(i => i.ProfileId == profile.ProfileId).FirstOrDefault();

                    if (user == null)
                    {
                        errors.Add(new IdentityError()
                        {
                            Description = "Profiles can only be created by registered users. If you have an account, please log in."
                        });
                    }
                    else if (currentProfile == null)
                    {
                        errors.Add(new IdentityError()
                        {
                            Description = "No profile found."
                        });
                    }
                    else
                    {
                        //populate the user
                        if (user.PenProfiles.Count == 0)
                        {
                            user.PenProfiles = db.PenProfiles.Where(i => i.UserId == user.Id).ToList();
                        }

                        foreach (var penProfile in user.PenProfiles)
                        {
                            if (penProfile.PenRole == null)
                            {
                                penProfile.PenRole = db.ProfileRoles.Where(i => i.RoleId == penProfile.RoleId).FirstOrDefault();
                            }
                        }

                        IsValidDisplayName(profile.DisplayName, user, profile.ProfileId, currentProfile.RoleId, currentProfile.UseSecondaryRoleName, ref errors);
                    } //if user is not null
                }

                string errorString = "";

                foreach (IdentityError error in errors)
                {
                    errorString += error.Description + " ";
                }

                return errors.Count == 0 ? ValidationResult.Success : new ValidationResult(errorString);
            }
        }

        public class UrlStringForCreateAttribute : ValidationAttribute
        {
            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                var profile = (CreateProfileViewModel)validationContext.ObjectInstance;
                List<IdentityError> errors = new List<IdentityError>();
                DbContextOptions<ApplicationDbContext> options = new DbContextOptions<ApplicationDbContext>();
                ApplicationDbContext db = new ApplicationDbContext(options);
                AccessController ac = new AccessController();
                string? username = ac.GetUserName();

                if (profile == null)
                {
                    errors.Add(new IdentityError()
                    {
                        Description = "Profile fields must be completed."
                    });
                }
                else if (username == null)
                {
                    errors.Add(new IdentityError()
                    {
                        Description = "Profiles can only be created by registered users. If you have an account, please log in."
                    });
                }
                else
                {
                    IsValidUrlString(profile.UrlString, null, ref errors);
                }

                string errorString = "";

                foreach (IdentityError error in errors)
                {
                    errorString += error.Description + " ";
                }

                return errors.Count == 0 ? ValidationResult.Success : new ValidationResult(errorString);
            }
        }

        public class UrlStringForEditAttribute : ValidationAttribute
        {
            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                var profile = (EditProfileViewModel)validationContext.ObjectInstance;
                List<IdentityError> errors = new List<IdentityError>();
                DbContextOptions<ApplicationDbContext> options = new DbContextOptions<ApplicationDbContext>();
                ApplicationDbContext db = new ApplicationDbContext(options);
                AccessController ac = new AccessController();
                string? username = ac.GetUserName();

                if (profile == null)
                {
                    errors.Add(new IdentityError()
                    {
                        Description = "Profile fields must be completed."
                    });
                }
                else if (username == null)
                {
                    errors.Add(new IdentityError()
                    {
                        Description = "Profiles can only be created by registered users. If you have an account, please log in."
                    });
                }
                else
                {
                    IsValidUrlString(profile.UrlString, profile.ProfileId, ref errors);
                }

                string errorString = "";

                foreach (IdentityError error in errors)
                {
                    errorString += error.Description + " ";
                }

                return errors.Count == 0 ? ValidationResult.Success : new ValidationResult(errorString);
            }
        }

        public static bool IsValidDisplayName(string? displayName, PenUser user, int? profileId, int roleId, bool useSecondaryRoleName, ref List<IdentityError> errors)
        {
            if (errors == null)
            {
                errors = new List<IdentityError>();
            } //create the error list if it doesn't already exist

            int errorCount = errors.Count;

            if (displayName == null)
            {
                errors.Add(new IdentityError()
                {
                    Description = "Please enter a display name."
                });
                return false;
            }
            else
            {
                displayName = NormalizeName(displayName);
            }

            List<PenProfile> penProfiles;

            if (profileId == null)
            {
                penProfiles = user.PenProfiles.ToList();
            }
            else
            {
                penProfiles = user.PenProfiles.Where(i => i.ProfileId != profileId).ToList();
            }

            foreach (var penProfile in penProfiles)
            {
                if (displayName.Equals(NormalizeName(penProfile.DisplayName)) && penProfile.RoleId == roleId && penProfile.UseSecondaryRoleName == useSecondaryRoleName)
                {
                    string roleName = useSecondaryRoleName ? penProfile.PenRole.SecondaryRoleName : penProfile.PenRole.RoleName;

                    errors.Add(new IdentityError()
                    {
                        Description = String.Format("You already have a {0} profile with that display name. Please enter another display name or select a different role type.", roleName)
                    });
                    return false;
                }
            }

            return errors.Count == errorCount ? true : false;
        }

        public static bool IsValidUrlString(string? urlString, int? profileId, ref List<IdentityError> errors)
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptions<ApplicationDbContext>();
            ApplicationDbContext db = new ApplicationDbContext(options);

            if (errors == null)
            {
                errors = new List<IdentityError>();
            } //create the error list if it doesn't already exist

            int errorCount = errors.Count;

            if (urlString == null)
            {
                return true;
            }
            else
            {
                urlString = NormalizeName(urlString);

                if (urlString.Length > 40)
                {
                    errors.Add(new IdentityError()
                    {
                        Description = "A URL string cannot be more than 40 characters long."
                    });
                }

                var urlChars = urlString.ToCharArray();

                foreach (var uc in urlChars)
                {
                    if (!(uc <= '9' && uc >= '0') && !(uc >= 'a' && uc <= 'z') && uc != '-' && uc != '_')
                    {
                        errors.Add(new IdentityError()
                        {
                            Description = "This URL string contains an invalid character. The valid characters for a URL string are the special characters \"-\" and \"_\" and the alphanumeric characters A-Z and 0-9."
                        });
                        break;
                    }
                }

                List<string> urlStrings;

                if (profileId == null)
                {
                    urlStrings = db.PenProfiles.Select(i => i.UrlString).ToList();
                }
                else
                {
                    urlStrings = db.PenProfiles.Where(i => i.ProfileId != profileId).Select(i => i.UrlString).ToList();
                }

                foreach (var us in urlStrings)
                {
                    if (us.Equals(urlString))
                    {
                        errors.Add(new IdentityError
                        {
                            Description = "This URL string is already in use by another user."
                        });
                        break;
                    }
                }
            }

            return errors.Count == errorCount ? true : false;
        }

        public static string NormalizeName(string name)
        {
            return name.ToLower().Trim();
        }
    }
}
