using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddingTrnFwdTrnFwdCO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrnFwd",
                columns: table => new
                {
                    FwdId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    FromUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromProfileId = table.Column<int>(type: "int", nullable: false),
                    ToUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToProfileId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnFwd", x => x.FwdId);
                });

            migrationBuilder.CreateTable(
                name: "TrnFwdCO",
                columns: table => new
                {
                    FwdCOId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    ArmyNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    COUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnFwdCO", x => x.FwdCOId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrnFwd");

            migrationBuilder.DropTable(
                name: "TrnFwdCO");
        }
    }
}
