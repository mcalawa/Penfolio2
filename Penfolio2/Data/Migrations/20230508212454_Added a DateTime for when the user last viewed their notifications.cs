using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Penfolio2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedaDateTimeforwhentheuserlastviewedtheirnotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Folder_FolderId",
                schema: "dbo",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_FollowerFollowing_Folder_FolderId",
                schema: "dbo",
                table: "FollowerFollowing");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Comments_CommentId",
                schema: "dbo",
                table: "Likes");

            migrationBuilder.AlterColumn<int>(
                name: "PreviousWritingId",
                schema: "dbo",
                table: "WritingSeries",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "NextWritingId",
                schema: "dbo",
                table: "WritingSeries",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastNotificationViewDate",
                schema: "dbo",
                table: "User",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "PreviousSeriesId",
                schema: "dbo",
                table: "Series",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "WritingId",
                schema: "dbo",
                table: "Likes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SeriesId",
                schema: "dbo",
                table: "Likes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FolderId",
                schema: "dbo",
                table: "Likes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CommentId",
                schema: "dbo",
                table: "Likes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ParentGenreId",
                schema: "dbo",
                table: "GenreFormat",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TertiaryParentId",
                schema: "dbo",
                table: "GenreCategory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SecondaryParentId",
                schema: "dbo",
                table: "GenreCategory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SecondaryParentId",
                schema: "dbo",
                table: "FormatCategory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SeriesId",
                schema: "dbo",
                table: "FollowerFollowing",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FolderId",
                schema: "dbo",
                table: "FollowerFollowing",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "WritingId",
                schema: "dbo",
                table: "Comments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SeriesId",
                schema: "dbo",
                table: "Comments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FolderId",
                schema: "dbo",
                table: "Comments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Folder_FolderId",
                schema: "dbo",
                table: "Comments",
                column: "FolderId",
                principalSchema: "dbo",
                principalTable: "Folder",
                principalColumn: "FolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowerFollowing_Folder_FolderId",
                schema: "dbo",
                table: "FollowerFollowing",
                column: "FolderId",
                principalSchema: "dbo",
                principalTable: "Folder",
                principalColumn: "FolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Comments_CommentId",
                schema: "dbo",
                table: "Likes",
                column: "CommentId",
                principalSchema: "dbo",
                principalTable: "Comments",
                principalColumn: "CommentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Folder_FolderId",
                schema: "dbo",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_FollowerFollowing_Folder_FolderId",
                schema: "dbo",
                table: "FollowerFollowing");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Comments_CommentId",
                schema: "dbo",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "LastNotificationViewDate",
                schema: "dbo",
                table: "User");

            migrationBuilder.AlterColumn<int>(
                name: "PreviousWritingId",
                schema: "dbo",
                table: "WritingSeries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NextWritingId",
                schema: "dbo",
                table: "WritingSeries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PreviousSeriesId",
                schema: "dbo",
                table: "Series",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "WritingId",
                schema: "dbo",
                table: "Likes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SeriesId",
                schema: "dbo",
                table: "Likes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FolderId",
                schema: "dbo",
                table: "Likes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CommentId",
                schema: "dbo",
                table: "Likes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ParentGenreId",
                schema: "dbo",
                table: "GenreFormat",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TertiaryParentId",
                schema: "dbo",
                table: "GenreCategory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SecondaryParentId",
                schema: "dbo",
                table: "GenreCategory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SecondaryParentId",
                schema: "dbo",
                table: "FormatCategory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SeriesId",
                schema: "dbo",
                table: "FollowerFollowing",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FolderId",
                schema: "dbo",
                table: "FollowerFollowing",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "WritingId",
                schema: "dbo",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SeriesId",
                schema: "dbo",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FolderId",
                schema: "dbo",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Folder_FolderId",
                schema: "dbo",
                table: "Comments",
                column: "FolderId",
                principalSchema: "dbo",
                principalTable: "Folder",
                principalColumn: "FolderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FollowerFollowing_Folder_FolderId",
                schema: "dbo",
                table: "FollowerFollowing",
                column: "FolderId",
                principalSchema: "dbo",
                principalTable: "Folder",
                principalColumn: "FolderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Comments_CommentId",
                schema: "dbo",
                table: "Likes",
                column: "CommentId",
                principalSchema: "dbo",
                principalTable: "Comments",
                principalColumn: "CommentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
