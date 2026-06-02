using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Adding_ForeignKey_BankId_in_AccountDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_trnAccountDetails_BankId",
                table: "trnAccountDetails",
                column: "BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_trnAccountDetails_MBank_BankId",
                table: "trnAccountDetails",
                column: "BankId",
                principalTable: "MBank",
                principalColumn: "BankId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trnAccountDetails_MBank_BankId",
                table: "trnAccountDetails");

            migrationBuilder.DropIndex(
                name: "IX_trnAccountDetails_BankId",
                table: "trnAccountDetails");
        }
    }
}
