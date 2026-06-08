using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddingForeignKeyintrnApprovedLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TrnApprovedLogs_UserId",
                table: "TrnApprovedLogs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrnApprovedLogs_AspNetUsers_UserId",
                table: "TrnApprovedLogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnApprovedLogs_AspNetUsers_UserId",
                table: "TrnApprovedLogs");

            migrationBuilder.DropIndex(
                name: "IX_TrnApprovedLogs_UserId",
                table: "TrnApprovedLogs");
        }
    }
}
