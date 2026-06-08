using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Fixing_TrnCar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CA_Amt_Eligible_for_loan",
                table: "trnCar");

            migrationBuilder.DropColumn(
                name: "CA_EMI_Eligible",
                table: "trnCar");

            migrationBuilder.DropColumn(
                name: "CA_repayingCapacity",
                table: "trnCar");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CA_Amt_Eligible_for_loan",
                table: "trnCar",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CA_EMI_Eligible",
                table: "trnCar",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CA_repayingCapacity",
                table: "trnCar",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
