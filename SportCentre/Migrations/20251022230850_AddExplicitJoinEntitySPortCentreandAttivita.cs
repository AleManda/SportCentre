using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportCentre.Migrations
{
    /// <inheritdoc />
    public partial class AddExplicitJoinEntitySPortCentreandAttivita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SportCentreAttivita",
                columns: table => new
                {
                    SportCentreId = table.Column<int>(type: "int", nullable: false),
                    AttivitaId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportCentreAttivita", x => new { x.SportCentreId, x.AttivitaId });
                    table.ForeignKey(
                        name: "FK_SportCentreAttivita_SportCentres_SportCentreId",
                        column: x => x.SportCentreId,
                        principalTable: "SportCentres",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SportCentreAttivita_attivita_AttivitaId",
                        column: x => x.AttivitaId,
                        principalTable: "attivita",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SportCentreAttivita_AttivitaId",
                table: "SportCentreAttivita",
                column: "AttivitaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SportCentreAttivita");
        }
    }
}
