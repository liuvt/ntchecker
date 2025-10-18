using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiNT.Data.Migrations
{
    /// <inheritdoc />
    public partial class blogforSEO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Blogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Blogs",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Blogs");
        }
    }
}
