using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddressandAccountTablesadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "trnApplications");

            migrationBuilder.DropColumn(
                name: "ConfirmSalaryAcctNo",
                table: "trnApplications");

            migrationBuilder.DropColumn(
                name: "Distt",
                table: "trnApplications");

            migrationBuilder.DropColumn(
                name: "IfsCode",
                table: "trnApplications");

            migrationBuilder.DropColumn(
                name: "NameOfBank",
                table: "trnApplications");

            migrationBuilder.DropColumn(
                name: "NameOfBankBranch",
                table: "trnApplications");

            migrationBuilder.DropColumn(
                name: "PostOffice",
                table: "trnApplications");

            migrationBuilder.DropColumn(
                name: "SalaryAcctNo",
                table: "trnApplications");

            migrationBuilder.DropColumn(
                name: "State",
                table: "trnApplications");

            migrationBuilder.DropColumn(
                name: "Vill_Town",
                table: "trnApplications");

            migrationBuilder.CreateTable(
                name: "trnAccountDetails",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    SalaryAcctNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ConfirmSalaryAcctNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IfsCode = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    NameOfBank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameOfBankBranch = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trnAccountDetails", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_trnAccountDetails_trnApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "trnApplications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trnAddressDetails",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    Vill_Town = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PostOffice = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Distt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trnAddressDetails", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_trnAddressDetails_trnApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "trnApplications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trnAccountDetails_ApplicationId",
                table: "trnAccountDetails",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_trnAddressDetails_ApplicationId",
                table: "trnAddressDetails",
                column: "ApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trnAccountDetails");

            migrationBuilder.DropTable(
                name: "trnAddressDetails");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "trnApplications",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConfirmSalaryAcctNo",
                table: "trnApplications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Distt",
                table: "trnApplications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IfsCode",
                table: "trnApplications",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameOfBank",
                table: "trnApplications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameOfBankBranch",
                table: "trnApplications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostOffice",
                table: "trnApplications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SalaryAcctNo",
                table: "trnApplications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "trnApplications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Vill_Town",
                table: "trnApplications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
