using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Penfolio2.Data.Migrations
{
    /// <inheritdoc />
    public partial class Addedscaffoldingforconnectingwriterstopublishers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MyAgentAccess",
                schema: "dbo",
                table: "AccessPermission",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PublisherWriter",
                schema: "dbo",
                columns: table => new
                {
                    PublisherWriterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublisherId = table.Column<int>(type: "int", nullable: false),
                    WriterId = table.Column<int>(type: "int", nullable: false),
                    AcceptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublisherWriter", x => x.PublisherWriterId);
                    table.ForeignKey(
                        name: "FK_PublisherWriter_Profile_PublisherId",
                        column: x => x.PublisherId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublisherWriter_Profile_WriterId",
                        column: x => x.WriterId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RepresentationRequest",
                schema: "dbo",
                columns: table => new
                {
                    RepresentationRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequesterId = table.Column<int>(type: "int", nullable: false),
                    RequesteeId = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Resolved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepresentationRequest", x => x.RepresentationRequestId);
                    table.ForeignKey(
                        name: "FK_RepresentationRequest_Profile_RequesteeId",
                        column: x => x.RequesteeId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RepresentationRequest_Profile_RequesterId",
                        column: x => x.RequesterId,
                        principalSchema: "dbo",
                        principalTable: "Profile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PublisherWriter_PublisherId",
                schema: "dbo",
                table: "PublisherWriter",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_PublisherWriter_WriterId",
                schema: "dbo",
                table: "PublisherWriter",
                column: "WriterId");

            migrationBuilder.CreateIndex(
                name: "IX_RepresentationRequest_RequesteeId",
                schema: "dbo",
                table: "RepresentationRequest",
                column: "RequesteeId");

            migrationBuilder.CreateIndex(
                name: "IX_RepresentationRequest_RequesterId",
                schema: "dbo",
                table: "RepresentationRequest",
                column: "RequesterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublisherWriter",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RepresentationRequest",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "MyAgentAccess",
                schema: "dbo",
                table: "AccessPermission");
        }
    }
}
