using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Addexpdncolumnintrnclaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTotalExpenditureFilePdf",
                table: "trnEducationDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TotalExpenditureFilePdf",
                table: "trnEducationDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTotalExpenditureFilePdf",
                table: "trnEducationDetails");

            migrationBuilder.DropColumn(
                name: "TotalExpenditureFilePdf",
                table: "trnEducationDetails");
        }
    }
}
