using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace temp_back.Data.Migrations
{
    /// <inheritdoc />
    public partial class M9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code_travaux",
                table: "travaux",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ref_payement",
                table: "payements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Surface",
                table: "maisons",
                type: "numeric(20,2)",
                precision: 20,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Lieu",
                table: "devis",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ref_devis",
                table: "devis",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code_travaux",
                table: "travaux");

            migrationBuilder.DropColumn(
                name: "Ref_payement",
                table: "payements");

            migrationBuilder.DropColumn(
                name: "Surface",
                table: "maisons");

            migrationBuilder.DropColumn(
                name: "Lieu",
                table: "devis");

            migrationBuilder.DropColumn(
                name: "Ref_devis",
                table: "devis");
        }
    }
}
