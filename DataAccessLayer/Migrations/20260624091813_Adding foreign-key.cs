using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Addingforeignkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TrnStatusCounter_ApplicationId",
                table: "TrnStatusCounter",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnLoginLogs_ProfileId",
                table: "TrnLoginLogs",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnFwdCO_ApplicationId",
                table: "TrnFwdCO",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnFwd_ApplicationId",
                table: "TrnFwd",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_TrnClaimStatusCounter_ApplicationId",
                table: "TrnClaimStatusCounter",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_trnClaimAddressDetails_Distt",
                table: "trnClaimAddressDetails",
                column: "Distt");

            migrationBuilder.CreateIndex(
                name: "IX_trnClaimAddressDetails_State",
                table: "trnClaimAddressDetails",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_trnCar_VehTypeId",
                table: "trnCar",
                column: "VehTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_trnCar_MVehType_VehTypeId",
                table: "trnCar",
                column: "VehTypeId",
                principalTable: "MVehType",
                principalColumn: "VehTypeId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_trnClaimAddressDetails_MDist_Distt",
                table: "trnClaimAddressDetails",
                column: "Distt",
                principalTable: "MDist",
                principalColumn: "DistrictId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_trnClaimAddressDetails_MState_State",
                table: "trnClaimAddressDetails",
                column: "State",
                principalTable: "MState",
                principalColumn: "StateId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnClaimStatusCounter_trnClaim_ApplicationId",
                table: "TrnClaimStatusCounter",
                column: "ApplicationId",
                principalTable: "trnClaim",
                principalColumn: "ApplicationId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFwd_trnApplications_ApplicationId",
                table: "TrnFwd",
                column: "ApplicationId",
                principalTable: "trnApplications",
                principalColumn: "ApplicationId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnFwdCO_trnApplications_ApplicationId",
                table: "TrnFwdCO",
                column: "ApplicationId",
                principalTable: "trnApplications",
                principalColumn: "ApplicationId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnLoginLogs_UserProfiles_ProfileId",
                table: "TrnLoginLogs",
                column: "ProfileId",
                principalTable: "UserProfiles",
                principalColumn: "ProfileId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TrnStatusCounter_trnApplications_ApplicationId",
                table: "TrnStatusCounter",
                column: "ApplicationId",
                principalTable: "trnApplications",
                principalColumn: "ApplicationId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trnCar_MVehType_VehTypeId",
                table: "trnCar");

            migrationBuilder.DropForeignKey(
                name: "FK_trnClaimAddressDetails_MDist_Distt",
                table: "trnClaimAddressDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_trnClaimAddressDetails_MState_State",
                table: "trnClaimAddressDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnClaimStatusCounter_trnClaim_ApplicationId",
                table: "TrnClaimStatusCounter");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnFwd_trnApplications_ApplicationId",
                table: "TrnFwd");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnFwdCO_trnApplications_ApplicationId",
                table: "TrnFwdCO");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnLoginLogs_UserProfiles_ProfileId",
                table: "TrnLoginLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TrnStatusCounter_trnApplications_ApplicationId",
                table: "TrnStatusCounter");

            migrationBuilder.DropIndex(
                name: "IX_TrnStatusCounter_ApplicationId",
                table: "TrnStatusCounter");

            migrationBuilder.DropIndex(
                name: "IX_TrnLoginLogs_ProfileId",
                table: "TrnLoginLogs");

            migrationBuilder.DropIndex(
                name: "IX_TrnFwdCO_ApplicationId",
                table: "TrnFwdCO");

            migrationBuilder.DropIndex(
                name: "IX_TrnFwd_ApplicationId",
                table: "TrnFwd");

            migrationBuilder.DropIndex(
                name: "IX_TrnClaimStatusCounter_ApplicationId",
                table: "TrnClaimStatusCounter");

            migrationBuilder.DropIndex(
                name: "IX_trnClaimAddressDetails_Distt",
                table: "trnClaimAddressDetails");

            migrationBuilder.DropIndex(
                name: "IX_trnClaimAddressDetails_State",
                table: "trnClaimAddressDetails");

            migrationBuilder.DropIndex(
                name: "IX_trnCar_VehTypeId",
                table: "trnCar");
        }
    }
}
