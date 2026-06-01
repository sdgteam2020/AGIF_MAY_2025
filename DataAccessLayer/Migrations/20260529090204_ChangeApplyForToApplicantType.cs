using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeApplyForToApplicantType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MRanks_MApplyFor_ApplyForId",
                table: "MRanks");

            migrationBuilder.DropTable(
                name: "MApplyFor");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "trnUserMappings");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "trnPropertyRenovation");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "trnPCA");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "trnMarriageward");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "trnHBA");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "trnEducationDetails");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "trnDocumentUpload");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "trnClaimDocumentUpload");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "trnClaimAddressDetails");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "trnClaimAccountDetails");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "trnCar");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "trnApplications");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "trnAddressDetails");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "trnAccountDetails");

            migrationBuilder.DropColumn(
                name: "RankAbbreviation",
                table: "MRanks");

            migrationBuilder.DropColumn(
                name: "RetirementAge",
                table: "MRanks");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "MRanks");

            migrationBuilder.DropColumn(
                name: "rank_cd",
                table: "MRanks");

            migrationBuilder.DropColumn(
                name: "Updatedby",
                table: "MAppointments");

            migrationBuilder.AlterColumn<bool>(
                name: "PrematureRetirement",
                table: "trnClaim",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsMergePdf",
                table: "trnClaim",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "ApplyForId",
                table: "MRanks",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AddForeignKey(
                name: "FK_MRanks_MApplicantTypes_ApplyForId",
                table: "MRanks",
                column: "ApplyForId",
                principalTable: "MApplicantTypes",
                principalColumn: "ApplicantTypeId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MRanks_MApplicantTypes_ApplyForId",
                table: "MRanks");

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "UserProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "trnUserMappings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "trnPropertyRenovation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "trnPCA",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "trnMarriageward",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "trnHBA",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "trnEducationDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "trnDocumentUpload",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "trnClaimDocumentUpload",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "trnClaimAddressDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "trnClaimAccountDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "PrematureRetirement",
                table: "trnClaim",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsMergePdf",
                table: "trnClaim",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "trnClaim",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "trnCar",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "trnApplications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "trnAddressDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "trnAccountDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "ApplyForId",
                table: "MRanks",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "RankAbbreviation",
                table: "MRanks",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RetirementAge",
                table: "MRanks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "MRanks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rank_cd",
                table: "MRanks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Updatedby",
                table: "MAppointments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MApplyFor",
                columns: table => new
                {
                    ApplyForId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MApplyFor", x => x.ApplyForId);
                });

            
            migrationBuilder.AddForeignKey(
                name: "FK_MRanks_MApplyFor_ApplyForId",
                table: "MRanks",
                column: "ApplyForId",
                principalTable: "MApplyFor",
                principalColumn: "ApplyForId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
