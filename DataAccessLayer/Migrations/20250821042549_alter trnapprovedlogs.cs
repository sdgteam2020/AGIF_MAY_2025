using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class altertrnapprovedlogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "coCdrProfileId",
                table: "TrnApprovedLogs",
                newName: "coProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "coProfileId",
                table: "TrnApprovedLogs",
                newName: "coCdrProfileId");
        }
    }
}
