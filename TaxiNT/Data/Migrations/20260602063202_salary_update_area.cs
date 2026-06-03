using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiNT.Data.Migrations
{
    /// <inheritdoc />
    public partial class salary_update_area : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "updatedAt",
                table: "SalaryDetails");

            migrationBuilder.DropColumn(
                name: "updatedAt",
                table: "Salaries");

            migrationBuilder.AddColumn<string>(
                name: "area",
                table: "SalaryDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "salaryDate",
                table: "SalaryDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "salaryDate",
                table: "Salaries",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "noteDeductOrder",
                table: "Salaries",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "area",
                table: "Salaries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "area",
                table: "SalaryDetails");

            migrationBuilder.DropColumn(
                name: "salaryDate",
                table: "SalaryDetails");

            migrationBuilder.DropColumn(
                name: "area",
                table: "Salaries");

            migrationBuilder.AddColumn<DateTime>(
                name: "updatedAt",
                table: "SalaryDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "salaryDate",
                table: "Salaries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "noteDeductOrder",
                table: "Salaries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updatedAt",
                table: "Salaries",
                type: "datetime2",
                nullable: true);
        }
    }
}
