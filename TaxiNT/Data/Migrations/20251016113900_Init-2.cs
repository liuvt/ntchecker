using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiNT.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ctAmout",
                table: "Contracts",
                newName: "ctAmount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ctAmount",
                table: "Contracts",
                newName: "ctAmout");
        }
    }
}
