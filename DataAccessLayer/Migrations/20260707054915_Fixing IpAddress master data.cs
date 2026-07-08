using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class FixingIpAddressmasterdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "TrnLoginLogs");

            migrationBuilder.DropColumn(
                name: "ipAddress",
                table: "trnDigitalSignRecords");

            migrationBuilder.DropColumn(
                name: "ipAddress",
                table: "trnClaimDigitalSignRecords");

            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "ipAddress",
                table: "trnApplications");

            migrationBuilder.AddColumn<int>(
                name: "IpAddressId",
                table: "TrnLoginLogs",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "IpAddressId",
                table: "trnDigitalSignRecords",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "IpAddressId",
                table: "trnClaimDigitalSignRecords",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "IpAddressId",
                table: "trnClaim",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "IpAddressId",
                table: "trnApplications",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.CreateIndex(
                name: "IX_TrnLoginLogs_IpAddressId",
                table: "TrnLoginLogs",
                column: "IpAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_trnDigitalSignRecords_IpAddressId",
                table: "trnDigitalSignRecords",
                column: "IpAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_trnClaimDigitalSignRecords_IpAddressId",
                table: "trnClaimDigitalSignRecords",
                column: "IpAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_trnClaim_IpAddressId",
                table: "trnClaim",
                column: "IpAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_trnApplications_IpAddressId",
                table: "trnApplications",
                column: "IpAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_trnApplications_MIpAddresses_IpAddressId",
                table: "trnApplications",
                column: "IpAddressId",
                principalTable: "MIpAddresses",
                principalColumn: "IpAddressId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_trnClaim_MIpAddresses_IpAddressId",
                table: "trnClaim",
                column: "IpAddressId",
                principalTable: "MIpAddresses",
                principalColumn: "IpAddressId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_trnClaimDigitalSignRecords_MIpAddresses_IpAddressId",
                table: "trnClaimDigitalSignRecords",
                column: "IpAddressId",
                principalTable: "MIpAddresses",
                principalColumn: "IpAddressId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_trnDigitalSignRecords_MIpAddresses_IpAddressId",
                table: "trnDigitalSignRecords",
                column: "IpAddressId",
                principalTable: "MIpAddresses",
                principalColumn: "IpAddressId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnLoginLogs_MIpAddresses_IpAddressId",
                table: "TrnLoginLogs",
                column: "IpAddressId",
                principalTable: "MIpAddresses",
                principalColumn: "IpAddressId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trnApplications_MIpAddresses_IpAddressId",
                table: "trnApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_trnClaim_MIpAddresses_IpAddressId",
                table: "trnClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_trnClaimDigitalSignRecords_MIpAddresses_IpAddressId",
                table: "trnClaimDigitalSignRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_trnDigitalSignRecords_MIpAddresses_IpAddressId",
                table: "trnDigitalSignRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnLoginLogs_MIpAddresses_IpAddressId",
                table: "TrnLoginLogs");

            migrationBuilder.DropIndex(
                name: "IX_TrnLoginLogs_IpAddressId",
                table: "TrnLoginLogs");

            migrationBuilder.DropIndex(
                name: "IX_trnDigitalSignRecords_IpAddressId",
                table: "trnDigitalSignRecords");

            migrationBuilder.DropIndex(
                name: "IX_trnClaimDigitalSignRecords_IpAddressId",
                table: "trnClaimDigitalSignRecords");

            migrationBuilder.DropIndex(
                name: "IX_trnClaim_IpAddressId",
                table: "trnClaim");

            migrationBuilder.DropIndex(
                name: "IX_trnApplications_IpAddressId",
                table: "trnApplications");

            migrationBuilder.DropColumn(
                name: "IpAddressId",
                table: "TrnLoginLogs");

            migrationBuilder.DropColumn(
                name: "IpAddressId",
                table: "trnDigitalSignRecords");

            migrationBuilder.DropColumn(
                name: "IpAddressId",
                table: "trnClaimDigitalSignRecords");

            migrationBuilder.DropColumn(
                name: "IpAddressId",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "IpAddressId",
                table: "trnApplications");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "TrnLoginLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ipAddress",
                table: "trnDigitalSignRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ipAddress",
                table: "trnClaimDigitalSignRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "trnClaim",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ipAddress",
                table: "trnApplications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
