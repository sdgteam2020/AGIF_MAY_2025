using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddTwotablesstatusanddigital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trnClaimDigitalSignRecords",
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
                    table.PrimaryKey("PK_trnClaimDigitalSignRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrnClaimStatusCounter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    ActionOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnClaimStatusCounter", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trnClaimDigitalSignRecords");

            migrationBuilder.DropTable(
                name: "TrnClaimStatusCounter");
        }
    }
}
