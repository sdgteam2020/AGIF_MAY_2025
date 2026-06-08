using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class FixingtrnClaimDocumentUploadtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachBonafideLetterPdf",
                table: "trnClaimDocumentUpload");

            migrationBuilder.DropColumn(
                name: "AttachInvitationcardPdf",
                table: "trnClaimDocumentUpload");

            migrationBuilder.DropColumn(
                name: "AttachPartIIOrderPdf",
                table: "trnClaimDocumentUpload");

            migrationBuilder.DropColumn(
                name: "Attach_PartIIOrderPdf",
                table: "trnClaimDocumentUpload");

            migrationBuilder.DropColumn(
                name: "CancelledCheque",
                table: "trnClaimDocumentUpload");

            migrationBuilder.DropColumn(
                name: "OtherReasonsPdf",
                table: "trnClaimDocumentUpload");

            migrationBuilder.DropColumn(
                name: "PaySlipPdf",
                table: "trnClaimDocumentUpload");

            migrationBuilder.DropColumn(
                name: "SeviceExtnPdf",
                table: "trnClaimDocumentUpload");

            migrationBuilder.DropColumn(
                name: "SplWaiverPdf",
                table: "trnClaimDocumentUpload");

            migrationBuilder.DropColumn(
                name: "TotalExpenditureFile",
                table: "trnClaimDocumentUpload");

            migrationBuilder.RenameColumn(
                name: "IsAttach_PartIIOrderPdf",
                table: "trnClaimDocumentUpload",
                newName: "IsAttach_PartIIOrderPdfMarr");

            migrationBuilder.RenameColumn(
                name: "IsAttachPartIIOrderPdf",
                table: "trnClaimDocumentUpload",
                newName: "IsAttachPartIIOrderPdfEdu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAttach_PartIIOrderPdfMarr",
                table: "trnClaimDocumentUpload",
                newName: "IsAttach_PartIIOrderPdf");

            migrationBuilder.RenameColumn(
                name: "IsAttachPartIIOrderPdfEdu",
                table: "trnClaimDocumentUpload",
                newName: "IsAttachPartIIOrderPdf");

            migrationBuilder.AddColumn<string>(
                name: "AttachBonafideLetterPdf",
                table: "trnClaimDocumentUpload",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachInvitationcardPdf",
                table: "trnClaimDocumentUpload",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachPartIIOrderPdf",
                table: "trnClaimDocumentUpload",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Attach_PartIIOrderPdf",
                table: "trnClaimDocumentUpload",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelledCheque",
                table: "trnClaimDocumentUpload",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherReasonsPdf",
                table: "trnClaimDocumentUpload",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaySlipPdf",
                table: "trnClaimDocumentUpload",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeviceExtnPdf",
                table: "trnClaimDocumentUpload",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SplWaiverPdf",
                table: "trnClaimDocumentUpload",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalExpenditureFile",
                table: "trnClaimDocumentUpload",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
