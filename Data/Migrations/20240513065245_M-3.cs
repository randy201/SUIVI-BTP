using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace temp_back.Data.Migrations
{
    /// <inheritdoc />
    public partial class M3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "maisons",
                columns: table => new
                {
                    Id_maison = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    Prix_total = table.Column<double>(type: "double precision", nullable: false),
                    Durrer_totale = table.Column<double>(type: "double precision", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_maisons", x => x.Id_maison);
                });

            migrationBuilder.CreateTable(
                name: "type_finitions",
                columns: table => new
                {
                    Id_type_finition = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    Augmentation = table.Column<double>(type: "double precision", precision: 0, nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_type_finitions", x => x.Id_type_finition);
                });

            migrationBuilder.CreateTable(
                name: "unites",
                columns: table => new
                {
                    Id_unite = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unites", x => x.Id_unite);
                });

            migrationBuilder.CreateTable(
                name: "devis",
                columns: table => new
                {
                    Id_devis = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UtilisateurId_utilisateur = table.Column<int>(type: "integer", nullable: false),
                    Type_maisonId_maison = table.Column<int>(type: "integer", nullable: false),
                    Date_devis = table.Column<DateOnly>(type: "date", nullable: false),
                    Date_debut = table.Column<DateOnly>(type: "date", nullable: false),
                    Heurre_debut = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Date_Fin = table.Column<DateOnly>(type: "date", nullable: false),
                    Heurre_fin = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Type_finitionId_type_finition = table.Column<int>(type: "integer", nullable: false),
                    Prix_total = table.Column<double>(type: "numeric(20,2)", nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devis", x => x.Id_devis);
                    table.ForeignKey(
                        name: "FK_devis_maisons_Type_maisonId_maison",
                        column: x => x.Type_maisonId_maison,
                        principalTable: "maisons",
                        principalColumn: "Id_maison",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_devis_type_finitions_Type_finitionId_type_finition",
                        column: x => x.Type_finitionId_type_finition,
                        principalTable: "type_finitions",
                        principalColumn: "Id_type_finition",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_devis_utilisateurs_UtilisateurId_utilisateur",
                        column: x => x.UtilisateurId_utilisateur,
                        principalTable: "utilisateurs",
                        principalColumn: "Id_utilisateur",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "travaux",
                columns: table => new
                {
                    Id_travaux = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    Prix_unitaire = table.Column<double>(type: "numeric(20,2)", nullable: false),
                    UniteId_unite = table.Column<int>(type: "integer", nullable: false),
                    Durree = table.Column<int>(type: "integer", nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_travaux", x => x.Id_travaux);
                    table.ForeignKey(
                        name: "FK_travaux_unites_UniteId_unite",
                        column: x => x.UniteId_unite,
                        principalTable: "unites",
                        principalColumn: "Id_unite",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "detail_maisons",
                columns: table => new
                {
                    Id_detail_maison = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TravauxId_travaux = table.Column<int>(type: "integer", nullable: false),
                    Quantite = table.Column<double>(type: "double precision", nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detail_maisons", x => x.Id_detail_maison);
                    table.ForeignKey(
                        name: "FK_detail_maisons_travaux_TravauxId_travaux",
                        column: x => x.TravauxId_travaux,
                        principalTable: "travaux",
                        principalColumn: "Id_travaux",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_detail_maisons_TravauxId_travaux",
                table: "detail_maisons",
                column: "TravauxId_travaux");

            migrationBuilder.CreateIndex(
                name: "IX_devis_Type_finitionId_type_finition",
                table: "devis",
                column: "Type_finitionId_type_finition");

            migrationBuilder.CreateIndex(
                name: "IX_devis_Type_maisonId_maison",
                table: "devis",
                column: "Type_maisonId_maison");

            migrationBuilder.CreateIndex(
                name: "IX_devis_UtilisateurId_utilisateur",
                table: "devis",
                column: "UtilisateurId_utilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_travaux_UniteId_unite",
                table: "travaux",
                column: "UniteId_unite");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detail_maisons");

            migrationBuilder.DropTable(
                name: "devis");

            migrationBuilder.DropTable(
                name: "travaux");

            migrationBuilder.DropTable(
                name: "maisons");

            migrationBuilder.DropTable(
                name: "type_finitions");

            migrationBuilder.DropTable(
                name: "unites");
        }
    }
}
