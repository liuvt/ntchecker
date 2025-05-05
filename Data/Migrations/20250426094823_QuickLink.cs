using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ntchecker.Data.Migrations
{
    /// <inheritdoc />
    public partial class QuickLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuickLinks",
                columns: table => new
                {
                    ql_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ql_Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ql_BankId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ql_AccountNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ql_template = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ql_amount = table.Column<double>(type: "float", nullable: true),
                    ql_description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ql_AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activity = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuickLinks", x => x.ql_Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuickLinks");
        }
    }
}
