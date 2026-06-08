using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Add_hitCounter_and_MIPAddress_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MIpAddresses",
                columns: table => new
                {
                    IpAddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MIpAddresses", x => x.IpAddressId);
                });

            migrationBuilder.CreateTable(
                name: "HitCounters",
                columns: table => new
                {
                    HitCounterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IpAddressId = table.Column<int>(type: "int", nullable: false),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HitCounters", x => x.HitCounterId);
                    table.ForeignKey(
                        name: "FK_HitCounters_MIpAddresses_IpAddressId",
                        column: x => x.IpAddressId,
                        principalTable: "MIpAddresses",
                        principalColumn: "IpAddressId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HitCounters_IpAddressId",
                table: "HitCounters",
                column: "IpAddressId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HitCounters");

            migrationBuilder.DropTable(
                name: "MIpAddresses");
        }
    }
}
