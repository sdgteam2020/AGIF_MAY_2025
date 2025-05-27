using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MRanks_AspNetUsers_Updatedby",
                table: "MRanks");

            migrationBuilder.DropIndex(
                name: "IX_MRanks_Updatedby",
                table: "MRanks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MRanks_Updatedby",
                table: "MRanks",
                column: "Updatedby");

            migrationBuilder.AddForeignKey(
                name: "FK_MRanks_AspNetUsers_Updatedby",
                table: "MRanks",
                column: "Updatedby",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
