using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Penfolio2.Models
{
    public class AccessPermission
    {
        public AccessPermission()
        {
            IndividualAccessGrants = new HashSet<IndividualAccessGrant>();
            IndividualAccessRevokes = new HashSet<IndividualAccessRevoke>();
            AccessRequests = new HashSet<AccessRequest>();
        }

        [Required]
        [Key]
        public int AccessPermissionId { get; set; }

        [AllowNull]
        public int? ProfileId { get; set; }

        [AllowNull]
        public int? WritingId { get; set; }

        [AllowNull]
        public int? FolderId { get; set; }

        [AllowNull]
        public int? SeriesId { get; set; }

        [Required, NotNull]
        public bool PublicAccess { get; set; }

        [Required, NotNull]
        public bool FriendAccess { get; set; }

        [Required, NotNull]
        public bool PublisherAccess { get; set; }

        [Required, NotNull]
        public bool MyAgentAccess { get; set; } = true;

        [Required, NotNull]
        public bool MinorAccess { get; set; }

        [Required, NotNull]
        public bool ShowsUpInSearch { get; set; }

        public virtual ICollection<IndividualAccessGrant> IndividualAccessGrants { get; set; }

        public virtual ICollection<IndividualAccessRevoke> IndividualAccessRevokes { get; set; }

        public virtual ICollection<AccessRequest> AccessRequests { get; set; }
    }

    public class AccessRequest
    {
        [Required]
        [Key]
        public int AccessRequestId { get; set; }

        [Required, NotNull]
        [ForeignKey("AccessPermission")]
        public int AccessPermissionId { get; set; }

        [Required, NotNull]
        [ForeignKey("PenProfile")]
        public int RequesterId { get; set; }

        [Required, NotNull]
        public DateTime RequestDate { get; set; }

        [Required, NotNull]
        public bool Resolved { get; set; }

        public virtual AccessPermission? AccessPermission { get; set; }

        //[ForeignKey("RequesterId")]
        public virtual PenProfile? Requester { get; set; }
    }

    public class RepresentationRequest
    {
        [Required, Key]
        public int RepresentationRequestId { get; set; }

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

    public class IndividualAccessGrant
    {
        [Required]
        [Key]
        public int IndividualAccessGrantId { get; set; }

        [Required, NotNull]
        [ForeignKey("AccessPermission")]
        public int AccessPermissionId { get; set; }

        [Required, NotNull]
        [ForeignKey("PenProfile")]
        public int GranteeId { get; set; }

        [Required, NotNull]
        public DateTime GrantDate { get; set; }

        [Required, NotNull]
        public bool Active { get; set; }

        public virtual AccessPermission? AccessPermission { get; set; }

        public virtual PenProfile? Grantee { get; set; }
    }

    public class IndividualAccessRevoke
    {
        [Required]
        [Key]
        public int IndividualAccessRevokeId { get; set; }

        [Required, NotNull]
        [ForeignKey("AccessPermission")]
        public int AccessPermissionId { get; set; }

        [Required, NotNull]
        [ForeignKey("PenProfile")]
        public int RevokeeId { get; set; }

        [Required, NotNull]
        public DateTime RevokeDate { get; set; }

        [Required, NotNull]
        public bool Active { get; set; }

        public virtual AccessPermission? AccessPermission { get; set; }

        //[ForeignKey("RevokeeId")]
        public virtual PenProfile? Revokee { get; set; }
    }
}
