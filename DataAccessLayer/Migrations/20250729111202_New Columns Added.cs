using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class NewColumnsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "ConfirmSalaryAcctNo",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "Distt",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "IfsCode",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "NameOfBank",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "NameOfBankBranch",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "PostOffice",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "SalaryAcctNo",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "State",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "Vill_Town",
                table: "trnClaim");

            migrationBuilder.AddColumn<string>(
                name: "AGIFRemarks",
                table: "trnApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "trnClaimAccountDetails",
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
                    table.PrimaryKey("PK_trnClaimAccountDetails", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_trnClaimAccountDetails_trnClaim_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "trnClaim",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trnClaimAddressDetails",
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
                    table.PrimaryKey("PK_trnClaimAddressDetails", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_trnClaimAddressDetails_trnClaim_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "trnClaim",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trnClaimAccountDetails_ApplicationId",
                table: "trnClaimAccountDetails",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_trnClaimAddressDetails_ApplicationId",
                table: "trnClaimAddressDetails",
                column: "ApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trnClaimAccountDetails");

            migrationBuilder.DropTable(
                name: "trnClaimAddressDetails");

            migrationBuilder.DropColumn(
                name: "AGIFRemarks",
                table: "trnApplications");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "trnClaim",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConfirmSalaryAcctNo",
                table: "trnClaim",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Distt",
                table: "trnClaim",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IfsCode",
                table: "trnClaim",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameOfBank",
                table: "trnClaim",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameOfBankBranch",
                table: "trnClaim",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostOffice",
                table: "trnClaim",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SalaryAcctNo",
                table: "trnClaim",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "trnClaim",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Vill_Town",
                table: "trnClaim",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
