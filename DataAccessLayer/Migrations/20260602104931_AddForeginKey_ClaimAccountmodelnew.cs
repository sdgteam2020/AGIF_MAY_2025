using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddForeginKey_ClaimAccountmodelnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "trnClaimAccountDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_trnClaimAccountDetails_BankId",
                table: "trnClaimAccountDetails",
                column: "BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_trnClaimAccountDetails_MBank_BankId",
                table: "trnClaimAccountDetails",
                column: "BankId",
                principalTable: "MBank",
                principalColumn: "BankId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trnClaimAccountDetails_MBank_BankId",
                table: "trnClaimAccountDetails");

            migrationBuilder.DropIndex(
                name: "IX_trnClaimAccountDetails_BankId",
                table: "trnClaimAccountDetails");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "trnClaimAccountDetails");
        }
    }
}
