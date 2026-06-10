using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiNT.Data.Migrations
{
    /// <inheritdoc />
    public partial class SalaryDeduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeductCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalaryDeductDetails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SalaryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeductCategoryId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryDeductDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalaryDeductDetails_DeductCategories_DeductCategoryId",
                        column: x => x.DeductCategoryId,
                        principalTable: "DeductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalaryDeductDetails_Salaries_SalaryId",
                        column: x => x.SalaryId,
                        principalTable: "Salaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalaryDeductDetails_DeductCategoryId",
                table: "SalaryDeductDetails",
                column: "DeductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryDeductDetails_SalaryId",
                table: "SalaryDeductDetails",
                column: "SalaryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalaryDeductDetails");

            migrationBuilder.DropTable(
                name: "DeductCategories");
        }
    }
}
