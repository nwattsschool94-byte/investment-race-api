using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentRace.API.Migrations
{
    /// <inheritdoc />
    public partial class AddYouthNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YouthNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YouthId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YouthNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YouthNotes_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_YouthNotes_Youths_YouthId",
                        column: x => x.YouthId,
                        principalTable: "Youths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_YouthNotes_CreatedByUserId",
                table: "YouthNotes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_YouthNotes_YouthId",
                table: "YouthNotes",
                column: "YouthId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YouthNotes");
        }
    }
}
