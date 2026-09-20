using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentRace.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSocialWorkerYouths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SocialWorkerYouths",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SocialWorkerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    YouthId = table.Column<int>(type: "int", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialWorkerYouths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialWorkerYouths_AspNetUsers_SocialWorkerId",
                        column: x => x.SocialWorkerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialWorkerYouths_Youths_YouthId",
                        column: x => x.YouthId,
                        principalTable: "Youths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SocialWorkerYouths_SocialWorkerId_YouthId",
                table: "SocialWorkerYouths",
                columns: new[] { "SocialWorkerId", "YouthId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SocialWorkerYouths_YouthId",
                table: "SocialWorkerYouths",
                column: "YouthId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SocialWorkerYouths");
        }
    }
}
