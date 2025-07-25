using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Addexpdncolumnintrnspl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTotalExpenditureFilePdf",
                table: "trnSplWaiver",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TotalExpenditureFilePdf",
                table: "trnSplWaiver",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTotalExpenditureFilePdf",
                table: "trnSplWaiver");

            migrationBuilder.DropColumn(
                name: "TotalExpenditureFilePdf",
                table: "trnSplWaiver");
        }
    }
}
