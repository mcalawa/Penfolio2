using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Penfolio2.Models
{
    public class PenProfile
    {
        public PenProfile()
        {
            ProfileWritings = new HashSet<WritingProfile>();
            OwnedFolders = new HashSet<FolderOwner>();
            OwnedSeries = new HashSet<SeriesOwner>();
            CritiqueGiven = new HashSet<Critique>();
            Friends = new HashSet<Friendship>();
            FriendRequestsSent = new HashSet<FriendRequest>();
            FriendRequestsReceived = new HashSet<FriendRequest>();
            PendingAccessRequests = new HashSet<AccessRequest>();
            CommentsReceived = new HashSet<Comment>();
            CommentsMade = new HashSet<Comment>();
            CommentsFlagged = new HashSet<CommentFlag>();
            LikesMade = new HashSet<Like>();
            LikesReceived = new HashSet<Like>();
            Following = new HashSet<FollowerFollowing>();
            Followers = new HashSet<FollowerFollowing>();
            PublisherWriters = new HashSet<PublisherWriter>();
            WriterPublishers = new HashSet<PublisherWriter>();
            RepresentationRequestsReceived = new HashSet<RepresentationRequest>();
            RepresentationRequestsSent = new HashSet<RepresentationRequest>();
        }

        [Required]
        [Key]
        public int ProfileId { get; set; }

        [Required, NotNull]
        [ForeignKey("PenUser")]
        public string UserId { get; set; } = string.Empty;

        [Required, NotNull]
        public string DisplayName { get; set; } = string.Empty;

        [Required, NotNull]
        [ForeignKey("PenRole")]
        public int RoleId { get; set; }

        [Required, NotNull]
        [ForeignKey("AccessPermission")]
        public int AccessPermissionId { get; set; }

        [Required, NotNull]
        public bool UseSecondaryRoleName { get; set; } = false;

        [Required, NotNull]
        [StringLength(900)]
        public string UrlString { get; set; } = string.Empty;

        [Required, NotNull]
        public bool IsMainProfile { get; set; } = false;

        public string? ProfileDescription { get; set; }

        public byte[] ProfileImage { get; set; }

        [Required, NotNull]
        public bool Verified { get; set; } = false;

        //[ForeignKey("UserId")]
        public virtual PenUser? PenUser { get; set; }

        public virtual PenRole? PenRole { get; set; }

        public virtual AccessPermission? AccessPermission { get; set; }

        public virtual ICollection<WritingProfile> ProfileWritings { get; set; }

        //[ForeignKey("OwnerId")]
        public virtual ICollection<FolderOwner> OwnedFolders { get; set; }

        //[ForeignKey("OwnerId")]
        public virtual ICollection<SeriesOwner> OwnedSeries { get; set; }

        //[ForeignKey("CriticId")]
        public virtual ICollection<Critique> CritiqueGiven { get; set; }

        //[ForeignKey("CriticId")]
        public virtual CritiqueGiver? CritiqueNotificationSettings { get; set; }

        [ForeignKey("FirstFriendId")]
        public virtual ICollection<Friendship> Friends { get; set; }

        [ForeignKey("RequesterId")]
        public virtual ICollection<FriendRequest> FriendRequestsSent { get; set; }

        [ForeignKey("RequesteeId")]
        public virtual ICollection<FriendRequest> FriendRequestsReceived { get; set; }

        [ForeignKey("RequesterId")]
        public virtual ICollection<AccessRequest> PendingAccessRequests { get; set; }

        //[ForeignKey("ProfileId")]
        public virtual ICollection<Comment> CommentsReceived { get; set; }

        [ForeignKey("CommenterId")]
        public virtual ICollection<Comment> CommentsMade { get; set; }

        [ForeignKey("FlaggerId")]
        public virtual ICollection<CommentFlag> CommentsFlagged { get; set; }

        [ForeignKey("LikerId")]
        public virtual ICollection<Like> LikesMade { get; set; }

        [ForeignKey("ProfileId")]
        public virtual ICollection<Like> LikesReceived { get; set; }

        [ForeignKey("FollowerId")]
        public virtual ICollection<FollowerFollowing> Following { get; set; }

        [ForeignKey("ProfileId")]
        public virtual ICollection<FollowerFollowing> Followers { get; set; }

        [ForeignKey("WriterId")]
        public virtual ICollection<PublisherWriter> WriterPublishers { get; set; }

        [ForeignKey("PublisherId")]
        public virtual ICollection<PublisherWriter> PublisherWriters { get; set; }

        [ForeignKey("RequesterId")]
        public virtual ICollection<RepresentationRequest> RepresentationRequestsSent { get; set; }

        [ForeignKey("RequesteeId")]
        public virtual ICollection<RepresentationRequest> RepresentationRequestsReceived { get; set; }
    }
}
