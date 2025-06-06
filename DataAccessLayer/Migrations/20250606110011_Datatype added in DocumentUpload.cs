using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class DatatypeaddedinDocumentUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCancelledCheque",
                table: "DocumentUpload",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDrivingLicensePdf",
                table: "DocumentUpload",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaySlipPdf",
                table: "DocumentUpload",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsQuotationPdf",
                table: "DocumentUpload",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSeviceExtnPdf",
                table: "DocumentUpload",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelledCheque",
                table: "DocumentUpload");

            migrationBuilder.DropColumn(
                name: "IsDrivingLicensePdf",
                table: "DocumentUpload");

            migrationBuilder.DropColumn(
                name: "IsPaySlipPdf",
                table: "DocumentUpload");

            migrationBuilder.DropColumn(
                name: "IsQuotationPdf",
                table: "DocumentUpload");

            migrationBuilder.DropColumn(
                name: "IsSeviceExtnPdf",
                table: "DocumentUpload");
        }
    }
}
