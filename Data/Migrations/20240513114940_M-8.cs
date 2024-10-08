using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace temp_back.Data.Migrations
{
    /// <inheritdoc />
    public partial class M8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "payements",
                columns: table => new
                {
                    Id_payement = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DevisId_devis = table.Column<int>(type: "integer", nullable: false),
                    Date_payement = table.Column<DateOnly>(type: "date", nullable: false),
                    Montant = table.Column<double>(type: "double precision", nullable: false),
                    Reste_payer = table.Column<double>(type: "double precision", precision: 20, scale: 0, nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payements", x => x.Id_payement);
                    table.ForeignKey(
                        name: "FK_payements_devis_DevisId_devis",
                        column: x => x.DevisId_devis,
                        principalTable: "devis",
                        principalColumn: "Id_devis",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payements_DevisId_devis",
                table: "payements",
                column: "DevisId_devis");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payements");
        }
    }
}
