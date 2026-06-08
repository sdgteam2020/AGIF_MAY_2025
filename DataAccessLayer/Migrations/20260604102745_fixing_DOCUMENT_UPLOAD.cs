using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class fixing_DOCUMENT_UPLOAD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledCheque",
                table: "trnDocumentUpload");

            migrationBuilder.DropColumn(
                name: "DrivingLicensePdf",
                table: "trnDocumentUpload");

            migrationBuilder.DropColumn(
                name: "PaySlipPdf",
                table: "trnDocumentUpload");

            migrationBuilder.DropColumn(
                name: "QuotationPdf",
                table: "trnDocumentUpload");

            migrationBuilder.DropColumn(
                name: "SeviceExtnPdf",
                table: "trnDocumentUpload");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelledCheque",
                table: "trnDocumentUpload",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrivingLicensePdf",
                table: "trnDocumentUpload",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaySlipPdf",
                table: "trnDocumentUpload",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuotationPdf",
                table: "trnDocumentUpload",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeviceExtnPdf",
                table: "trnDocumentUpload",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
