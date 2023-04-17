using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Penfolio2.Models
{
    public class Series
    {
        public Series()
        {
            SeriesWritings = new HashSet<WritingSeries>();
            Subseries = new HashSet<SeriesSeries>();
            ParentSeries = new HashSet<SeriesSeries>();
            Owners = new HashSet<SeriesOwner>();
            Comments = new HashSet<Comment>();
            Likes = new HashSet<Like>();
            Followers = new HashSet<FollowerFollowing>();
        }

        [Required]
        [Key]
        public int SeriesId { get; set; }

        [Required, NotNull]
        [ForeignKey("AccessPermission")]
        public int AccessPermissionId { get; set; }

        [Required, NotNull]
        [ForeignKey("PenUser")]
        public string CreatorId { get; set; }

        [Required, NotNull]
        public string SeriesName { get; set; }

        [AllowNull]
        public string? SeriesDescription { get; set; }

        [Required, NotNull]
        public bool IsComplete { get; set; } = false;

        [Required, NotNull]
        public bool IsStandAlone { get; set; } = false;

        [AllowNull]
        public int? PreviousSeriesId { get; set; }

        [AllowNull]
        public int? NextSeriesId { get; set; }

        public virtual AccessPermission AccessPermission { get; set; }

        public virtual PenUser Creator { get; set; }

        [ForeignKey("SeriesId")]
        public virtual ICollection<WritingSeries> SeriesWritings { get; set; }

        [ForeignKey("OverarchingSeriesId")]
        public virtual ICollection<SeriesSeries> Subseries { get; set; }

        [ForeignKey("SeriesMemberId")]
        public virtual ICollection<SeriesSeries> ParentSeries { get; set; }

        public virtual ICollection<SeriesOwner> Owners { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Like> Likes { get; set; }

        public virtual ICollection<FollowerFollowing> Followers { get; set; }

        [AllowNull]
        public virtual Series PreviousSeries { get; set; }

        [AllowNull]
        public virtual Series NextSeries { get; set; }
    }
}
