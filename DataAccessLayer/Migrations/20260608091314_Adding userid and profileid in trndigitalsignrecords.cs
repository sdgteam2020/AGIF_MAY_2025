using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Addinguseridandprofileidintrndigitalsignrecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArmyNo",
                table: "trnDigitalSignRecords");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "trnDigitalSignRecords");

            migrationBuilder.DropColumn(
                name: "RankName",
                table: "trnDigitalSignRecords");

            migrationBuilder.AddColumn<int>(
                name: "ProfileId",
                table: "trnDigitalSignRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "trnDigitalSignRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "trnDigitalSignRecords");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "trnDigitalSignRecords");

            migrationBuilder.AddColumn<string>(
                name: "ArmyNo",
                table: "trnDigitalSignRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DomainId",
                table: "trnDigitalSignRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RankName",
                table: "trnDigitalSignRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
