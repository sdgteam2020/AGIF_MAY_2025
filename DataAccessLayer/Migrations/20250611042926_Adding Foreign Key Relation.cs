using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddingForeignKeyRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PCA_LoanFreq",
                table: "trnPCA",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "HBA_LoanFreq",
                table: "trnHBA",
                type: "int",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "ApplicantType",
                table: "trnApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AppointmentAbbreviation",
                table: "MAppointments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_trnPCA_PCA_LoanFreq",
                table: "trnPCA",
                column: "PCA_LoanFreq");

            migrationBuilder.CreateIndex(
                name: "IX_trnHBA_HBA_LoanFreq",
                table: "trnHBA",
                column: "HBA_LoanFreq");

            migrationBuilder.CreateIndex(
                name: "IX_trnDocumentUpload_ApplicationId",
                table: "trnDocumentUpload",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_trnCar_CA_LoanFreq",
                table: "trnCar",
                column: "CA_LoanFreq");

            migrationBuilder.CreateIndex(
                name: "IX_trnApplications_ApplicantType",
                table: "trnApplications",
                column: "ApplicantType");

            migrationBuilder.CreateIndex(
                name: "IX_trnApplications_ApplicationType",
                table: "trnApplications",
                column: "ApplicationType");

            migrationBuilder.AddForeignKey(
                name: "FK_trnApplications_MApplicantTypes_ApplicantType",
                table: "trnApplications",
                column: "ApplicantType",
                principalTable: "MApplicantTypes",
                principalColumn: "ApplicantTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_trnApplications_MApplicationTypes_ApplicationType",
                table: "trnApplications",
                column: "ApplicationType",
                principalTable: "MApplicationTypes",
                principalColumn: "ApplicationTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_trnCar_MLoanFreqs_CA_LoanFreq",
                table: "trnCar",
                column: "CA_LoanFreq",
                principalTable: "MLoanFreqs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_trnDocumentUpload_trnApplications_ApplicationId",
                table: "trnDocumentUpload",
                column: "ApplicationId",
                principalTable: "trnApplications",
                principalColumn: "ApplicationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_trnHBA_MLoanFreqs_HBA_LoanFreq",
                table: "trnHBA",
                column: "HBA_LoanFreq",
                principalTable: "MLoanFreqs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_trnPCA_MLoanFreqs_PCA_LoanFreq",
                table: "trnPCA",
                column: "PCA_LoanFreq",
                principalTable: "MLoanFreqs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trnApplications_MApplicantTypes_ApplicantType",
                table: "trnApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_trnApplications_MApplicationTypes_ApplicationType",
                table: "trnApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_trnCar_MLoanFreqs_CA_LoanFreq",
                table: "trnCar");

            migrationBuilder.DropForeignKey(
                name: "FK_trnDocumentUpload_trnApplications_ApplicationId",
                table: "trnDocumentUpload");

            migrationBuilder.DropForeignKey(
                name: "FK_trnHBA_MLoanFreqs_HBA_LoanFreq",
                table: "trnHBA");

            migrationBuilder.DropForeignKey(
                name: "FK_trnPCA_MLoanFreqs_PCA_LoanFreq",
                table: "trnPCA");

            migrationBuilder.DropIndex(
                name: "IX_trnPCA_PCA_LoanFreq",
                table: "trnPCA");

            migrationBuilder.DropIndex(
                name: "IX_trnHBA_HBA_LoanFreq",
                table: "trnHBA");

            migrationBuilder.DropIndex(
                name: "IX_trnDocumentUpload_ApplicationId",
                table: "trnDocumentUpload");

            migrationBuilder.DropIndex(
                name: "IX_trnCar_CA_LoanFreq",
                table: "trnCar");

            migrationBuilder.DropIndex(
                name: "IX_trnApplications_ApplicantType",
                table: "trnApplications");

            migrationBuilder.DropIndex(
                name: "IX_trnApplications_ApplicationType",
                table: "trnApplications");

            migrationBuilder.DropColumn(
                name: "ApplicantType",
                table: "trnApplications");

            migrationBuilder.DropColumn(
                name: "AppointmentAbbreviation",
                table: "MAppointments");

            migrationBuilder.AlterColumn<string>(
                name: "PCA_LoanFreq",
                table: "trnPCA",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "HBA_LoanFreq",
                table: "trnHBA",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50);
        }
    }
}
