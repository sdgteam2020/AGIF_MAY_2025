using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Fixing_TrnApprovedLogs_Columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "TrnApprovedLogs");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "TrnApprovedLogs");

            migrationBuilder.DropColumn(
                name: "coDomainId",
                table: "TrnApprovedLogs");

            migrationBuilder.AddColumn<int>(
                name: "AdminProfileId",
                table: "TrnApprovedLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminProfileId",
                table: "TrnApprovedLogs");

            migrationBuilder.AddColumn<string>(
                name: "DomainId",
                table: "TrnApprovedLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TrnApprovedLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "coDomainId",
                table: "TrnApprovedLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
