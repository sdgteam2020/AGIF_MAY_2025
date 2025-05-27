using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PANNo",
                table: "OnlineApplications",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AadharNo",
                table: "OnlineApplications",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "MApplyFor",
                columns: table => new
                {
                    ApplyForId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MApplyFor", x => x.ApplyForId);
                });

            migrationBuilder.CreateTable(
                name: "MAppointments",
                columns: table => new
                {
                    ApptId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppointmentAbbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MAppointments", x => x.ApptId);
                });

            migrationBuilder.CreateTable(
                name: "MUnits",
                columns: table => new
                {
                    UnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sus_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Suffix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerify = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MUnits", x => x.UnitId);
                });

            migrationBuilder.CreateTable(
                name: "MRanks",
                columns: table => new
                {
                    RankId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RankName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    RankAbbreviation = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    Orderby = table.Column<short>(type: "smallint", nullable: false),
                    ApplyForId = table.Column<byte>(type: "tinyint", nullable: false),
                    rank_cd = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRanks", x => x.RankId);
                    table.ForeignKey(
                        name: "FK_MRanks_AspNetUsers_Updatedby",
                        column: x => x.Updatedby,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MRanks_MApplyFor_ApplyForId",
                        column: x => x.ApplyForId,
                        principalTable: "MApplyFor",
                        principalColumn: "ApplyForId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MRanks_ApplyForId",
                table: "MRanks",
                column: "ApplyForId");

            migrationBuilder.CreateIndex(
                name: "IX_MRanks_Updatedby",
                table: "MRanks",
                column: "Updatedby");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MAppointments");

            migrationBuilder.DropTable(
                name: "MRanks");

            migrationBuilder.DropTable(
                name: "MUnits");

            migrationBuilder.DropTable(
                name: "MApplyFor");

            migrationBuilder.AlterColumn<string>(
                name: "PANNo",
                table: "OnlineApplications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "AadharNo",
                table: "OnlineApplications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)");
        }
    }
}
