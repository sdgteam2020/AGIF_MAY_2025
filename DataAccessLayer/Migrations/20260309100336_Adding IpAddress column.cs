using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddingIpAddresscolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ipAddress",
                table: "trnDigitalSignRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ipAddress",
                table: "trnClaimDigitalSignRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

          
            migrationBuilder.AddColumn<string>(
                name: "ipAddress",
                table: "trnApplications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ipAddress",
                table: "trnDigitalSignRecords");

            migrationBuilder.DropColumn(
                name: "ipAddress",
                table: "trnClaimDigitalSignRecords");

            migrationBuilder.DropColumn(
                name: "ipAddress",
                table: "trnApplications");

         
        }
    }
}
