using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Penfolio2.Data.Migrations
{
    /// <inheritdoc />
    public partial class addednewcolumntokeeptrackofotherfriendshipId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OtherFriendshipId",
                schema: "dbo",
                table: "Friendship",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherFriendshipId",
                schema: "dbo",
                table: "Friendship");
        }
    }
}
