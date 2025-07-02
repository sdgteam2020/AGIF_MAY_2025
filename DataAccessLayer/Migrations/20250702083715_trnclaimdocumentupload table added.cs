using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class trnclaimdocumentuploadtableadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PresentlyStudyingIn",
                table: "trnEducationDetails");

            migrationBuilder.AddColumn<bool>(
                name: "IsOtherReasonPdf",
                table: "trnSplWaiver",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OtherReasonsPdf",
                table: "trnSplWaiver",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalExpenditureFilePdf",
                table: "trnPropertyRenovation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachInvitationcardPdf",
                table: "trnMarriageward",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachPartIIOrderPdf",
                table: "trnMarriageward",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachBonafideLetterPdf",
                table: "trnEducationDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachPartIIOrderPdf",
                table: "trnEducationDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "trnClaimDocumentUpload",
                columns: table => new
                {
                    UploadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    AttachBonafideLetterPdf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachPartIIOrderPdf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAttachBonafideLetterPdf = table.Column<bool>(type: "bit", nullable: false),
                    IsAttachPartIIOrderPdf = table.Column<bool>(type: "bit", nullable: false),
                    Attach_PartIIOrderPdf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttachInvitationcardPdf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAttachInvitationcardPdf = table.Column<bool>(type: "bit", nullable: false),
                    IsAttach_PartIIOrderPdf = table.Column<bool>(type: "bit", nullable: false),
                    TotalExpenditureFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsTotalExpenditureFilePdf = table.Column<bool>(type: "bit", nullable: false),
                    OtherReasonsPdf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsOtherReasonPdf = table.Column<bool>(type: "bit", nullable: false),
                    CancelledCheque = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaySlipPdf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SplWaiverPdf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCancelledChequePdf = table.Column<bool>(type: "bit", nullable: false),
                    IsPaySlipPdf = table.Column<bool>(type: "bit", nullable: false),
                    IsSplWaiverPdf = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trnClaimDocumentUpload", x => x.UploadId);
                    table.ForeignKey(
                        name: "FK_trnClaimDocumentUpload_trnClaim_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "trnClaim",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trnClaimDocumentUpload_ApplicationId",
                table: "trnClaimDocumentUpload",
                column: "ApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trnClaimDocumentUpload");

            migrationBuilder.DropColumn(
                name: "IsOtherReasonPdf",
                table: "trnSplWaiver");

            migrationBuilder.DropColumn(
                name: "OtherReasonsPdf",
                table: "trnSplWaiver");

            migrationBuilder.DropColumn(
                name: "TotalExpenditureFilePdf",
                table: "trnPropertyRenovation");

            migrationBuilder.DropColumn(
                name: "AttachInvitationcardPdf",
                table: "trnMarriageward");

            migrationBuilder.DropColumn(
                name: "AttachPartIIOrderPdf",
                table: "trnMarriageward");

            migrationBuilder.DropColumn(
                name: "AttachBonafideLetterPdf",
                table: "trnEducationDetails");

            migrationBuilder.DropColumn(
                name: "AttachPartIIOrderPdf",
                table: "trnEducationDetails");

            migrationBuilder.AddColumn<string>(
                name: "PresentlyStudyingIn",
                table: "trnEducationDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
