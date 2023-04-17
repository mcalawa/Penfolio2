using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Penfolio2.Data.Migrations
{
    /// <inheritdoc />
    public partial class Updatedmodelsandsuch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims");

            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "UserTokens",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "UserRoles",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "UserLogins",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "UserClaims",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "Role",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "RoleClaims",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "dbo",
                table: "UserRoles",
                newName: "IX_UserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "dbo",
                table: "UserLogins",
                newName: "IX_UserLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "dbo",
                table: "UserClaims",
                newName: "IX_UserClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "dbo",
                table: "RoleClaims",
                newName: "IX_RoleClaims_RoleId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "UserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                schema: "dbo",
                table: "UserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                schema: "dbo",
                table: "UserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                schema: "dbo",
                table: "UserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTokens",
                schema: "dbo",
                table: "UserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                schema: "dbo",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLogins",
                schema: "dbo",
                table: "UserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClaims",
                schema: "dbo",
                table: "UserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                schema: "dbo",
                table: "Role",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleClaims",
                schema: "dbo",
                table: "RoleClaims",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AccessPermission",
                schema: "dbo",
                columns: table => new
                {
                    AccessPermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<int>(type: "int", nullable: true),
                    WritingId = table.Column<int>(type: "int", nullable: true),
                    FolderId = table.Column<int>(type: "int", nullable: true),
                    SeriesId = table.Column<int>(type: "int", nullable: true),
                    PublicAccess = table.Column<bool>(type: "bit", nullable: false),
                    FriendAccess = table.Column<bool>(type: "bit", nullable: false),
                    PublisherAccess = table.Column<bool>(type: "bit", nullable: false),
                    MinorAccess = table.Column<bool>(type: "bit", nullable: false),
                    ShowsUpInSearch = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPermission", x => x.AccessPermissionId);
                });

            migrationBuilder.CreateTable(
                name: "FormatTag",
                schema: "dbo",
                columns: table => new
                {
                    FormatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormatName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFictionOnly = table.Column<bool>(type: "bit", nullable: false),
                    IsNonfictionOnly = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormatTag", x => x.FormatId);
                });

            migrationBuilder.CreateTable(
                name: "GenreTag",
                schema: "dbo",
                columns: table => new
                {
                    GenreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenreName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFictionOnly = table.Column<bool>(type: "bit", nullable: false),
                    IsNonfictionOnly = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreTag", x => x.GenreId);
                });

            migrationBuilder.CreateTable(
                name: "PenRole",
                schema: "dbo",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondaryRoleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanPostWritings = table.Column<bool>(type: "bit", nullable: false),
                    CanRepresentWriters = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PenRole", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Birthdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UseLowDataMode = table.Column<bool>(type: "bit", nullable: false),
                    Strikes = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AltFormatName",
                schema: "dbo",
                columns: table => new
                {
                    AltFormatNameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormatId = table.Column<int>(type: "int", nullable: false),
                    AltName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AltFormatName", x => x.AltFormatNameId);
                    table.ForeignKey(
                        name: "FK_AltFormatName_FormatTag_FormatId",
                        column: x => x.FormatId,
                        principalSchema: "dbo",
                        principalTable: "FormatTag",
                        principalColumn: "FormatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormatCategory",
                schema: "dbo",
                columns: table => new
                {
                    FormatCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormatId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    SecondaryParentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormatCategory", x => x.FormatCategoryId);
                    table.ForeignKey(
                        name: "FK_FormatCategory_FormatTag_FormatId",
                        column: x => x.FormatId,
                        principalSchema: "dbo",
                        principalTable: "FormatTag",
                        principalColumn: "FormatId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormatCategory_FormatTag_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "dbo",
                        principalTable: "FormatTag",
                        principalColumn: "FormatId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormatCategory_FormatTag_SecondaryParentId",
                        column: x => x.SecondaryParentId,
                        principalSchema: "dbo",
                        principalTable: "FormatTag",
                        principalColumn: "FormatId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AltGenreName",
                schema: "dbo",
                columns: table => new
                {
                    AltGenreNameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    AltName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AltGenreName", x => x.AltGenreNameId);
                    table.ForeignKey(
                        name: "FK_AltGenreName_GenreTag_GenreId",
                        column: x => x.GenreId,
                        principalSchema: "dbo",
                        principalTable: "GenreTag",
                        principalColumn: "GenreId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreCategory",
                schema: "dbo",
                columns: table => new
                {
                    GenreCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    SecondaryParentId = table.Column<int>(type: "int", nullable: false),
                    TertiaryParentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreCategory", x => x.GenreCategoryId);
                    table.ForeignKey(
                        name: "FK_GenreCategory_GenreTag_GenreId",
                        column: x => x.GenreId,
                        principalSchema: "dbo",
                        principalTable: "GenreTag",
                        principalColumn: "GenreId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreCategory_GenreTag_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "dbo",
                        principalTable: "GenreTag",
                        principalColumn: "GenreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GenreCategory_GenreTag_SecondaryParentId",
                        column: x => x.SecondaryParentId,
                        principalSchema: "dbo",
                        principalTable: "GenreTag",
                        principalColumn: "GenreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GenreCategory_GenreTag_TertiaryParentId",
                        column: x => x.TertiaryParentId,
                        principalSchema: "dbo",
                        principalTable: "GenreTag",
                        principalColumn: "GenreId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GenreFormat",
                schema: "dbo",
                columns: table => new
                {
                    GenreFormatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    ParentFormatId = table.Column<int>(type: "int", nullable: false),
                    ParentGenreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreFormat", x => x.GenreFormatId);
                    table.ForeignKey(
                        name: "FK_GenreFormat_FormatTag_ParentFormatId",
                        column: x => x.ParentFormatId,
                        principalSchema: "dbo",
                        principalTable: "FormatTag",
                        principalColumn: "FormatId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreFormat_GenreTag_GenreId",
                        column: x => x.GenreId,
                        principalSchema: "dbo",
                        principalTable: "GenreTag",
                        principalColumn: "GenreId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreFormat_GenreTag_ParentGenreId",
                        column: x => x.ParentGenreId,
                        principalSchema: "dbo",
                        principalTable: "GenreTag",
                        principalColumn: "GenreId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Folder",
                schema: "dbo",
                columns: table => new
                {
                    FolderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessPermissionId = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FolderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FolderDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folder", x => x.FolderId);
                    table.ForeignKey(
                        name: "FK_Folder_AccessPermission_AccessPermissionId",
                        column: x => x.AccessPermissionId,
                        principalSchema: "dbo",
                        principalTable: "AccessPermission",
                        principalColumn: "AccessPermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Folder_User_CreatorId",
                        column: x => x.CreatorId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profile",
                schema: "dbo",
                columns: table => new
                {
                    ProfileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    AccessPermissionId = table.Column<int>(type: "int", nullable: false),
                    UseSecondaryRoleName = table.Column<bool>(type: "bit", nullable: false),
                    UrlString = table.Column<string>(type: "nvarchar(900)", maxLength: 900, nullable: false),
                    IsMainProfile = table.Column<bool>(type: "bit", nullable: false),
                    ProfileDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Verified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.ProfileId);
                    table.ForeignKey(
                        name: "FK_Profile_AccessPermission_AccessPermissionId",
                        column: x => x.AccessPermissionId,
                        principalSchema: "dbo",
                        principalTable: "AccessPermission",
                        principalColumn: "AccessPermissionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Profile_PenRole_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "PenRole",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Profile_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                schema: "dbo",
                columns: table => new
                {
                    SeriesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessPermissionId = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SeriesName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeriesDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    IsStandAlone = table.Column<bool>(type: "bit", nullable: false),
                    PreviousSeriesId = table.Column<int>(type: "int", nullable: false),
                    NextSeriesId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.SeriesId);
                    table.ForeignKey(
                        name: "FK_Series_AccessPermission_AccessPermissionId",
                        column: x => x.AccessPermissionId,
                        principalSchema: "dbo",
                        principalTable: "AccessPermission",
                        principalColumn: "AccessPermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Series_Series_NextSeriesId",
                        column: x => x.NextSeriesId,
                        principalSchema: "dbo",
                        principalTable: "Series",
                        principalColumn: "SeriesId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Series_Series_PreviousSeriesId",
                        column: x => x.PreviousSeriesId,
                        principalSchema: "dbo",
                        principalTable: "Series",
                        principalColumn: "SeriesId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Series_User_CreatorId",
                        column: x => x.CreatorId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBlock",
                schema: "dbo",
                columns: table => new
                {
                    UserBlockId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlockingUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BlockedUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BlockedAsProfileId = table.Column<int>(type: "int", nullable: true),
                    CanStillSeeUser = table.Column<bool>(type: "bit", nullable: false),
                    CanSeeOtherProfilesForUser = table.Column<bool>(type: "bit", nullable: false),
                    BlockDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBlock", x => x.UserBlockId);
                    table.ForeignKey(
                        name: "FK_UserBlock_User_BlockedUserId",
                        column: x => x.BlockedUserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserBlock_User_BlockingUserId",
                        column: x => x.BlockingUserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Writing",
                schema: "dbo",
                columns: table => new
                {
                    WritingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessPermissionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Document = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LikesOn = table.Column<bool>(type: "bit", nullable: false),
                    CommentsOn = table.Column<bool>(type: "bit", nullable: false),
                    CritiqueOn = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsStandAlone = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Writing", x => x.WritingId);
                    table.ForeignKey(
                        name: "FK_Writing_AccessPermission_AccessPermissionId",
                        column: x => x.AccessPermissionId,
                        principalSchema: "dbo",
                        principalTable: "AccessPermission",
                        principalColumn: "AccessPermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Writing_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FolderFormat",
                schema: "dbo",
                columns: table => new
                {
                    FolderId = table.Column<int>(type: "int", nullable: false),
                    FormatId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderFormat", x => new { x.FolderId, x.FormatId });
                    table.ForeignKey(
                        name: "FK_FolderFormat_Folder_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "dbo",
                        principalTable: "Folder",
                        principalColumn: "FolderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FolderFormat_FormatTag_FormatId",
                        column: x => x.FormatId,
                        principalSchema: "dbo",
                        principalTable: "FormatTag",
                        principalColumn: "FormatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FolderGenre",
                schema: "dbo",
                columns: table => new
                {
                    FolderId = table.Column<int>(type: "int", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderGenre", x => new { x.GenreId, x.FolderId });
                    table.ForeignKey(
                        name: "FK_FolderGenre_Folder_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "dbo",
                        principalTable: "Folder",
                        principalColumn: "FolderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FolderGenre_GenreTag_GenreId",
                        column: x => x.GenreId,
                        principalSchema: "dbo",
                        principalTable: "GenreTag",
                        principalColumn: "GenreId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FolderSubfolder",
                schema: "dbo",
                columns: table => new
                {
                    FolderId = table.Column<int>(type: "int", nullable: false),
                    SubfolderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderSubfolder", x => new { x.FolderId, x.SubfolderId });
                    table.ForeignKey(
                        name: "FK_FolderSubfolder_Folder_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "dbo",
                        principalTable: "Folder",
                        principalColumn: "FolderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FolderSubfolder_Folder_SubfolderId",
                        column: x => x.SubfolderId,
                        principalSchema: "dbo",
                        principalTable: "Folder",
                        principalColumn: "FolderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccessRequest",
                schema: "dbo",
                columns: table => new
                {
                    AccessRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessPermissionId = table.Column<int>(type: "int", nullable: false),
                    RequesterId = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Resolved = table.Column<bool>(type: "bit", nullable: false),
                    PenProfileProfileId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessRequest", x => x.AccessRequestId);
                    table.ForeignKey(
                        name: "FK_AccessRequest_AccessPermission_AccessPermissionId",
                        column: x => x.AccessPermissionId,
                        principalSchema: "dbo",
                        principalTable: "AccessPermission",
                        principalColumn: "AccessPermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessRequest_Profile_PenProfileProfileId",
                        column: x => x.PenProfileProfileId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId");
                    table.ForeignKey(
                        name: "FK_AccessRequest_Profile_RequesterId",
                        column: x => x.RequesterId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CritiqueGiver",
                schema: "dbo",
                columns: table => new
                {
                    CritiqueGiverId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CriticId = table.Column<int>(type: "int", nullable: false),
                    ForAny = table.Column<bool>(type: "bit", nullable: false),
                    ForFriends = table.Column<bool>(type: "bit", nullable: false),
                    ForMyWriters = table.Column<bool>(type: "bit", nullable: false),
                    ForProfilesFollowing = table.Column<bool>(type: "bit", nullable: false),
                    ForFoldersFollowing = table.Column<bool>(type: "bit", nullable: false),
                    ForSeriesFollowing = table.Column<bool>(type: "bit", nullable: false),
                    ForFormatFollowing = table.Column<bool>(type: "bit", nullable: false),
                    ForGenreFollowing = table.Column<bool>(type: "bit", nullable: false),
                    ForMatureWriting = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CritiqueGiver", x => x.CritiqueGiverId);
                    table.ForeignKey(
                        name: "FK_CritiqueGiver_Profile_CriticId",
                        column: x => x.CriticId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FolderOwner",
                schema: "dbo",
                columns: table => new
                {
                    FolderId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    PenProfileProfileId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderOwner", x => new { x.FolderId, x.OwnerId });
                    table.ForeignKey(
                        name: "FK_FolderOwner_Folder_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "dbo",
                        principalTable: "Folder",
                        principalColumn: "FolderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FolderOwner_Profile_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FolderOwner_Profile_PenProfileProfileId",
                        column: x => x.PenProfileProfileId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId");
                });

            migrationBuilder.CreateTable(
                name: "FriendRequest",
                schema: "dbo",
                columns: table => new
                {
                    FriendRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequesterId = table.Column<int>(type: "int", nullable: false),
                    RequesteeId = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Resolved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequest", x => x.FriendRequestId);
                    table.ForeignKey(
                        name: "FK_FriendRequest_Profile_RequesteeId",
                        column: x => x.RequesteeId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendRequest_Profile_RequesterId",
                        column: x => x.RequesterId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Friendship",
                schema: "dbo",
                columns: table => new
                {
                    FriendshipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstFriendId = table.Column<int>(type: "int", nullable: false),
                    SecondFriendId = table.Column<int>(type: "int", nullable: false),
                    AcceptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendship", x => x.FriendshipId);
                    table.ForeignKey(
                        name: "FK_Friendship_Profile_FirstFriendId",
                        column: x => x.FirstFriendId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Friendship_Profile_SecondFriendId",
                        column: x => x.SecondFriendId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IndividualAccessGrant",
                schema: "dbo",
                columns: table => new
                {
                    IndividualAccessGrantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessPermissionId = table.Column<int>(type: "int", nullable: false),
                    GranteeId = table.Column<int>(type: "int", nullable: false),
                    GrantDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndividualAccessGrant", x => x.IndividualAccessGrantId);
                    table.ForeignKey(
                        name: "FK_IndividualAccessGrant_AccessPermission_AccessPermissionId",
                        column: x => x.AccessPermissionId,
                        principalSchema: "dbo",
                        principalTable: "AccessPermission",
                        principalColumn: "AccessPermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndividualAccessGrant_Profile_GranteeId",
                        column: x => x.GranteeId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IndividualAccessRevoke",
                schema: "dbo",
                columns: table => new
                {
                    IndividualAccessRevokeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessPermissionId = table.Column<int>(type: "int", nullable: false),
                    RevokeeId = table.Column<int>(type: "int", nullable: false),
                    RevokeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndividualAccessRevoke", x => x.IndividualAccessRevokeId);
                    table.ForeignKey(
                        name: "FK_IndividualAccessRevoke_AccessPermission_AccessPermissionId",
                        column: x => x.AccessPermissionId,
                        principalSchema: "dbo",
                        principalTable: "AccessPermission",
                        principalColumn: "AccessPermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndividualAccessRevoke_Profile_RevokeeId",
                        column: x => x.RevokeeId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FollowerFollowing",
                schema: "dbo",
                columns: table => new
                {
                    FollowerFollowingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FollowerId = table.Column<int>(type: "int", nullable: false),
                    ProfileId = table.Column<int>(type: "int", nullable: true),
                    FolderId = table.Column<int>(type: "int", nullable: false),
                    FormatId = table.Column<int>(type: "int", nullable: true),
                    GenreId = table.Column<int>(type: "int", nullable: true),
                    SeriesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowerFollowing", x => x.FollowerFollowingId);
                    table.ForeignKey(
                        name: "FK_FollowerFollowing_Folder_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "dbo",
                        principalTable: "Folder",
                        principalColumn: "FolderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FollowerFollowing_FormatTag_FormatId",
                        column: x => x.FormatId,
                        principalSchema: "dbo",
                        principalTable: "FormatTag",
                        principalColumn: "FormatId");
                    table.ForeignKey(
                        name: "FK_FollowerFollowing_GenreTag_GenreId",
                        column: x => x.GenreId,
                        principalSchema: "dbo",
                        principalTable: "GenreTag",
                        principalColumn: "GenreId");
                    table.ForeignKey(
                        name: "FK_FollowerFollowing_Profile_FollowerId",
                        column: x => x.FollowerId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FollowerFollowing_Profile_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FollowerFollowing_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalSchema: "dbo",
                        principalTable: "Series",
                        principalColumn: "SeriesId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SeriesFormat",
                schema: "dbo",
                columns: table => new
                {
                    SeriesId = table.Column<int>(type: "int", nullable: false),
                    FormatId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesFormat", x => new { x.SeriesId, x.FormatId });
                    table.ForeignKey(
                        name: "FK_SeriesFormat_FormatTag_FormatId",
                        column: x => x.FormatId,
                        principalSchema: "dbo",
                        principalTable: "FormatTag",
                        principalColumn: "FormatId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeriesFormat_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalSchema: "dbo",
                        principalTable: "Series",
                        principalColumn: "SeriesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeriesGenre",
                schema: "dbo",
                columns: table => new
                {
                    SeriesId = table.Column<int>(type: "int", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesGenre", x => new { x.SeriesId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_SeriesGenre_GenreTag_GenreId",
                        column: x => x.GenreId,
                        principalSchema: "dbo",
                        principalTable: "GenreTag",
                        principalColumn: "GenreId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeriesGenre_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalSchema: "dbo",
                        principalTable: "Series",
                        principalColumn: "SeriesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeriesOwner",
                schema: "dbo",
                columns: table => new
                {
                    SeriesId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesOwner", x => new { x.SeriesId, x.OwnerId });
                    table.ForeignKey(
                        name: "FK_SeriesOwner_Profile_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeriesOwner_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalSchema: "dbo",
                        principalTable: "Series",
                        principalColumn: "SeriesId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SeriesSeries",
                schema: "dbo",
                columns: table => new
                {
                    OverarchingSeriesId = table.Column<int>(type: "int", nullable: false),
                    SeriesMemberId = table.Column<int>(type: "int", nullable: false),
                    IsStandAlone = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesSeries", x => new { x.OverarchingSeriesId, x.SeriesMemberId });
                    table.ForeignKey(
                        name: "FK_SeriesSeries_Series_OverarchingSeriesId",
                        column: x => x.OverarchingSeriesId,
                        principalSchema: "dbo",
                        principalTable: "Series",
                        principalColumn: "SeriesId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeriesSeries_Series_SeriesMemberId",
                        column: x => x.SeriesMemberId,
                        principalSchema: "dbo",
                        principalTable: "Series",
                        principalColumn: "SeriesId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                schema: "dbo",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommenterId = table.Column<int>(type: "int", nullable: false),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CommentText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileId = table.Column<int>(type: "int", nullable: true),
                    WritingId = table.Column<int>(type: "int", nullable: false),
                    FolderId = table.Column<int>(type: "int", nullable: false),
                    SeriesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comments_Folder_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "dbo",
                        principalTable: "Folder",
                        principalColumn: "FolderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Profile_CommenterId",
                        column: x => x.CommenterId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Profile_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId");
                    table.ForeignKey(
                        name: "FK_Comments_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalSchema: "dbo",
                        principalTable: "Series",
                        principalColumn: "SeriesId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Writing_WritingId",
                        column: x => x.WritingId,
                        principalSchema: "dbo",
                        principalTable: "Writing",
                        principalColumn: "WritingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Critique",
                schema: "dbo",
                columns: table => new
                {
                    CritiqueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CriticId = table.Column<int>(type: "int", nullable: false),
                    WritingId = table.Column<int>(type: "int", nullable: false),
                    CritiqueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditedDocument = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Critique", x => x.CritiqueId);
                    table.ForeignKey(
                        name: "FK_Critique_Profile_CriticId",
                        column: x => x.CriticId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Critique_Writing_WritingId",
                        column: x => x.WritingId,
                        principalSchema: "dbo",
                        principalTable: "Writing",
                        principalColumn: "WritingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CritiqueRequest",
                schema: "dbo",
                columns: table => new
                {
                    CritiqueRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WritingId = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    WritingId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CritiqueRequest", x => x.CritiqueRequestId);
                    table.ForeignKey(
                        name: "FK_CritiqueRequest_Writing_WritingId",
                        column: x => x.WritingId,
                        principalSchema: "dbo",
                        principalTable: "Writing",
                        principalColumn: "WritingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CritiqueRequest_Writing_WritingId1",
                        column: x => x.WritingId1,
                        principalSchema: "dbo",
                        principalTable: "Writing",
                        principalColumn: "WritingId");
                });

            migrationBuilder.CreateTable(
                name: "WritingFolder",
                schema: "dbo",
                columns: table => new
                {
                    WritingId = table.Column<int>(type: "int", nullable: false),
                    FolderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WritingFolder", x => new { x.WritingId, x.FolderId });
                    table.ForeignKey(
                        name: "FK_WritingFolder_Folder_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "dbo",
                        principalTable: "Folder",
                        principalColumn: "FolderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WritingFolder_Writing_WritingId",
                        column: x => x.WritingId,
                        principalSchema: "dbo",
                        principalTable: "Writing",
                        principalColumn: "WritingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WritingFormat",
                schema: "dbo",
                columns: table => new
                {
                    WritingId = table.Column<int>(type: "int", nullable: false),
                    FormatId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WritingFormat", x => new { x.WritingId, x.FormatId });
                    table.ForeignKey(
                        name: "FK_WritingFormat_FormatTag_FormatId",
                        column: x => x.FormatId,
                        principalSchema: "dbo",
                        principalTable: "FormatTag",
                        principalColumn: "FormatId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WritingFormat_Writing_WritingId",
                        column: x => x.WritingId,
                        principalSchema: "dbo",
                        principalTable: "Writing",
                        principalColumn: "WritingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WritingGenre",
                schema: "dbo",
                columns: table => new
                {
                    WritingId = table.Column<int>(type: "int", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WritingGenre", x => new { x.GenreId, x.WritingId });
                    table.ForeignKey(
                        name: "FK_WritingGenre_GenreTag_GenreId",
                        column: x => x.GenreId,
                        principalSchema: "dbo",
                        principalTable: "GenreTag",
                        principalColumn: "GenreId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WritingGenre_Writing_WritingId",
                        column: x => x.WritingId,
                        principalSchema: "dbo",
                        principalTable: "Writing",
                        principalColumn: "WritingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WritingProfile",
                schema: "dbo",
                columns: table => new
                {
                    WritingId = table.Column<int>(type: "int", nullable: false),
                    ProfileId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WritingProfile", x => new { x.WritingId, x.ProfileId });
                    table.ForeignKey(
                        name: "FK_WritingProfile_Profile_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WritingProfile_Writing_WritingId",
                        column: x => x.WritingId,
                        principalSchema: "dbo",
                        principalTable: "Writing",
                        principalColumn: "WritingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WritingSeries",
                schema: "dbo",
                columns: table => new
                {
                    WritingSeriesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeriesId = table.Column<int>(type: "int", nullable: false),
                    WritingId = table.Column<int>(type: "int", nullable: false),
                    PreviousWritingId = table.Column<int>(type: "int", nullable: false),
                    NextWritingId = table.Column<int>(type: "int", nullable: false),
                    WritingId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WritingSeries", x => x.WritingSeriesId);
                    table.ForeignKey(
                        name: "FK_WritingSeries_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalSchema: "dbo",
                        principalTable: "Series",
                        principalColumn: "SeriesId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WritingSeries_Writing_NextWritingId",
                        column: x => x.NextWritingId,
                        principalSchema: "dbo",
                        principalTable: "Writing",
                        principalColumn: "WritingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WritingSeries_Writing_PreviousWritingId",
                        column: x => x.PreviousWritingId,
                        principalSchema: "dbo",
                        principalTable: "Writing",
                        principalColumn: "WritingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WritingSeries_Writing_WritingId",
                        column: x => x.WritingId,
                        principalSchema: "dbo",
                        principalTable: "Writing",
                        principalColumn: "WritingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WritingSeries_Writing_WritingId1",
                        column: x => x.WritingId1,
                        principalSchema: "dbo",
                        principalTable: "Writing",
                        principalColumn: "WritingId");
                });

            migrationBuilder.CreateTable(
                name: "CommentFlag",
                schema: "dbo",
                columns: table => new
                {
                    CommentFlagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentId = table.Column<int>(type: "int", nullable: false),
                    FlaggerId = table.Column<int>(type: "int", nullable: false),
                    FlagDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FlagReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentFlag", x => x.CommentFlagId);
                    table.ForeignKey(
                        name: "FK_CommentFlag_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "dbo",
                        principalTable: "Comments",
                        principalColumn: "CommentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentFlag_Profile_FlaggerId",
                        column: x => x.FlaggerId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentReply",
                schema: "dbo",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false),
                    ReplyId = table.Column<int>(type: "int", nullable: false),
                    CommentId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentReply", x => new { x.CommentId, x.ReplyId });
                    table.ForeignKey(
                        name: "FK_CommentReply_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "dbo",
                        principalTable: "Comments",
                        principalColumn: "CommentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentReply_Comments_CommentId1",
                        column: x => x.CommentId1,
                        principalSchema: "dbo",
                        principalTable: "Comments",
                        principalColumn: "CommentId");
                    table.ForeignKey(
                        name: "FK_CommentReply_Comments_ReplyId",
                        column: x => x.ReplyId,
                        principalSchema: "dbo",
                        principalTable: "Comments",
                        principalColumn: "CommentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                schema: "dbo",
                columns: table => new
                {
                    LikeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LikerId = table.Column<int>(type: "int", nullable: false),
                    IsAnonymous = table.Column<bool>(type: "bit", nullable: false),
                    LikeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProfileId = table.Column<int>(type: "int", nullable: true),
                    WritingId = table.Column<int>(type: "int", nullable: false),
                    FolderId = table.Column<int>(type: "int", nullable: false),
                    SeriesId = table.Column<int>(type: "int", nullable: false),
                    CommentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.LikeId);
                    table.ForeignKey(
                        name: "FK_Likes_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "dbo",
                        principalTable: "Comments",
                        principalColumn: "CommentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_Folder_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "dbo",
                        principalTable: "Folder",
                        principalColumn: "FolderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Likes_Profile_LikerId",
                        column: x => x.LikerId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Likes_Profile_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Likes_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalSchema: "dbo",
                        principalTable: "Series",
                        principalColumn: "SeriesId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Likes_Writing_WritingId",
                        column: x => x.WritingId,
                        principalSchema: "dbo",
                        principalTable: "Writing",
                        principalColumn: "WritingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessRequest_AccessPermissionId",
                schema: "dbo",
                table: "AccessRequest",
                column: "AccessPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessRequest_PenProfileProfileId",
                schema: "dbo",
                table: "AccessRequest",
                column: "PenProfileProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessRequest_RequesterId",
                schema: "dbo",
                table: "AccessRequest",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_AltFormatName_FormatId",
                schema: "dbo",
                table: "AltFormatName",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_AltGenreName_GenreId",
                schema: "dbo",
                table: "AltGenreName",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentFlag_CommentId",
                schema: "dbo",
                table: "CommentFlag",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentFlag_FlaggerId",
                schema: "dbo",
                table: "CommentFlag",
                column: "FlaggerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReply_CommentId1",
                schema: "dbo",
                table: "CommentReply",
                column: "CommentId1",
                unique: true,
                filter: "[CommentId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReply_ReplyId",
                schema: "dbo",
                table: "CommentReply",
                column: "ReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommenterId",
                schema: "dbo",
                table: "Comments",
                column: "CommenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_FolderId",
                schema: "dbo",
                table: "Comments",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ProfileId",
                schema: "dbo",
                table: "Comments",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_SeriesId",
                schema: "dbo",
                table: "Comments",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_WritingId",
                schema: "dbo",
                table: "Comments",
                column: "WritingId");

            migrationBuilder.CreateIndex(
                name: "IX_Critique_CriticId",
                schema: "dbo",
                table: "Critique",
                column: "CriticId");

            migrationBuilder.CreateIndex(
                name: "IX_Critique_WritingId",
                schema: "dbo",
                table: "Critique",
                column: "WritingId");

            migrationBuilder.CreateIndex(
                name: "IX_CritiqueGiver_CriticId",
                schema: "dbo",
                table: "CritiqueGiver",
                column: "CriticId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CritiqueRequest_WritingId",
                schema: "dbo",
                table: "CritiqueRequest",
                column: "WritingId");

            migrationBuilder.CreateIndex(
                name: "IX_CritiqueRequest_WritingId1",
                schema: "dbo",
                table: "CritiqueRequest",
                column: "WritingId1",
                unique: true,
                filter: "[WritingId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Folder_AccessPermissionId",
                schema: "dbo",
                table: "Folder",
                column: "AccessPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Folder_CreatorId",
                schema: "dbo",
                table: "Folder",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderFormat_FormatId",
                schema: "dbo",
                table: "FolderFormat",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderGenre_FolderId",
                schema: "dbo",
                table: "FolderGenre",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderOwner_OwnerId",
                schema: "dbo",
                table: "FolderOwner",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderOwner_PenProfileProfileId",
                schema: "dbo",
                table: "FolderOwner",
                column: "PenProfileProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderSubfolder_SubfolderId",
                schema: "dbo",
                table: "FolderSubfolder",
                column: "SubfolderId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerFollowing_FolderId",
                schema: "dbo",
                table: "FollowerFollowing",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerFollowing_FollowerId",
                schema: "dbo",
                table: "FollowerFollowing",
                column: "FollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerFollowing_FormatId",
                schema: "dbo",
                table: "FollowerFollowing",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerFollowing_GenreId",
                schema: "dbo",
                table: "FollowerFollowing",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerFollowing_ProfileId",
                schema: "dbo",
                table: "FollowerFollowing",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerFollowing_SeriesId",
                schema: "dbo",
                table: "FollowerFollowing",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_FormatCategory_FormatId",
                schema: "dbo",
                table: "FormatCategory",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_FormatCategory_ParentId",
                schema: "dbo",
                table: "FormatCategory",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_FormatCategory_SecondaryParentId",
                schema: "dbo",
                table: "FormatCategory",
                column: "SecondaryParentId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequest_RequesteeId",
                schema: "dbo",
                table: "FriendRequest",
                column: "RequesteeId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequest_RequesterId",
                schema: "dbo",
                table: "FriendRequest",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_FirstFriendId",
                schema: "dbo",
                table: "Friendship",
                column: "FirstFriendId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_SecondFriendId",
                schema: "dbo",
                table: "Friendship",
                column: "SecondFriendId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreCategory_GenreId",
                schema: "dbo",
                table: "GenreCategory",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreCategory_ParentId",
                schema: "dbo",
                table: "GenreCategory",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreCategory_SecondaryParentId",
                schema: "dbo",
                table: "GenreCategory",
                column: "SecondaryParentId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreCategory_TertiaryParentId",
                schema: "dbo",
                table: "GenreCategory",
                column: "TertiaryParentId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreFormat_GenreId",
                schema: "dbo",
                table: "GenreFormat",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreFormat_ParentFormatId",
                schema: "dbo",
                table: "GenreFormat",
                column: "ParentFormatId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreFormat_ParentGenreId",
                schema: "dbo",
                table: "GenreFormat",
                column: "ParentGenreId");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualAccessGrant_AccessPermissionId",
                schema: "dbo",
                table: "IndividualAccessGrant",
                column: "AccessPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualAccessGrant_GranteeId",
                schema: "dbo",
                table: "IndividualAccessGrant",
                column: "GranteeId");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualAccessRevoke_AccessPermissionId",
                schema: "dbo",
                table: "IndividualAccessRevoke",
                column: "AccessPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_IndividualAccessRevoke_RevokeeId",
                schema: "dbo",
                table: "IndividualAccessRevoke",
                column: "RevokeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_CommentId",
                schema: "dbo",
                table: "Likes",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_FolderId",
                schema: "dbo",
                table: "Likes",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_LikerId",
                schema: "dbo",
                table: "Likes",
                column: "LikerId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_ProfileId",
                schema: "dbo",
                table: "Likes",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_SeriesId",
                schema: "dbo",
                table: "Likes",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_WritingId",
                schema: "dbo",
                table: "Likes",
                column: "WritingId");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_AccessPermissionId",
                schema: "dbo",
                table: "Profile",
                column: "AccessPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_RoleId",
                schema: "dbo",
                table: "Profile",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_UserId",
                schema: "dbo",
                table: "Profile",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Series_AccessPermissionId",
                schema: "dbo",
                table: "Series",
                column: "AccessPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Series_CreatorId",
                schema: "dbo",
                table: "Series",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Series_NextSeriesId",
                schema: "dbo",
                table: "Series",
                column: "NextSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Series_PreviousSeriesId",
                schema: "dbo",
                table: "Series",
                column: "PreviousSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesFormat_FormatId",
                schema: "dbo",
                table: "SeriesFormat",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesGenre_GenreId",
                schema: "dbo",
                table: "SeriesGenre",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesOwner_OwnerId",
                schema: "dbo",
                table: "SeriesOwner",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesSeries_SeriesMemberId",
                schema: "dbo",
                table: "SeriesSeries",
                column: "SeriesMemberId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "dbo",
                table: "User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "dbo",
                table: "User",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlock_BlockedUserId",
                schema: "dbo",
                table: "UserBlock",
                column: "BlockedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlock_BlockingUserId",
                schema: "dbo",
                table: "UserBlock",
                column: "BlockingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Writing_AccessPermissionId",
                schema: "dbo",
                table: "Writing",
                column: "AccessPermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Writing_UserId",
                schema: "dbo",
                table: "Writing",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WritingFolder_FolderId",
                schema: "dbo",
                table: "WritingFolder",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_WritingFormat_FormatId",
                schema: "dbo",
                table: "WritingFormat",
                column: "FormatId");

            migrationBuilder.CreateIndex(
                name: "IX_WritingGenre_WritingId",
                schema: "dbo",
                table: "WritingGenre",
                column: "WritingId");

            migrationBuilder.CreateIndex(
                name: "IX_WritingProfile_ProfileId",
                schema: "dbo",
                table: "WritingProfile",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_WritingSeries_NextWritingId",
                schema: "dbo",
                table: "WritingSeries",
                column: "NextWritingId");

            migrationBuilder.CreateIndex(
                name: "IX_WritingSeries_PreviousWritingId",
                schema: "dbo",
                table: "WritingSeries",
                column: "PreviousWritingId");

            migrationBuilder.CreateIndex(
                name: "IX_WritingSeries_SeriesId",
                schema: "dbo",
                table: "WritingSeries",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_WritingSeries_WritingId",
                schema: "dbo",
                table: "WritingSeries",
                column: "WritingId");

            migrationBuilder.CreateIndex(
                name: "IX_WritingSeries_WritingId1",
                schema: "dbo",
                table: "WritingSeries",
                column: "WritingId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaims_Role_RoleId",
                schema: "dbo",
                table: "RoleClaims",
                column: "RoleId",
                principalSchema: "dbo",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaims_User_UserId",
                schema: "dbo",
                table: "UserClaims",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogins_User_UserId",
                schema: "dbo",
                table: "UserLogins",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Role_RoleId",
                schema: "dbo",
                table: "UserRoles",
                column: "RoleId",
                principalSchema: "dbo",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_User_UserId",
                schema: "dbo",
                table: "UserRoles",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_User_UserId",
                schema: "dbo",
                table: "UserTokens",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleClaims_Role_RoleId",
                schema: "dbo",
                table: "RoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaims_User_UserId",
                schema: "dbo",
                table: "UserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogins_User_UserId",
                schema: "dbo",
                table: "UserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Role_RoleId",
                schema: "dbo",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_User_UserId",
                schema: "dbo",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_User_UserId",
                schema: "dbo",
                table: "UserTokens");

            migrationBuilder.DropTable(
                name: "AccessRequest",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AltFormatName",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AltGenreName",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CommentFlag",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CommentReply",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Critique",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CritiqueGiver",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CritiqueRequest",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "FolderFormat",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "FolderGenre",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "FolderOwner",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "FolderSubfolder",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "FollowerFollowing",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "FormatCategory",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "FriendRequest",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Friendship",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "GenreCategory",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "GenreFormat",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "IndividualAccessGrant",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "IndividualAccessRevoke",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Likes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SeriesFormat",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SeriesGenre",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SeriesOwner",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "SeriesSeries",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserBlock",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WritingFolder",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WritingFormat",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WritingGenre",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WritingProfile",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "WritingSeries",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Comments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "FormatTag",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "GenreTag",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Folder",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Profile",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Series",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Writing",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PenRole",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AccessPermission",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "User",
                schema: "dbo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTokens",
                schema: "dbo",
                table: "UserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                schema: "dbo",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLogins",
                schema: "dbo",
                table: "UserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClaims",
                schema: "dbo",
                table: "UserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleClaims",
                schema: "dbo",
                table: "RoleClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                schema: "dbo",
                table: "Role");

            migrationBuilder.RenameTable(
                name: "UserTokens",
                schema: "dbo",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                schema: "dbo",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "UserLogins",
                schema: "dbo",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "UserClaims",
                schema: "dbo",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "RoleClaims",
                schema: "dbo",
                newName: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "Role",
                schema: "dbo",
                newName: "AspNetRoles");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_RoleId",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLogins_UserId",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserClaims_UserId",
                table: "AspNetUserClaims",
                newName: "IX_AspNetUserClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleClaims_RoleId",
                table: "AspNetRoleClaims",
                newName: "IX_AspNetRoleClaims_RoleId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
