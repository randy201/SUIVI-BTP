using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace temp_back.Data.Migrations
{
    /// <inheritdoc />
    public partial class M1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "all_types",
                columns: table => new
                {
                    Id_all_type = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    String_value = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Double_value = table.Column<double>(type: "double precision", precision: 30, scale: 2, nullable: true),
                    Int_value = table.Column<int>(type: "integer", nullable: false),
                    DateOnly_value = table.Column<DateOnly>(type: "date", nullable: false),
                    DateTime_value = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TimeOnly_value = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    File_path = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_all_types", x => x.Id_all_type);
                });

            migrationBuilder.CreateTable(
                name: "profils",
                columns: table => new
                {
                    Id_profil = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profils", x => x.Id_profil);
                });

            migrationBuilder.CreateTable(
                name: "utilisateurs",
                columns: table => new
                {
                    Id_utilisateur = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "text", nullable: true),
                    Prenom = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text unique", nullable: true),
                    Mot_de_passe = table.Column<string>(type: "text", nullable: true),
                    ProfilId_profil = table.Column<int>(type: "integer", nullable: false),
                    Telephone = table.Column<string>(type: "text unique", nullable: false),
                    Statut = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_utilisateurs", x => x.Id_utilisateur);
                    table.ForeignKey(
                        name: "FK_utilisateurs_profils_ProfilId_profil",
                        column: x => x.ProfilId_profil,
                        principalTable: "profils",
                        principalColumn: "Id_profil",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_utilisateurs_ProfilId_profil",
                table: "utilisateurs",
                column: "ProfilId_profil");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "all_types");

            migrationBuilder.DropTable(
                name: "utilisateurs");

            migrationBuilder.DropTable(
                name: "profils");
        }
    }
}
