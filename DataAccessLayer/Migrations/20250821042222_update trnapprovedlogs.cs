using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class updatetrnapprovedlogs : Migration
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
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
