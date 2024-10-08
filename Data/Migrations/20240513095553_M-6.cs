using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace temp_back.Data.Migrations
{
    /// <inheritdoc />
    public partial class M6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "devis_details",
                columns: table => new
                {
                    Id_devis_detail = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DevisId_devis = table.Column<int>(type: "integer", nullable: false),
                    Id_detail_maison = table.Column<int>(type: "integer", nullable: false),
                    TravauxId_travaux = table.Column<int>(type: "integer", nullable: false),
                    MaisonId_maison = table.Column<int>(type: "integer", nullable: false),
                    Quantite = table.Column<double>(type: "double precision", nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devis_details", x => x.Id_devis_detail);
                    table.ForeignKey(
                        name: "FK_devis_details_devis_DevisId_devis",
                        column: x => x.DevisId_devis,
                        principalTable: "devis",
                        principalColumn: "Id_devis",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_devis_details_maisons_MaisonId_maison",
                        column: x => x.MaisonId_maison,
                        principalTable: "maisons",
                        principalColumn: "Id_maison",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_devis_details_travaux_TravauxId_travaux",
                        column: x => x.TravauxId_travaux,
                        principalTable: "travaux",
                        principalColumn: "Id_travaux",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_devis_details_DevisId_devis",
                table: "devis_details",
                column: "DevisId_devis");

            migrationBuilder.CreateIndex(
                name: "IX_devis_details_MaisonId_maison",
                table: "devis_details",
                column: "MaisonId_maison");

            migrationBuilder.CreateIndex(
                name: "IX_devis_details_TravauxId_travaux",
                table: "devis_details",
                column: "TravauxId_travaux");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "devis_details");
        }
    }
}
