using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Creating_Master_for_ExceptionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExceptionType",
                table: "ErrorLogs");

            migrationBuilder.AddColumn<int>(
                name: "ExceptionTypeId",
                table: "ErrorLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MExceptionTypes",
                columns: table => new
                {
                    ExceptionTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExceptionTypeName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MExceptionTypes", x => x.ExceptionTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_ExceptionTypeId",
                table: "ErrorLogs",
                column: "ExceptionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MExceptionTypes_ExceptionTypeName",
                table: "MExceptionTypes",
                column: "ExceptionTypeName",
                unique: true,
                filter: "[ExceptionTypeName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorLogs_MExceptionTypes_ExceptionTypeId",
                table: "ErrorLogs",
                column: "ExceptionTypeId",
                principalTable: "MExceptionTypes",
                principalColumn: "ExceptionTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ErrorLogs_MExceptionTypes_ExceptionTypeId",
                table: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "MExceptionTypes");

            migrationBuilder.DropIndex(
                name: "IX_ErrorLogs_ExceptionTypeId",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "ExceptionTypeId",
                table: "ErrorLogs");

            migrationBuilder.AddColumn<string>(
                name: "ExceptionType",
                table: "ErrorLogs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
