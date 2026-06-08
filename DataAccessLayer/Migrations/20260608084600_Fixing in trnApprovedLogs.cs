using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class FixingintrnApprovedLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrnApprovedLogs_AspNetUsers_UserId",
                table: "TrnApprovedLogs");

            migrationBuilder.DropIndex(
                name: "IX_TrnApprovedLogs_UserId",
                table: "TrnApprovedLogs");

            migrationBuilder.RenameColumn(
                name: "coProfileId",
                table: "TrnApprovedLogs",
                newName: "CoProfileId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TrnApprovedLogs",
                newName: "CoUserId");

            migrationBuilder.AddColumn<int>(
                name: "AdminUserId",
                table: "TrnApprovedLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminUserId",
                table: "TrnApprovedLogs");

            migrationBuilder.RenameColumn(
                name: "CoProfileId",
                table: "TrnApprovedLogs",
                newName: "coProfileId");

            migrationBuilder.RenameColumn(
                name: "CoUserId",
                table: "TrnApprovedLogs",
                newName: "UserId");

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
    }
}
