using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using Penfolio2.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Penfolio2.Areas.Identity.Pages.Account;

namespace Penfolio2.Models
{
    public class PenUser : IdentityUser
    {
        public PenUser()
        {
            PenProfiles = new HashSet<PenProfile>();
            UsersBlockedBy = new HashSet<UserBlock>();
            BlockedUsers = new HashSet<UserBlock>();
        }

        public string? GivenName { get; set; }

        public string? Surname { get; set; }

        public DateTime? Birthdate { get; set; }

        [Required, NotNull]
        public bool UseLowDataMode { get; set; } = false;

        [Required, NotNull]
        public int Strikes { get; set; } = 0;

        [Required, NotNull]
        public DateTime LastNotificationViewDate { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public virtual ICollection<PenProfile> PenProfiles { get; set; }

        [ForeignKey("BlockedUserId")]
        public virtual ICollection<UserBlock> UsersBlockedBy { get; set; }

        [ForeignKey("BlockingUserId")]
        public virtual ICollection<UserBlock> BlockedUsers { get; set; }
    }

    public class ApplicationUserManager : UserManager<PenUser>
    {
        private readonly UserStore<PenUser, IdentityRole, ApplicationDbContext, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>> _store;

        public ApplicationUserManager(
            IUserStore<PenUser> store,
            IOptions<IdentityOptions> options,
            IPasswordHasher<PenUser> passwordHasher,
            IEnumerable<IUserValidator<PenUser>> userValidators,
            IEnumerable<IPasswordValidator<PenUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errorDescriber,
            IServiceProvider services,
            ILogger<UserManager<PenUser>> logger)
            : base(store, options, passwordHasher, userValidators, passwordValidators, keyNormalizer, errorDescriber, services, logger)
        {
            _store = (UserStore<PenUser, IdentityRole, ApplicationDbContext, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>)store;
        }

        /// <summary>
        /// Normalize email for consistent comparisons.
        /// </summary>
        /// <param name="email">The email to normalize.</param>
        /// <returns>A normalized value representing the specified <paramref name="email"/>.</returns>
        [return: NotNullIfNotNull("email")]
        public override string? NormalizeEmail(string? email)
#pragma warning disable CS8604 // Possible null reference argument.
            => (KeyNormalizer == null) ? email : CustomUsernameEmailPolicy.NormalizeEmail(email);
#pragma warning restore CS8604 // Possible null reference argument.
    }
}
