using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class TestFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trnClaim_MUnits_MUnitsPresentUnitId",
                table: "trnClaim");

            migrationBuilder.DropIndex(
                name: "IX_trnClaim_MUnitsPresentUnitId",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "MUnitsPresentUnitId",
                table: "trnClaim");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MUnitsPresentUnitId",
                table: "trnClaim",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_trnClaim_MUnitsPresentUnitId",
                table: "trnClaim",
                column: "MUnitsPresentUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_trnClaim_MUnits_MUnitsPresentUnitId",
                table: "trnClaim",
                column: "MUnitsPresentUnitId",
                principalTable: "MUnits",
                principalColumn: "UnitId");
        }
    }
}
