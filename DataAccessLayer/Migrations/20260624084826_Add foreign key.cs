using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Addforeignkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_trnAddressDetails_Distt",
                table: "trnAddressDetails",
                column: "Distt");

            migrationBuilder.CreateIndex(
                name: "IX_trnAddressDetails_State",
                table: "trnAddressDetails",
                column: "State");

            migrationBuilder.AddForeignKey(
                name: "FK_trnAddressDetails_MDist_Distt",
                table: "trnAddressDetails",
                column: "Distt",
                principalTable: "MDist",
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_trnAddressDetails_MState_State",
                table: "trnAddressDetails",
                column: "State",
                principalTable: "MState",
                principalColumn: "StateId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trnAddressDetails_MDist_Distt",
                table: "trnAddressDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_trnAddressDetails_MState_State",
                table: "trnAddressDetails");

            migrationBuilder.DropIndex(
                name: "IX_trnAddressDetails_Distt",
                table: "trnAddressDetails");

            migrationBuilder.DropIndex(
                name: "IX_trnAddressDetails_State",
                table: "trnAddressDetails");
        }
    }
}
