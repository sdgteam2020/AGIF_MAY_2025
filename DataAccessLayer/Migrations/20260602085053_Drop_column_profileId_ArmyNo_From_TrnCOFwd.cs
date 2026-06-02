using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Drop_column_profileId_ArmyNo_From_TrnCOFwd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArmyNo",
                table: "TrnFwdCO");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "TrnFwdCO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArmyNo",
                table: "TrnFwdCO",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProfileId",
                table: "TrnFwdCO",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
