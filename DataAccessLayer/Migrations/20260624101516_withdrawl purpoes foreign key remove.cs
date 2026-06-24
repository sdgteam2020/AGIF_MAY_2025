using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class withdrawlpurpoesforeignkeyremove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trnClaim_WithdrawalPurpose_WithdrawPurpose",
                table: "trnClaim");

            migrationBuilder.DropIndex(
                name: "IX_trnClaim_WithdrawPurpose",
                table: "trnClaim");

            migrationBuilder.CreateIndex(
                name: "IX_TrnApprovedLogs_CoProfileId",
                table: "TrnApprovedLogs",
                column: "CoProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnApprovedLogs_UserProfiles_CoProfileId",
                table: "TrnApprovedLogs",
                column: "CoProfileId",
                principalTable: "UserProfiles",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnApprovedLogs_UserProfiles_CoProfileId",
                table: "TrnApprovedLogs");

            migrationBuilder.DropIndex(
                name: "IX_TrnApprovedLogs_CoProfileId",
                table: "TrnApprovedLogs");

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
                onDelete: ReferentialAction.NoAction);
        }
    }
}
