using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Penfolio2.Models
{
    public class Writing
    {
        public Writing()
        {
            WritingProfiles = new HashSet<WritingProfile>();
            WritingFolders = new HashSet<WritingFolder>();
            WritingSeries = new HashSet<WritingSeries>();
            WritingFormats = new HashSet<WritingFormat>();
            WritingGenres = new HashSet<WritingGenre>();
            Comments = new HashSet<Comment>();
            Likes = new HashSet<Like>();
            Critiques = new HashSet<Critique>();
        }

        [Required]
        [Key]
        public int WritingId { get; set; }

        [Required, NotNull]
        [ForeignKey("AccessPermission")]
        public int AccessPermissionId { get; set; }

        [Required, NotNull]
        [ForeignKey("PenUser")]
        public string UserId { get; set; } = string.Empty;

        [Required, NotNull]
        public string Title { get; set; } = string.Empty;

        [Required, NotNull]
        public byte[] Document { get; set; }

        [Required, NotNull]
        [DisplayFormat(DataFormatString = "{0:MMMM d, yyyy h:mm tt}")]
        public DateTime AddDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMMM d, yyyy h:mm tt}")]
        public DateTime? EditDate { get; set; }

        [Required, NotNull]
        public bool LikesOn { get; set; } = false;

        [Required, NotNull]
        public bool CommentsOn { get; set; } = false;

        [Required, NotNull] 
        public bool CritiqueOn { get; set; } = false;

        [AllowNull]
        public string? Description { get; set; }

        [Required, NotNull]
        public bool IsStandAlone { get; set; } = true;

        //[ForeignKey("UserId")]
        public virtual PenUser? PenUser { get; set; }

        public virtual AccessPermission? AccessPermission { get; set; }

        public virtual ICollection<WritingProfile> WritingProfiles { get; set; }

        public virtual ICollection<WritingFolder> WritingFolders { get; set; }

        public virtual ICollection<WritingSeries> WritingSeries { get; set; }

        public virtual ICollection<WritingFormat> WritingFormats { get; set; }

        public virtual ICollection<WritingGenre> WritingGenres { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Like> Likes { get; set; }

        public virtual ICollection<Critique> Critiques { get; set; }

        public virtual CritiqueRequest? CritiqueRequest { get; set; }
    }
}
