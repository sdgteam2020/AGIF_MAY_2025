using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Updatingtrnclaimdigitalsignrecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArmyNo",
                table: "trnClaimDigitalSignRecords");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "trnClaimDigitalSignRecords");

            migrationBuilder.DropColumn(
                name: "RankName",
                table: "trnClaimDigitalSignRecords");

            migrationBuilder.AddColumn<int>(
                name: "ProfileId",
                table: "trnClaimDigitalSignRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "trnClaimDigitalSignRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "trnClaimDigitalSignRecords");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "trnClaimDigitalSignRecords");

            migrationBuilder.AddColumn<string>(
                name: "ArmyNo",
                table: "trnClaimDigitalSignRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DomainId",
                table: "trnClaimDigitalSignRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RankName",
                table: "trnClaimDigitalSignRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
