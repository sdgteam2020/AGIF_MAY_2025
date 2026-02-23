using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class Addingnewcolumnsintrnapprovedlogs : Migration
    {
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
