using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Updatingtrnclaimdigitalsignrecordsforeignkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_trnClaimDigitalSignRecords_ProfileId",
                table: "trnClaimDigitalSignRecords",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_trnClaimDigitalSignRecords_UserId",
                table: "trnClaimDigitalSignRecords",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_trnClaimDigitalSignRecords_AspNetUsers_UserId",
                table: "trnClaimDigitalSignRecords",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_trnClaimDigitalSignRecords_UserProfiles_ProfileId",
                table: "trnClaimDigitalSignRecords",
                column: "ProfileId",
                principalTable: "UserProfiles",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trnClaimDigitalSignRecords_AspNetUsers_UserId",
                table: "trnClaimDigitalSignRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_trnClaimDigitalSignRecords_UserProfiles_ProfileId",
                table: "trnClaimDigitalSignRecords");

            migrationBuilder.DropIndex(
                name: "IX_trnClaimDigitalSignRecords_ProfileId",
                table: "trnClaimDigitalSignRecords");

            migrationBuilder.DropIndex(
                name: "IX_trnClaimDigitalSignRecords_UserId",
                table: "trnClaimDigitalSignRecords");
        }
    }
}
