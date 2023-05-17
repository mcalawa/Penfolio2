using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Penfolio2.Models
{
    public class Friendship
    {
        [Required]
        [Key]
        public int FriendshipId { get; set; }

        [Required, NotNull]
        public int FirstFriendId { get; set; }

        [Required, NotNull]
        public int SecondFriendId { get; set; }

        [Required, NotNull]
        public DateTime AcceptDate { get; set; }

        [Required, NotNull]
        public bool Active { get; set; }

        public int? OtherFriendshipId { get; set; }

        [ForeignKey("FirstFriendId")]
        public virtual PenProfile? FirstFriend { get; set; }

        [ForeignKey("SecondFriendId")]
        public virtual PenProfile? SecondFriend { get; set; }
    }

    public class FriendRequest
    {
        [Required]
        [Key]
        public int FriendRequestId { get; set; }

        [Required, NotNull]
        public int RequesterId { get; set; }

        [Required, NotNull]
        public int RequesteeId { get; set; }

        [Required, NotNull]
        public DateTime RequestDate { get; set; }

        [Required, NotNull]
        public bool Resolved { get; set; }

        [ForeignKey("RequesterId")]
        public virtual PenProfile? Requester { get; set; }

        [ForeignKey("RequesteeId")]
        public virtual PenProfile? Requestee { get; set; }
    }

    public class UserBlock
    {
        [Required]
        [Key]
        public int UserBlockId { get; set; }

        [Required, NotNull]
        public string BlockingUserId { get; set; }

        [Required, NotNull]
        public string BlockedUserId { get; set; }

        [ForeignKey("PenProfile")]
        [AllowNull]
        public int? BlockedAsProfileId { get; set; }

        [Required, NotNull]
        public bool CanStillSeeUser { get; set; } = false;

        [Required, NotNull]
        public bool CanSeeOtherProfilesForUser { get; set; } = false;

        [Required, NotNull]
        public DateTime BlockDate { get; set; }

        [Required, NotNull]
        public bool Active { get; set; } = true;

        [ForeignKey("BlockingUserId")]
        public virtual PenUser? BlockingUser { get; set; }

        [ForeignKey("BlockedUserId")]
        public virtual PenUser? BlockedUser { get; set; }
    }

    public class PublisherWriter
    {
        [Required, Key]
        public int PublisherWriterId { get; set; }

        [Required, NotNull]
        public int PublisherId { get; set; }

        [Required, NotNull]
        public int WriterId { get; set; }

        [Required, NotNull]
        public DateTime AcceptDate { get; set; }

        [Required, NotNull]
        public bool Active { get; set; }

        [ForeignKey("PublisherId")]
        public virtual PenProfile? Publisher { get; set; }

        [ForeignKey("WriterId")]
        public virtual PenProfile? Writer { get; set; }
    }
}
