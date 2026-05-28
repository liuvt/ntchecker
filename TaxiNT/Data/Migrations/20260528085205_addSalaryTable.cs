using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaxiNT.Data.Migrations
{
    /// <inheritdoc />
    public partial class addSalaryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Salaries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    userId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    revenue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tripsTotal = table.Column<int>(type: "int", nullable: false),
                    salaryType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    businessDays = table.Column<int>(type: "int", nullable: false),
                    salaryBase = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deductTotal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    salaryNet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deductForDeposit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deductForSalaryAdvance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deductForNegativeSalary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deductForViolationReport = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    no_sua_chua = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    haomon_voxe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deductForCharging = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deductForChargingPenalty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deductForTollPayment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deductForSocialInsurance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deductForNegativeSalaryPartner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deductForPIT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    deductForOrder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    noteDeductOrder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    salaryDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salaries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Salaries");
        }
    }
}
