using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiNT.Data.Migrations
{
    /// <inheritdoc />
    public partial class PQ_gasmoney_pre_postpaid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "basicSalary",
                table: "ShiftWorks");

            migrationBuilder.AddColumn<decimal>(
                name: "gasmoney",
                table: "ShiftWorks",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "pre_postpaid",
                table: "ShiftWorks",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "gasmoney",
                table: "ShiftWorks");

            migrationBuilder.DropColumn(
                name: "pre_postpaid",
                table: "ShiftWorks");

            migrationBuilder.AddColumn<decimal>(
                name: "basicSalary",
                table: "ShiftWorks",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
