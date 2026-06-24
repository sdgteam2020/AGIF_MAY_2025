using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class withdrawlpurpoesforeignkeyAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_trnClaim_WithdrawPurpose",
                table: "trnClaim",
                column: "WithdrawPurpose");

            migrationBuilder.AddForeignKey(
                name: "FK_trnClaim_WithdrawalPurpose_WithdrawPurpose",
                table: "trnClaim",
                column: "WithdrawPurpose",
                principalTable: "WithdrawalPurpose",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trnClaim_WithdrawalPurpose_WithdrawPurpose",
                table: "trnClaim");

            migrationBuilder.DropIndex(
                name: "IX_trnClaim_WithdrawPurpose",
                table: "trnClaim");
        }
    }
}
