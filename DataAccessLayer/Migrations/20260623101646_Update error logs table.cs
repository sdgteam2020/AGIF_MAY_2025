using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Updateerrorlogstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmSalaryAcctNo",
                table: "trnClaimAccountDetails");

            migrationBuilder.DropColumn(
                name: "IsVerify",
                table: "MUnits");

            migrationBuilder.DropColumn(
                name: "AppointmentAbbreviation",
                table: "MAppointments");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "ErrorLogs");

            migrationBuilder.RenameColumn(
                name: "StackTrace",
                table: "ErrorLogs",
                newName: "ErrorDetail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ErrorDetail",
                table: "ErrorLogs",
                newName: "StackTrace");

            migrationBuilder.AddColumn<string>(
                name: "ConfirmSalaryAcctNo",
                table: "trnClaimAccountDetails",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerify",
                table: "MUnits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AppointmentAbbreviation",
                table: "MAppointments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "ErrorLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "ErrorLogs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
