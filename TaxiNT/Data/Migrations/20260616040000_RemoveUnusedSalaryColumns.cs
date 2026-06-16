using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiNT.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedSalaryColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "deductForCharging",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "deductForChargingPenalty",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "deductForDeposit",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "deductForNegativeSalary",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "deductForNegativeSalaryPartner",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "deductForOrder",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "deductForPIT",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "deductForSalaryAdvance",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "deductForSocialInsurance",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "deductForTollPayment",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "deductForViolationReport",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "haomon_voxe",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "no_sua_chua",
                table: "Salaries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "deductForCharging",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "deductForChargingPenalty",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "deductForDeposit",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "deductForNegativeSalary",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "deductForNegativeSalaryPartner",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "deductForOrder",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "deductForPIT",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "deductForSalaryAdvance",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "deductForSocialInsurance",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "deductForTollPayment",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "deductForViolationReport",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "haomon_voxe",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "no_sua_chua",
                table: "Salaries",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
