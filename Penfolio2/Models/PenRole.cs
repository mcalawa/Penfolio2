using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Penfolio2.Models
{
    public class PenRole
    {
        public PenRole()
        {
            RoleProfiles = new HashSet<PenProfile>();
        }

        [Required]
        [Key]
        public int RoleId { get; set; }

        [Required, NotNull]
        public string RoleName { get; set; } = string.Empty;

        [AllowNull]
        public string? SecondaryRoleName { get; set; }

        [Required, NotNull]
        public bool CanPostWritings { get; set; }

        [Required, NotNull]
        public bool CanRepresentWriters { get; set; }

        public virtual ICollection<PenProfile> RoleProfiles { get; set; }
    }
}
