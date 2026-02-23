using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class updatetrnapprovedlogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitCdrProfileId",
                table: "TrnApprovedLogs",
                newName: "coCdrProfileId");

            migrationBuilder.RenameColumn(
                name: "UnitCdrDomainId",
                table: "TrnApprovedLogs",
                newName: "coDomainId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "coDomainId",
                table: "TrnApprovedLogs",
                newName: "UnitCdrDomainId");

            migrationBuilder.RenameColumn(
                name: "coCdrProfileId",
                table: "TrnApprovedLogs",
                newName: "UnitCdrProfileId");
        }
    }
}
