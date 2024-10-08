using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace temp_back.Data.Migrations
{
    /// <inheritdoc />
    public partial class M5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date_Fin",
                table: "devis",
                newName: "Date_fin");

            migrationBuilder.AddColumn<double>(
                name: "Augmentation",
                table: "devis",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "MaisonId_maison",
                table: "detail_maisons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_detail_maisons_MaisonId_maison",
                table: "detail_maisons",
                column: "MaisonId_maison");

            migrationBuilder.AddForeignKey(
                name: "FK_detail_maisons_maisons_MaisonId_maison",
                table: "detail_maisons",
                column: "MaisonId_maison",
                principalTable: "maisons",
                principalColumn: "Id_maison",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_detail_maisons_maisons_MaisonId_maison",
                table: "detail_maisons");

            migrationBuilder.DropIndex(
                name: "IX_detail_maisons_MaisonId_maison",
                table: "detail_maisons");

            migrationBuilder.DropColumn(
                name: "Augmentation",
                table: "devis");

            migrationBuilder.DropColumn(
                name: "MaisonId_maison",
                table: "detail_maisons");

            migrationBuilder.RenameColumn(
                name: "Date_fin",
                table: "devis",
                newName: "Date_Fin");
        }
    }
}
