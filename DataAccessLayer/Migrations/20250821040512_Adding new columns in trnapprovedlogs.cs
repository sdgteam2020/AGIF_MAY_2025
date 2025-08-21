using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Addingnewcolumnsintrnapprovedlogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnitCdrDomainId",
                table: "TrnApprovedLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UnitCdrProfileId",
                table: "TrnApprovedLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitCdrDomainId",
                table: "TrnApprovedLogs");

            migrationBuilder.DropColumn(
                name: "UnitCdrProfileId",
                table: "TrnApprovedLogs");
        }
    }
}
