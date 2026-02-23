using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class columnsAddedtrnclaimUpload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSeviceExtnPdf",
                table: "trnClaimDocumentUpload",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SeviceExtnPdf",
                table: "trnClaimDocumentUpload",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSeviceExtnPdf",
                table: "trnClaimDocumentUpload");

            migrationBuilder.DropColumn(
                name: "SeviceExtnPdf",
                table: "trnClaimDocumentUpload");
        }
    }
}
