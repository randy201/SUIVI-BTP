using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace temp_back.Data.Migrations
{
    /// <inheritdoc />
    public partial class M4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_devis_maisons_Type_maisonId_maison",
                table: "devis");

            migrationBuilder.RenameColumn(
                name: "Type_maisonId_maison",
                table: "devis",
                newName: "MaisonId_maison");

            migrationBuilder.RenameIndex(
                name: "IX_devis_Type_maisonId_maison",
                table: "devis",
                newName: "IX_devis_MaisonId_maison");

            migrationBuilder.AddForeignKey(
                name: "FK_devis_maisons_MaisonId_maison",
                table: "devis",
                column: "MaisonId_maison",
                principalTable: "maisons",
                principalColumn: "Id_maison",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_devis_maisons_MaisonId_maison",
                table: "devis");

            migrationBuilder.RenameColumn(
                name: "MaisonId_maison",
                table: "devis",
                newName: "Type_maisonId_maison");

            migrationBuilder.RenameIndex(
                name: "IX_devis_MaisonId_maison",
                table: "devis",
                newName: "IX_devis_Type_maisonId_maison");

            migrationBuilder.AddForeignKey(
                name: "FK_devis_maisons_Type_maisonId_maison",
                table: "devis",
                column: "Type_maisonId_maison",
                principalTable: "maisons",
                principalColumn: "Id_maison",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
