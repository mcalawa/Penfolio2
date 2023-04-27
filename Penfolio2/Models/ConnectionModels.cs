using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Penfolio2.Models
{
    [PrimaryKey(nameof(FolderId), nameof(FormatId))]
    public class FolderFormat
    {
        [Required, NotNull]
        [ForeignKey("Folder")]
        public int FolderId { get; set; }

        [Required, NotNull]
        [ForeignKey("FormatTag")]
        public int FormatId { get; set; }

        public virtual Folder Folder { get; set; }

        public virtual FormatTag FormatTag { get; set; }
    }

    [PrimaryKey(nameof(FolderId), nameof(GenreId))]
    public class FolderGenre
    {
        [Required, NotNull]
        [ForeignKey("Folder")]
        public int FolderId { get; set; }

        [Required, NotNull]
        [ForeignKey("GenreTag")]
        public int GenreId { get; set; }

        public virtual Folder Folder { get; set; }

        public virtual GenreTag GenreTag { get; set; }
    }

    [PrimaryKey(nameof(FolderId), nameof(OwnerId))]
    public class FolderOwner
    {
        [Required, NotNull]
        [ForeignKey("Folder")]
        public int FolderId { get; set; }

        [Required, NotNull]
        [ForeignKey("PenProfile")]
        public int OwnerId { get; set; }

        public virtual Folder Folder { get; set; }

        public virtual PenProfile Owner { get; set; }
    }

    [PrimaryKey(nameof(FolderId), nameof(SubfolderId))]
    public class FolderSubfolder
    {
        [Required, NotNull]
        public int FolderId { get; set; }

        [Required, NotNull]
        public int SubfolderId { get; set; }

        [ForeignKey("FolderId")]
        public virtual Folder Folder { get; set; }

        [ForeignKey("SubfolderId")]
        public virtual Folder Subfolder { get; set; }
    }

    public class FollowerFollowing
    {
        [Required]
        [Key]
        public int FollowerFollowingId { get; set; }

        [Required, NotNull]
        public int FollowerId { get; set; }

        [AllowNull]
        public int? ProfileId { get; set; }

        [ForeignKey("Folder")]
        [AllowNull]
        public int? FolderId { get; set; }

        [ForeignKey("FormatTag")]
        [AllowNull]
        public int? FormatId { get; set; }

        [ForeignKey("GenreTag")]
        [AllowNull]
        public int? GenreId { get; set; }

        [ForeignKey("Series")]
        [AllowNull]
        public int? SeriesId { get; set; }

        [ForeignKey(nameof(FollowerId))]
        public virtual PenProfile Follower { get; set; }

        [ForeignKey("ProfileId")]
        [AllowNull]
        public virtual PenProfile FollowingProfile { get; set; }

        [AllowNull]
        public virtual Folder Folder { get; set; }

        [AllowNull]
        public virtual Series Series { get; set; }

        [AllowNull]
        public virtual FormatTag FormatTag { get; set; }

        [AllowNull]
        public virtual GenreTag GenreTag { get; set; }
    }

    [PrimaryKey(nameof(SeriesId), nameof(FormatId))]
    public class SeriesFormat
    {
        [Required, NotNull]
        [ForeignKey("Series")]
        public int SeriesId { get; set; }

        [Required, NotNull]
        [ForeignKey("FormatTag")]
        public int FormatId { get; set; }

        public virtual Series Series { get; set; }

        public virtual FormatTag FormatTag { get; set; }
    }

    [PrimaryKey(nameof(SeriesId), nameof(GenreId))]
    public class SeriesGenre
    {
        [Required, NotNull]
        [ForeignKey("Series")]
        public int SeriesId { get; set; }

        [Required, NotNull]
        [ForeignKey("GenreTag")]
        public int GenreId { get; set; }

        public virtual Series Series { get; set; }

        public virtual GenreTag GenreTag { get; set; }
    }

    [PrimaryKey(nameof(SeriesId), nameof(OwnerId))]
    public class SeriesOwner
    {
        [Required, NotNull]
        [ForeignKey("Series")]
        public int SeriesId { get; set; }

        [Required, NotNull]
        [ForeignKey("PenProfile")]
        public int OwnerId { get; set; }

        public virtual Series Series { get; set; }

        public virtual PenProfile Owner { get; set; }
    }

    [PrimaryKey(nameof(OverarchingSeriesId), nameof(SeriesMemberId))]
    public class SeriesSeries
    {
        [Required, NotNull]
        public int OverarchingSeriesId { get; set; }

        [Required, NotNull]
        public int SeriesMemberId { get; set; }

        [Required, NotNull]
        public bool IsStandAlone { get; set; }

        [ForeignKey("OverarchingSeriesId")]
        public virtual Series OverarchingSeries { get; set; }

        [ForeignKey("SeriesMemberId")]
        public virtual Series SeriesMember { get; set; }
    }

    [PrimaryKey(nameof(WritingId), nameof(FolderId))]
    public class WritingFolder
    {
        [Required, NotNull]
        [ForeignKey("Writing")]
        public int WritingId { get; set; }

        [Required, NotNull]
        [ForeignKey("Folder")]
        public int FolderId { get; set; }

        public virtual Writing Writing { get; set; }

        public virtual Folder Folder { get; set; }
    }

    [PrimaryKey(nameof(WritingId), nameof(FormatId))]
    public class WritingFormat
    {
        [Required, NotNull]
        [ForeignKey("Writing")]
        public int WritingId { get; set; }

        [Required, NotNull]
        [ForeignKey("FormatTag")]
        public int FormatId { get; set; }

        public virtual Writing? Writing { get; set; }

        public virtual FormatTag? FormatTag { get; set; }
    }

    [PrimaryKey(nameof(WritingId), nameof(GenreId))]
    public class WritingGenre
    {
        [Required, NotNull]
        [ForeignKey("Writing")]
        public int WritingId { get; set; }

        [Required, NotNull]
        [ForeignKey("GenreTag")]
        public int GenreId { get; set; }

        public virtual Writing? Writing { get; set; }

        public virtual GenreTag? GenreTag { get; set; }
    }

    [PrimaryKey(nameof(WritingId), nameof(ProfileId))]
    public class WritingProfile
    {
        [Required, NotNull]
        [ForeignKey("Writing")]
        public int WritingId { get; set; }

        [Required, NotNull]
        [ForeignKey("PenProfile")]
        public int ProfileId { get; set; }

        public virtual PenProfile? PenProfile { get; set; }

        public virtual Writing? Writing { get; set; }
    }

    //[PrimaryKey(nameof(WritingId), nameof(SeriesId))]
    public class WritingSeries
    {
        [Key]
        [Required, NotNull]
        public int WritingSeriesId { get; set; }

        [Required, NotNull]
        [ForeignKey("Series")]
        public int SeriesId { get; set; }

        [Required, NotNull]
        public int WritingId { get; set; }

        [AllowNull]
        public int? PreviousWritingId { get; set; }

        [AllowNull]
        public int? NextWritingId { get; set; }

        public virtual Series Series { get; set; }

        [ForeignKey("WritingId")]
        public virtual Writing Writing { get; set; }

        [ForeignKey(nameof(PreviousWritingId))]
        [AllowNull]
        public virtual Writing PreviousWriting { get; set; }

        [ForeignKey(nameof(NextWritingId))]
        [AllowNull]
        public virtual Writing NextWriting { get; set; }
    }
}
