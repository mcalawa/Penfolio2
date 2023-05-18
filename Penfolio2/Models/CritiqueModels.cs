using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Penfolio2.Models
{
    public class Critique
    {
        [Required]
        [Key]
        public int CritiqueId { get; set; }

        [Required, NotNull]
        [ForeignKey("PenProfile")]
        public int CriticId { get; set; }

        [Required, NotNull]
        [ForeignKey("Writing")]
        public int WritingId { get; set; }

        [Required, NotNull]
        public DateTime CritiqueDate { get; set; }

        public DateTime? EditDate { get; set; }

        [Required, NotNull]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public byte[] EditedDocument { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        //[ForeignKey("CriticId")]
        public virtual PenProfile? Critic { get; set; }

        public virtual Writing? Writing { get; set; }
    }

    public class CritiqueRequest
    {
        [Required]
        [Key]
        public int CritiqueRequestId { get; set; }

        [Required, NotNull]
        [ForeignKey("Writing")]
        public int WritingId { get; set; }

        [Required, NotNull]
        public DateTime RequestDate { get; set; }

        [Required, NotNull]
        public bool Active { get; set; }

        public virtual Writing? Writing { get; set; }
    }

    public class CritiqueGiver
    {
        [Required]
        [Key]
        public int CritiqueGiverId { get; set; }

        [Required, NotNull]
        [ForeignKey("PenProfile")]
        public int CriticId { get; set; }

        [Required, NotNull]
        public bool ForAny { get; set; } = false;

        [Required, NotNull]
        public bool ForFriends { get; set; } = false;

        [Required, NotNull]
        public bool ForMyWriters { get; set; } = false;

        [Required, NotNull]
        public bool ForProfilesFollowing { get; set; } = false;

        [Required, NotNull]
        public bool ForFoldersFollowing { get; set; } = false;

        [Required, NotNull]
        public bool ForSeriesFollowing { get; set; } = false;

        [Required, NotNull]
        public bool ForFormatFollowing { get; set; } = false;

        [Required, NotNull]
        public bool ForGenreFollowing { get; set; } = false;

        [Required, NotNull]
        public bool ForMatureWriting { get; set; } = false;

        //[ForeignKey("CriticId")]
        public virtual PenProfile? Critic { get; set; }
    }
}
