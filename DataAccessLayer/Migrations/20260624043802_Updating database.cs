using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Updatingdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "trnPropertyRenovation");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "trnMarriageward");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "trnEducationDetails");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "trnDocumentUpload");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "trnClaimDocumentUpload");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "trnClaimAddressDetails");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "trnClaimAccountDetails");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "trnAddressDetails");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "trnAccountDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "trnPropertyRenovation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "trnMarriageward",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "trnEducationDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "trnDocumentUpload",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "trnClaimDocumentUpload",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "trnClaimAddressDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "trnClaimAccountDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "trnAddressDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "trnAccountDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
