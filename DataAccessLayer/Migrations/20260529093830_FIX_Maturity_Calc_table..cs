using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class FIX_Maturity_Calc_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "TrnInvestmentChange_Officers");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "TrnInvestmentChange_JCO_OR");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "TrnBonusOfficers");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "TrnBonusJCO_OR");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "TrnInvestmentChange_Officers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "TrnInvestmentChange_JCO_OR",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "TrnBonusOfficers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "TrnBonusJCO_OR",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
