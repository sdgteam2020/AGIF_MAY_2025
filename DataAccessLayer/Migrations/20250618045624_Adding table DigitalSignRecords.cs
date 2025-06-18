using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddingtableDigitalSignRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trnDigitalSignRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArmyNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DomainId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplId = table.Column<int>(type: "int", nullable: false),
                    IsSign = table.Column<bool>(type: "bit", nullable: false),
                    IsRejectced = table.Column<bool>(type: "bit", nullable: false),
                    XMLSignResponse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trnDigitalSignRecords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trnDigitalSignRecords");
        }
    }
}
