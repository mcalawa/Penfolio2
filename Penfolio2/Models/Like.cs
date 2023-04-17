using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Penfolio2.Models
{
    [Table("Likes")]
    public class Like
    {
        [Required]
        [Key]
        public int LikeId { get; set; }

        [Required, NotNull]
        //[ForeignKey("PenProfile")]
        public int LikerId { get; set; }

        [Required, NotNull]
        public bool IsAnonymous { get; set; } = false;

        [Required, NotNull]
        public DateTime LikeDate { get; set; }

        [AllowNull]
        public int? ProfileId { get; set; }

        [ForeignKey("Writing")]
        [AllowNull]
        public int? WritingId { get; set; }

        [ForeignKey("Folder")]
        [AllowNull]
        public int? FolderId { get; set; }

        [ForeignKey("Series")]
        [AllowNull]
        public int? SeriesId { get; set; }

        [ForeignKey("Comment")]
        [AllowNull]
        public int? CommentId { get; set; }

        [ForeignKey("LikerId")]
        public virtual PenProfile Liker { get; set; }

        [ForeignKey("ProfileId")]
        [AllowNull]
        public virtual PenProfile LikedProfile { get; set; }

        [AllowNull]
        public virtual Writing LikedWriting { get; set; }

        [AllowNull]
        public virtual Folder LikedFolder { get; set; }

        [AllowNull]
        public virtual Series LikedSeries { get; set; }

        [AllowNull]
        public virtual Comment LikedComment { get; set; }
    }
}
