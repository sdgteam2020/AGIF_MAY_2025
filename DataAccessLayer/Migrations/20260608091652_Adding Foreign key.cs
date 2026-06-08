using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddingForeignkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_trnDigitalSignRecords_ProfileId",
                table: "trnDigitalSignRecords",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_trnDigitalSignRecords_UserId",
                table: "trnDigitalSignRecords",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_trnDigitalSignRecords_AspNetUsers_UserId",
                table: "trnDigitalSignRecords",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_trnDigitalSignRecords_UserProfiles_ProfileId",
                table: "trnDigitalSignRecords",
                column: "ProfileId",
                principalTable: "UserProfiles",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trnDigitalSignRecords_AspNetUsers_UserId",
                table: "trnDigitalSignRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_trnDigitalSignRecords_UserProfiles_ProfileId",
                table: "trnDigitalSignRecords");

            migrationBuilder.DropIndex(
                name: "IX_trnDigitalSignRecords_ProfileId",
                table: "trnDigitalSignRecords");

            migrationBuilder.DropIndex(
                name: "IX_trnDigitalSignRecords_UserId",
                table: "trnDigitalSignRecords");
        }
    }
}
