using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiNT.Data.Migrations
{
    /// <inheritdoc />
    public partial class factorycode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Area",
                table: "ShiftWorks",
                newName: "area");

            migrationBuilder.RenameColumn(
                name: "SauMucAnChia",
                table: "ShiftWorks",
                newName: "basicSalary");

            migrationBuilder.RenameColumn(
                name: "Rank",
                table: "ShiftWorks",
                newName: "ranking");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "area",
                table: "ShiftWorks",
                newName: "Area");

            migrationBuilder.RenameColumn(
                name: "ranking",
                table: "ShiftWorks",
                newName: "Rank");

            migrationBuilder.RenameColumn(
                name: "basicSalary",
                table: "ShiftWorks",
                newName: "SauMucAnChia");
        }
    }
}
