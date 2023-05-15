using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Penfolio2.Models;

namespace Penfolio2.Data
{
    public class ApplicationDbContext : IdentityDbContext<PenUser>
    {
        public ApplicationDbContext()
        {
            var builder = WebApplication.CreateBuilder();

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            this.Database.SetConnectionString(connectionString);
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            var builder = WebApplication.CreateBuilder();

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            this.Database.SetConnectionString(connectionString);
        }

        public virtual DbSet<PenUser> PenUsers { get; set; }
        public virtual DbSet<PenProfile> PenProfiles { get; set; }
        public virtual DbSet<PenRole> ProfileRoles { get; set; }
        public virtual DbSet<AccessPermission> AccessPermissions { get; set; }
        public virtual DbSet<AccessRequest> AccessRequests { get; set; }
        public virtual DbSet<IndividualAccessGrant> IndividualAccessGrants { get; set; }
        public virtual DbSet<IndividualAccessRevoke> IndividualAccessRevokes { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<CommentFlag> CommentFlags { get; set; }
        public virtual DbSet<CommentReply> CommentReplies { get; set; }
        public virtual DbSet<Critique> Critiques { get; set; }
        public virtual DbSet<CritiqueGiver> CritiqueGivers { get; set; }
        public virtual DbSet<CritiqueRequest> CritiqueRequests { get; set; }
        public virtual DbSet<Folder> Folders { get; set; }
        public virtual DbSet<FormatTag> FormatTags { get; set; }
        public virtual DbSet<AltFormatName> AltFormatNames { get; set; }
        public virtual DbSet<FormatCategory> FormatCategories { get; set; }
        public virtual DbSet<GenreTag> GenreTags { get; set; }
        public virtual DbSet<AltGenreName> AltGenreNames { get; set; }
        public virtual DbSet<GenreCategory> GenreCategories { get; set; }
        public virtual DbSet<GenreFormat> GenreFormats { get; set; }
        public virtual DbSet<Like> Likes { get; set; }
        public virtual DbSet<Friendship> Friendships { get; set; }
        public virtual DbSet<FriendRequest> FriendRequests { get; set; }
        public virtual DbSet<UserBlock> UserBlocks { get; set; }
        public virtual DbSet<Series> Series { get; set; }
        public virtual DbSet<Writing> Writings { get; set; }
        public virtual DbSet<FolderFormat> FolderFormats { get; set; }
        public virtual DbSet<FolderGenre> FolderGenres { get; set; }
        public virtual DbSet<FolderOwner> FolderOwners { get; set; }
        public virtual DbSet<FolderSubfolder> FolderSubfolders { get; set; }
        public virtual DbSet<FollowerFollowing> FollowersFollowing { get; set; }
        public virtual DbSet<SeriesFormat> SeriesFormats { get; set; }
        public virtual DbSet<SeriesGenre> SeriesGenres { get; set; }
        public virtual DbSet<SeriesOwner> SeriesOwners { get; set; }
        public virtual DbSet<SeriesSeries> SeriesSeries { get; set; }
        public virtual DbSet<WritingFolder> WritingFolders { get; set; }
        public virtual DbSet<WritingFormat> WritingFormats { get; set; }
        public virtual DbSet<WritingGenre> WritingGenres { get; set; }
        public virtual DbSet<WritingProfile> WritingProfiles { get; set; }
        public virtual DbSet<WritingSeries> WritingSeries { get; set; }
        public virtual DbSet<PublisherWriter> PublisherWriters { get; set; }
        public virtual DbSet<RepresentationRequest> RepresentationRequests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }

            //optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Entity<PenUser>(entity =>
            {
                entity.ToTable(name: "User");

                entity.HasKey(x => x.Id);

                entity.HasMany(p => p.PenProfiles)
                     ?.WithOne(d => d.PenUser)
                     ?.HasForeignKey(p => p.UserId);

                entity?.HasMany(p => p.BlockedUsers)
                      ?.WithOne(d => d.BlockingUser)
                      ?.HasForeignKey(d => d.BlockingUserId)
                      ?.OnDelete(DeleteBehavior.Restrict);

                entity?.HasMany(p => p.UsersBlockedBy)
                      ?.WithOne(d => d.BlockedUser)
                      ?.HasForeignKey(d => d.BlockedUserId)
                      ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });
            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });
            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
            modelBuilder.Entity<AccessPermission>(entity =>
            {
                entity.ToTable("AccessPermission");

                entity.HasKey(x => x.AccessPermissionId);

                entity.HasMany(p => p.IndividualAccessGrants)
                     ?.WithOne(d => d.AccessPermission)
                     ?.HasForeignKey(d => d.AccessPermissionId);

                entity.HasMany(p => p.IndividualAccessRevokes)
                     ?.WithOne(d => d.AccessPermission)
                     ?.HasForeignKey(d => d.AccessPermissionId);

                entity.HasMany(p => p.AccessRequests)
                     ?.WithOne(d => d.AccessPermission)
                     ?.HasForeignKey(d => d.AccessPermissionId);

            });
            modelBuilder.Entity<PenRole>(entity =>
            {
                entity.ToTable("PenRole");

                entity.HasKey(x => x.RoleId);

                entity.HasMany(p => p.RoleProfiles)
                     ?.WithOne(d => d.PenRole)
                     ?.HasForeignKey(p => p.RoleId)
                     ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<PenProfile>(entity =>
            {
                entity.ToTable("Profile");

                entity.HasKey(x => x.ProfileId);

                entity.HasOne(p => p.AccessPermission)
                     ?.WithMany()
                     ?.HasForeignKey(p => p.AccessPermissionId)
                     ?.OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.ProfileWritings)
                     ?.WithOne(d => d.PenProfile)
                     ?.HasForeignKey(d => d.ProfileId)
                     ?.OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.OwnedFolders)
                     ?.WithOne(d => d.Owner)
                     ?.HasForeignKey(d => d.OwnerId);

                entity.HasMany(p => p.OwnedSeries)
                     ?.WithOne(d => d.Owner)
                     ?.HasForeignKey(d => d.OwnerId);

                entity.HasMany(p => p.CritiqueGiven)
                     ?.WithOne(d => d.Critic)
                     ?.HasForeignKey(d => d.CriticId);

                entity?.HasMany(p => p.Followers)
                      ?.WithOne(d => d.FollowingProfile)
                      ?.HasForeignKey(d => d.ProfileId)
                      ?.OnDelete(DeleteBehavior.Restrict);

                entity?.HasMany(p => p.Following)
                      ?.WithOne(d => d.Follower)
                      ?.HasForeignKey(d => d.FollowerId)
                      ?.OnDelete(DeleteBehavior.Restrict);

                entity?.HasMany(p => p.FriendRequestsReceived)
                      ?.WithOne(d => d.Requestee)
                      ?.HasForeignKey(d => d.RequesteeId);

                entity?.HasMany(p => p.FriendRequestsSent)
                      ?.WithOne(d => d.Requester)
                      ?.HasForeignKey(d => d.RequesterId);

                entity?.HasMany(p => p.Friends)
                      ?.WithOne(d => d.FirstFriend)
                      ?.HasForeignKey(d => d.FirstFriendId);

                entity?.HasMany(p => p.PendingAccessRequests)
                      ?.WithOne(d => d.Requester)
                      ?.HasForeignKey(d => d.RequesterId)
                      ?.OnDelete(DeleteBehavior.Restrict);

                entity?.HasMany(p => p.CommentsMade)
                      ?.WithOne(d => d.Commenter)
                      ?.HasForeignKey(d => d.CommenterId)
                      ?.OnDelete(DeleteBehavior.Restrict);

                entity?.HasMany(p => p.CommentsReceived)
                      ?.WithOne(d => d.CommentProfile)
                      ?.HasForeignKey(d => d.ProfileId);

                entity?.HasMany(p => p.CommentsFlagged)
                      ?.WithOne(d => d.Flagger)
                      ?.HasForeignKey(d => d.FlaggerId);

                entity?.HasMany(p => p.LikesReceived)
                      ?.WithOne(d => d.LikedProfile)
                      ?.HasForeignKey(d => d.ProfileId)
                      ?.OnDelete(DeleteBehavior.Restrict);

                entity?.HasMany(p => p.LikesMade)
                      ?.WithOne(d => d.Liker)
                      ?.HasForeignKey(d => d.LikerId)
                      ?.OnDelete(DeleteBehavior.Restrict);

                entity?.HasMany(p => p.PublisherWriters)
                      ?.WithOne(d => d.Publisher)
                      ?.HasForeignKey(d => d.PublisherId)
                      ?.OnDelete(DeleteBehavior.Restrict);

                entity?.HasMany(p => p.WriterPublishers)
                      ?.WithOne(d => d.Writer)
                      ?.HasForeignKey(d => d.WriterId)
                      ?.OnDelete(DeleteBehavior.Restrict);

                entity?.HasMany(p => p.RepresentationRequestsReceived)
                      ?.WithOne(d => d.Requestee)
                      ?.HasForeignKey(d => d.RequesteeId)
                      ?.OnDelete(DeleteBehavior.Restrict);

                entity?.HasMany(p => p.RepresentationRequestsSent)
                      ?.WithOne(d => d.Requester)
                      ?.HasForeignKey(d => d.RequesterId)
                      ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<UserBlock>(entity =>
            {
                entity.ToTable("UserBlock");

                entity.HasKey(x => x.UserBlockId);
            });
            modelBuilder.Entity<AccessRequest>(entity =>
            {
                entity.ToTable("AccessRequest");

                entity.HasKey(x => x.AccessRequestId);

                entity.HasOne(p => p.Requester)
                     ?.WithMany()
                     ?.HasForeignKey(p => p.RequesterId)
                     ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<IndividualAccessGrant>(entity =>
            {
                entity.ToTable("IndividualAccessGrant");

                entity.HasKey(x => x.IndividualAccessGrantId);

                entity.HasOne(p => p.Grantee)
                     ?.WithMany()
                     ?.HasForeignKey(p => p.GranteeId)
                     ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<IndividualAccessRevoke>(entity =>
            {
                entity.ToTable("IndividualAccessRevoke");

                entity.HasKey(x => x.IndividualAccessRevokeId);

                entity.HasOne(p => p.Revokee)
                     ?.WithMany()
                     ?.HasForeignKey(p => p.RevokeeId)
                     ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Friendship>(entity =>
            {
                entity.ToTable("Friendship");

                entity.HasKey(x => x.FriendshipId);

                entity.HasOne(p => p.SecondFriend)
                     ?.WithMany()
                     ?.HasForeignKey(p => p.SecondFriendId)
                     ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<FriendRequest>(entity =>
            {
                entity.ToTable("FriendRequest");

                entity.HasKey(x => x.FriendRequestId);
            });
            modelBuilder.Entity<Writing>(entity =>
            {
                entity.ToTable("Writing");

                entity.HasKey(x => x.WritingId);

                entity.HasMany(p => p.WritingGenres)
                     ?.WithOne(d => d.Writing)
                     ?.HasForeignKey(d => d.WritingId);

                entity.HasMany(p => p.WritingSeries)
                     ?.WithOne(d => d.Writing)
                     ?.HasForeignKey(d => d.WritingId);

                entity.HasMany(p => p.WritingFormats)
                     ?.WithOne(d => d.Writing)
                     ?.HasForeignKey(d => d.WritingId);

                entity.HasMany(p => p.WritingProfiles)
                     ?.WithOne(d => d.Writing)
                     ?.HasForeignKey(d => d.WritingId);

                entity.HasMany(p => p.WritingFolders)
                     ?.WithOne(d => d.Writing)
                     ?.HasForeignKey(d => d.WritingId)
                     ?.OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Comments)
                     ?.WithOne(d => d.Writing)
                     ?.HasForeignKey(d => d.WritingId)
                     ?.OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Likes)
                     ?.WithOne(d => d.LikedWriting)
                     ?.HasForeignKey(d => d.WritingId)
                     ?.OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Critiques)
                     ?.WithOne(d => d.Writing)
                     ?.HasForeignKey(d => d.WritingId)
                     ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<WritingProfile>(entity =>
            {
                entity.ToTable("WritingProfile");

                entity.HasKey(x => new { x.WritingId, x.ProfileId });
            });
            modelBuilder.Entity<Critique>(entity =>
            {
                entity.ToTable("Critique");

                entity.HasKey(x => x.CritiqueId);
            });
            modelBuilder.Entity<CritiqueRequest>(entity =>
            {
                entity.ToTable("CritiqueRequest");

                entity.HasKey(x => x.CritiqueRequestId);

                entity.HasOne(p => p.Writing)
                     ?.WithMany()
                     ?.HasForeignKey(p => p.WritingId)
                     ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Folder>(entity =>
            {
                entity.ToTable("Folder");

                entity.HasKey(x => x.FolderId);

                entity.HasMany(p => p.Subfolders)
                      ?.WithOne(d => d.Folder)
                      ?.HasForeignKey(d => d.FolderId);

                entity.HasMany(p => p.Owners)
                     ?.WithOne(d => d.Folder)
                     ?.HasForeignKey(d => d.FolderId);

                entity.HasMany(p => p.Likes)
                     ?.WithOne(d => d.LikedFolder)
                     ?.HasForeignKey(d => d.FolderId)
                     ?.OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Followers)
                     ?.WithOne(d => d.Folder)
                     ?.HasForeignKey(d => d.FolderId);

                entity.HasMany(p => p.Formats)
                     ?.WithOne(d => d.Folder)
                     ?.HasForeignKey(d => d.FolderId);

                entity.HasMany(p => p.Genres)
                     ?.WithOne(d => d.Folder)
                     ?.HasForeignKey(d => d.FolderId);
            });
            modelBuilder.Entity<WritingFolder>(entity =>
            {
                entity.ToTable("WritingFolder");

                entity.HasKey(x => new { x.WritingId, x.FolderId });
            });
            modelBuilder.Entity<FolderSubfolder>(entity =>
            {
                entity.ToTable("FolderSubfolder");

                entity.HasKey(x => new { x.FolderId, x.SubfolderId });

                entity.HasOne(d => d.Folder)
                      .WithMany(p => p.Subfolders)
                      .HasForeignKey(d => d.FolderId);

                entity.HasOne(p => p.Subfolder)
                     ?.WithMany()
                     ?.HasForeignKey(p => p.SubfolderId)
                     ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<FolderOwner>(entity =>
            {
                entity.ToTable("FolderOwner");

                entity.HasKey(x => new { x.FolderId, x.OwnerId });

                entity.HasOne(p => p.Owner)
                     ?.WithMany()
                     ?.HasForeignKey(p => p.OwnerId)
                     ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Series>(entity =>
            {
                entity.ToTable("Series");

                entity.HasKey(x => x.SeriesId);

                entity.HasMany(p => p.ParentSeries)
                     ?.WithOne(d => d.SeriesMember)
                     ?.HasForeignKey(d => d.SeriesMemberId)
                     ?.OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Subseries)
                     ?.WithOne(d => d.OverarchingSeries)
                     ?.HasForeignKey(d => d.OverarchingSeriesId);

                entity.HasMany(p => p.SeriesWritings)
                     ?.WithOne(d => d.Series)
                     ?.HasForeignKey(d => d.SeriesId);

                entity.HasMany(p => p.Comments)
                     ?.WithOne(d => d.Series)
                     ?.HasForeignKey(d => d.SeriesId)
                     ?.OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Likes)
                     ?.WithOne(d => d.LikedSeries)
                     ?.HasForeignKey(d => d.SeriesId)
                     ?.OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Owners)
                     ?.WithOne(d => d.Series)
                     ?.HasForeignKey(d => d.SeriesId)
                     ?.OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Followers)
                     ?.WithOne(d => d.Series)
                     ?.HasForeignKey(d => d.SeriesId)
                     ?.OnDelete(DeleteBehavior.Restrict);

                entity?.HasOne(typeof(Series), "NextSeries")
                      ?.WithMany()
                      ?.HasForeignKey("PreviousSeriesId")
                      ?.OnDelete(DeleteBehavior.Restrict);

                entity?.HasOne(typeof(Series), "PreviousSeries")
                      ?.WithMany()
                      ?.HasForeignKey("NextSeriesId")
                      ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<WritingSeries>(entity =>
            {
                entity.ToTable("WritingSeries");

                entity.HasKey(x => x.WritingSeriesId);

                entity?.HasOne(p => p.NextWriting)
                      ?.WithMany()
                      ?.HasForeignKey(p => p.NextWritingId)
                      ?.OnDelete(DeleteBehavior.Restrict);

                entity?.HasOne(p => p.PreviousWriting)
                      ?.WithMany()
                      ?.HasForeignKey(p => p.PreviousWritingId)
                      ?.OnDelete(DeleteBehavior.Restrict);

                entity?.HasOne(p => p.Writing)
                      ?.WithMany()
                      ?.HasForeignKey(p => p.WritingId)
                      ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<SeriesSeries>(entity =>
            {
                entity.ToTable("SeriesSeries");

                entity.HasKey(x => new { x.OverarchingSeriesId, x.SeriesMemberId });
            });
            modelBuilder.Entity<SeriesOwner>(entity =>
            {
                entity.ToTable("SeriesOwner");

                entity.HasKey(x => new { x.SeriesId, x.OwnerId });
            });
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comments");

                entity.HasKey(x => x.CommentId);

                entity.HasMany(p => p.CommentReplies)
                     ?.WithOne(d => d.Comment)
                     ?.HasForeignKey(d => d.CommentId)
                     ?.OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Likes)
                     ?.WithOne(d => d.LikedComment)
                     ?.HasForeignKey(d => d.CommentId);

                entity.HasMany(p => p.CommentFlags)
                     ?.WithOne(d => d.Comment)
                     ?.HasForeignKey(d => d.CommentId)
                     ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<CommentFlag>(entity =>
            {
                entity.ToTable("CommentFlag");

                entity.HasKey(x => x.CommentFlagId);
            });
            modelBuilder.Entity<Like>(entity =>
            {
                entity.ToTable("Likes");

                entity.HasKey(x => x.LikeId);
            });
            modelBuilder.Entity<CommentReply>(entity =>
            {
                entity.ToTable("CommentReply");

                entity.HasKey(x => new { x.CommentId, x.ReplyId });

                entity.HasOne(p => p.Comment)
                      .WithMany(d => d.CommentReplies)
                      .HasForeignKey(p => p.CommentId);

                entity?.HasOne(p => p.Reply)
                      ?.WithMany()
                      ?.HasForeignKey(p => p.ReplyId)
                      ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<FormatTag>(entity =>
            {
                entity.ToTable("FormatTag");

                entity.HasKey(x => x.FormatId);

                entity.HasMany(p => p.ParentFormats)
                     ?.WithOne(d => d.FormatTag)
                     ?.HasForeignKey(d => d.FormatId);

                entity.HasMany(p => p.ChildFormats)
                     ?.WithOne(d => d.ParentFormat)
                     ?.HasForeignKey(d => d.ParentId)
                     ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<AltFormatName>(entity =>
            {
                entity.ToTable("AltFormatName");

                entity.HasKey(x => x.AltFormatNameId);
            });
            modelBuilder.Entity<FormatCategory>(entity =>
            {
                entity.ToTable("FormatCategory");

                entity.HasKey(x => x.FormatCategoryId);

                entity?.HasOne(p => p.SecondaryParentFormat)
                      ?.WithMany()
                      ?.HasForeignKey(p => p.SecondaryParentId)
                      ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<WritingFormat>(entity =>
            {
                entity.ToTable("WritingFormat");

                entity.HasKey(x => new { x.WritingId, x.FormatId });
            });
            modelBuilder.Entity<FolderFormat>(entity =>
            {
                entity.ToTable("FolderFormat");

                entity.HasKey(x => new { x.FolderId, x.FormatId });
            });
            modelBuilder.Entity<SeriesFormat>(entity =>
            {
                entity.ToTable("SeriesFormat");

                entity.HasKey(x => new { x.SeriesId, x.FormatId });
            });
            modelBuilder.Entity<GenreTag>(entity =>
            {
                entity.ToTable("GenreTag");

                entity.HasKey(x => x.GenreId);

                entity.HasMany(p => p.ParentGenres)
                     ?.WithOne(d => d.GenreTag)
                     ?.HasForeignKey(d => d.GenreId);

                entity.HasMany(p => p.ChildGenres)
                     ?.WithOne(d => d.ParentGenre)
                     ?.HasForeignKey(d => d.ParentId)
                     ?.OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.GenreFormats)
                     ?.WithOne(d => d.GenreTag)
                     ?.HasForeignKey(d => d.GenreId);
            });
            modelBuilder.Entity<AltGenreName>(entity =>
            {
                entity.ToTable("AltGenreName");
            });
            modelBuilder.Entity<GenreCategory>(entity =>
            {
                entity.ToTable("GenreCategory");

                entity?.HasOne(p => p.SecondaryParentGenre)
                      ?.WithMany()
                      ?.HasForeignKey(p => p.SecondaryParentId)
                      ?.OnDelete(DeleteBehavior.Restrict);

                entity?.HasOne(p => p.TertiaryParentGenre)
                      ?.WithMany()
                      ?.HasForeignKey(p => p.TertiaryParentId)
                      ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<GenreFormat>(entity =>
            {
                entity.ToTable("GenreFormat");

                entity?.HasOne(p => p.ParentGenreTag)
                      ?.WithMany()
                      ?.HasForeignKey(p => p.ParentGenreId)
                      ?.OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<WritingGenre>(entity =>
            {
                entity.ToTable("WritingGenre");

                entity.HasKey(x => new { x.GenreId, x.WritingId });
            });
            modelBuilder.Entity<FolderGenre>(entity =>
            {
                entity.ToTable("FolderGenre");

                entity.HasKey(x => new { x.GenreId, x.FolderId });
            });
            modelBuilder.Entity<SeriesGenre>(entity =>
            {
                entity.ToTable("SeriesGenre");

                entity.HasKey(x => new { x.SeriesId, x.GenreId });
            });
            modelBuilder.Entity<FollowerFollowing>(entity =>
            {
                entity.ToTable("FollowerFollowing");

                entity.HasKey(x => x.FollowerFollowingId);
            });
            modelBuilder.Entity<CritiqueGiver>(entity =>
            {
                entity.ToTable("CritiqueGiver");

                entity.HasKey(x => x.CritiqueGiverId);
            });
            modelBuilder.Entity<PublisherWriter>(entity =>
            {
                entity.ToTable("PublisherWriter");

                entity.HasKey(x => x.PublisherWriterId);
            });
            modelBuilder.Entity<RepresentationRequest>(entity =>
            {
                entity.ToTable("RepresentationRequest");

                entity.HasKey(x => x.RepresentationRequestId);
            });
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}