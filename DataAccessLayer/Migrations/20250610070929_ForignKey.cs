using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class ForignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OnlineApplications");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "UserMappings",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserMappings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ArmyPrefix",
                table: "Applications",
                column: "ArmyPrefix");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_MArmyPrefixes_ArmyPrefix",
                table: "Applications",
                column: "ArmyPrefix",
                principalTable: "MArmyPrefixes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_MArmyPrefixes_ArmyPrefix",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_ArmyPrefix",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserMappings");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "UserMappings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "OnlineApplications",
                columns: table => new
                {
                    Application_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AadharNo = table.Column<string>(type: "varchar(50)", nullable: false),
                    ApplicationType = table.Column<int>(type: "int", nullable: false),
                    PANNo = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineApplications", x => x.Application_Id);
                });
        }
    }
}
