using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Penfolio2.Models
{
    [Table("Comments")]
    public class Comment
    {
        public Comment()
        {
            CommentFlags = new HashSet<CommentFlag>();
            Likes = new HashSet<Like>();
            CommentReplies = new HashSet<CommentReply>();
        }

        [Required]
        [Key]
        public int CommentId { get; set; }

        [Required, NotNull]
        //[ForeignKey("PenProfile")]
        public int CommenterId { get; set; }

        [Required, NotNull]
        public DateTime AddDate { get; set; }

        [AllowNull]
        public DateTime? EditDate { get; set; }

        [Required, NotNull]
        public string CommentText { get; set; }

        public int? ProfileId { get; set; }

        [ForeignKey("Writing")]
        public int? WritingId { get; set; }

        [ForeignKey("Folder")]
        public int? FolderId { get; set; }

        [ForeignKey("Series")]
        public int? SeriesId { get; set; }

        [ForeignKey("CommenterId")]
        public virtual PenProfile Commenter { get; set; }

        [ForeignKey("ProfileId")]
        public virtual PenProfile CommentProfile { get; set; }

        public virtual Writing Writing { get; set; }

        public virtual Folder Folder { get; set; }

        public virtual Series Series { get; set; }

        public virtual ICollection<CommentFlag> CommentFlags { get; set; }

        public virtual ICollection<Like> Likes { get; set; }

        //[ForeignKey("CommentId")]
        public virtual ICollection<CommentReply> CommentReplies { get; set; }

        //[ForeignKey("ReplyId")]
        public virtual CommentReply ReplyTo { get; set; }
    }

    public class CommentFlag
    {
        [Required]
        [Key]
        public int CommentFlagId { get; set; }

        [Required, NotNull]
        [ForeignKey("Comment")]
        public int CommentId { get; set; }

        [Required, NotNull]
        [ForeignKey("PenProfile")]
        public int FlaggerId { get; set; }

        [Required, NotNull]
        public DateTime FlagDate { get; set; }

        public string? FlagReason { get; set; }

        public virtual Comment Comment { get; set; }

        //[ForeignKey("FlaggerId")]
        public virtual PenProfile Flagger { get; set; }
    }

    [PrimaryKey(nameof(CommentId), nameof(ReplyId))]
    public class CommentReply
    {
        [Required, NotNull]
        public int CommentId { get; set; }

        [Required, NotNull]
        public int ReplyId { get; set; }

        [ForeignKey("CommentId")]
        public virtual Comment Comment { get; set; }

        [ForeignKey("ReplyId")]
        public virtual Comment Reply { get; set; }
    }
}
