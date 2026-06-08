using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class fixing_car_hba_pca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PCA_Amt_Eligible_for_loan",
                table: "trnPCA");

            migrationBuilder.DropColumn(
                name: "PCA_EMI_Eligible",
                table: "trnPCA");

            migrationBuilder.DropColumn(
                name: "PCA_repayingCapacity",
                table: "trnPCA");

            migrationBuilder.DropColumn(
                name: "HBA_Amt_Eligible_for_loan",
                table: "trnHBA");

            migrationBuilder.DropColumn(
                name: "HBA_EMI_Eligible",
                table: "trnHBA");

            migrationBuilder.DropColumn(
                name: "HBA_repayingCapacity",
                table: "trnHBA");

            migrationBuilder.DropColumn(
                name: "ConfirmSalaryAcctNo",
                table: "trnAccountDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PCA_Amt_Eligible_for_loan",
                table: "trnPCA",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PCA_EMI_Eligible",
                table: "trnPCA",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PCA_repayingCapacity",
                table: "trnPCA",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HBA_Amt_Eligible_for_loan",
                table: "trnHBA",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HBA_EMI_Eligible",
                table: "trnHBA",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HBA_repayingCapacity",
                table: "trnHBA",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ConfirmSalaryAcctNo",
                table: "trnAccountDetails",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
