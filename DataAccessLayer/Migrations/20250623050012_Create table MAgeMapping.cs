using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class CreatetableMAgeMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MAgeMapping",
                columns: table => new
                {
                    AgeMappingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegtId = table.Column<int>(type: "int", nullable: false),
                    RankId = table.Column<int>(type: "int", nullable: false),
                    RetirementAge = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MAgeMapping", x => x.AgeMappingId);
                    table.ForeignKey(
                        name: "FK_MAgeMapping_MRanks_RankId",
                        column: x => x.RankId,
                        principalTable: "MRanks",
                        principalColumn: "RankId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MAgeMapping_MRegtCorps_RegtId",
                        column: x => x.RegtId,
                        principalTable: "MRegtCorps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MAgeMapping_RankId",
                table: "MAgeMapping",
                column: "RankId");

            migrationBuilder.CreateIndex(
                name: "IX_MAgeMapping_RegtId",
                table: "MAgeMapping",
                column: "RegtId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MAgeMapping");
        }
    }
}
