using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Penfolio2.Models
{
    public class Folder
    {
        public Folder()
        {
            Owners = new HashSet<FolderOwner>();
            Subfolders = new HashSet<FolderSubfolder>();
            Comments = new HashSet<Comment>();
            Likes = new HashSet<Like>();
            Formats = new HashSet<FolderFormat>();
            Genres = new HashSet<FolderGenre>();
            Followers = new HashSet<FollowerFollowing>();
        }

        [Required]
        [Key]
        public int FolderId { get; set; }

        [Required, NotNull]
        [ForeignKey("AccessPermission")]
        public int AccessPermissionId { get; set; }

        [Required, NotNull]
        [ForeignKey("PenUser")] 
        public string CreatorId { get; set; } = string.Empty;

        [Required, NotNull]
        public string FolderName { get; set; } = string.Empty;

        [AllowNull]
        public string? FolderDescription { get; set; }

        public virtual AccessPermission? AccessPermission { get; set; }

        public virtual PenUser? Creator { get; set; }

        public virtual ICollection<FolderOwner> Owners { get; set; }

        public virtual ICollection<FolderSubfolder> Subfolders { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Like> Likes { get; set; }

        public virtual ICollection<FolderFormat> Formats { get; set; }

        public virtual ICollection<FolderGenre> Genres { get; set; }

        public virtual ICollection<FollowerFollowing> Followers { get; set; }
    }
}
