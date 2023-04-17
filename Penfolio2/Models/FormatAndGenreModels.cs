using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Penfolio2.Models
{
    public class FormatTag
    {
        public FormatTag()
        {
            AltFormatNames = new HashSet<AltFormatName>();
            ChildFormats = new HashSet<FormatCategory>();
            ParentFormats = new HashSet<FormatCategory>();
            ChildGenres = new HashSet<GenreFormat>();
            FormatWritings = new HashSet<WritingFormat>();
            FormatFolders = new HashSet<FolderFormat>();
            Followers = new HashSet<FollowerFollowing>();
        }

        [Required]
        [Key]
        public int FormatId { get; set; }

        [Required, NotNull]
        public string FormatName { get; set; }

        [AllowNull]
        public string? Explanation { get; set; }

        [Required, NotNull]
        public bool IsFictionOnly { get; set; } = false;

        [Required, NotNull]
        public bool IsNonfictionOnly { get; set; } = false;

        public virtual ICollection<AltFormatName> AltFormatNames { get; set; }

        [ForeignKey("ParentId")]
        public virtual ICollection<FormatCategory> ChildFormats { get; set; }

        [ForeignKey("FormatId")]
        public virtual ICollection<FormatCategory> ParentFormats { get; set; }

        [ForeignKey("ParentFormatId")]
        public virtual ICollection<GenreFormat> ChildGenres { get; set; }

        public virtual ICollection<WritingFormat> FormatWritings { get; set; }

        public virtual ICollection<FolderFormat> FormatFolders { get; set; }

        public virtual ICollection<FollowerFollowing> Followers { get; set; }
    }

    public class AltFormatName
    {
        [Required]
        [Key]
        public int AltFormatNameId { get; set; }

        [Required, NotNull]
        [ForeignKey("FormatTag")]
        public int FormatId { get; set; }

        [Required, NotNull]
        public string AltName { get; set; }

        public virtual FormatTag FormatTag { get; set; }
    }

    public class FormatCategory
    {
        [Required]
        [Key]
        public int FormatCategoryId { get; set; }

        [Required, NotNull]
        public int FormatId { get; set; }

        [Required, NotNull]
        public int ParentId { get; set; }

        [AllowNull]
        public int? SecondaryParentId { get; set; }

        [ForeignKey("FormatId")]
        public virtual FormatTag FormatTag { get; set; }

        [ForeignKey("ParentId")]
        public virtual FormatTag ParentFormat { get; set; }

        [ForeignKey("SecondaryParentId")]
        [AllowNull]
        public virtual FormatTag SecondaryParentFormat { get; set; }
    }

    public class GenreTag
    {
        public GenreTag()
        {
            AltGenreNames = new HashSet<AltGenreName>();
            ChildGenres = new HashSet<GenreCategory>();
            ParentGenres = new HashSet<GenreCategory>();
            GenreFormats = new HashSet<GenreFormat>();
            GenreWritings = new HashSet<WritingGenre>();
            GenreFolders = new HashSet<FolderGenre>();
            Followers = new HashSet<FollowerFollowing>();
        }

        [Required]
        [Key]
        public int GenreId { get; set; }

        [Required, NotNull]
        public string GenreName { get; set; }

        [AllowNull]
        public string? Explanation { get; set; }

        [Required, NotNull]
        public bool IsFictionOnly { get; set; }

        [Required, NotNull]
        public bool IsNonfictionOnly { get; set; }

        public virtual ICollection<AltGenreName> AltGenreNames { get; set; }

        [ForeignKey("ParentId")]
        public virtual ICollection<GenreCategory> ChildGenres { get; set; }

        [ForeignKey("GenreId")]
        public virtual ICollection<GenreCategory> ParentGenres { get; set; }

        [ForeignKey("GenreId")]
        public virtual ICollection<GenreFormat> GenreFormats { get; set; }

        public virtual ICollection<WritingGenre> GenreWritings { get; set; }

        public virtual ICollection<FolderGenre> GenreFolders { get; set; }

        public virtual ICollection<FollowerFollowing> Followers { get; set; }
    }

    public class AltGenreName
    {
        [Required]
        [Key]
        public int AltGenreNameId { get; set; }

        [Required, NotNull]
        [ForeignKey("GenreTag")]
        public int GenreId { get; set; }

        [Required, NotNull]
        public string AltName { get; set; }

        public virtual GenreTag GenreTag { get; set; }
    }

    public class GenreCategory
    {
        [Required]
        [Key]
        public int GenreCategoryId { get; set; }

        [Required, NotNull]
        public int GenreId { get; set; }

        [Required, NotNull]
        public int ParentId { get; set; }

        [AllowNull]
        public int? SecondaryParentId { get; set; }

        [AllowNull]
        public int? TertiaryParentId { get; set; }

        [ForeignKey("GenreId")]
        public virtual GenreTag GenreTag { get; set; }

        [ForeignKey("ParentId")]
        public virtual GenreTag ParentGenre { get; set; }

        [ForeignKey("SecondaryParentId")]
        [AllowNull]
        public virtual GenreTag SecondaryParentGenre { get; set; }

        [ForeignKey("TertiaryParentId")]
        [AllowNull]
        public virtual GenreTag TertiaryParentGenre { get; set; }
    }

    public class GenreFormat
    {
        [Required]
        [Key]
        public int GenreFormatId { get; set; }

        [Required, NotNull]
        public int GenreId { get; set; }

        [Required, NotNull]
        [ForeignKey("FormatTag")]
        public int ParentFormatId { get; set; }

        [AllowNull]
        public int? ParentGenreId { get; set; }

        [ForeignKey("GenreId")]
        public virtual GenreTag GenreTag { get; set; }

        public virtual FormatTag ParentFormatTag { get; set; }

        [ForeignKey("ParentGenreId")]
        [AllowNull]
        public virtual GenreTag ParentGenreTag { get; set; }
    }
}
