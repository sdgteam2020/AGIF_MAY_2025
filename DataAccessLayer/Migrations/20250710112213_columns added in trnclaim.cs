using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class columnsaddedintrnclaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "trnClaim",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Distt",
                table: "trnClaim",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostOffice",
                table: "trnClaim",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "trnClaim",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Vill_Town",
                table: "trnClaim",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "Distt",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "PostOffice",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "State",
                table: "trnClaim");

            migrationBuilder.DropColumn(
                name: "Vill_Town",
                table: "trnClaim");
        }
    }
}
