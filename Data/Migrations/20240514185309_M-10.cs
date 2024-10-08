using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace temp_back.Data.Migrations
{
    /// <inheritdoc />
    public partial class M10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Augmentation",
                table: "type_finitions",
                type: "numeric(20,2)",
                precision: 20,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Augmentation",
                table: "type_finitions",
                type: "double precision",
                precision: 0,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "numeric(20,2)",
                oldPrecision: 20,
                oldScale: 2);
        }
    }
}
