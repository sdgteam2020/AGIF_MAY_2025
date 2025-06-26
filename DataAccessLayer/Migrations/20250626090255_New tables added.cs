using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Newtablesadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IOUnit",
                table: "trnApplications");

            migrationBuilder.AddColumn<string>(
                name: "IOArmyNo",
                table: "trnApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "trnClaim",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArmyPrefix = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Suffix = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    OldArmyPrefix = table.Column<int>(type: "int", nullable: false),
                    OldNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    OldSuffix = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DdlRank = table.Column<int>(type: "int", nullable: false),
                    ApplicantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfCommission = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtnOfService = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfPromotion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateOfRetirement = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AadharCardNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PanCardNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailDomain = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TotalService = table.Column<int>(type: "int", nullable: true),
                    ResidualService = table.Column<int>(type: "int", nullable: true),
                    RegtCorps = table.Column<int>(type: "int", nullable: false),
                    pcda_pao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    pcda_AcctNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ParentUnit = table.Column<int>(type: "int", nullable: false),
                    PresentUnit = table.Column<int>(type: "int", nullable: false),
                    MUnitsPresentUnitId = table.Column<int>(type: "int", nullable: true),
                    PresentUnitPin = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    ArmyPostOffice = table.Column<int>(type: "int", nullable: false),
                    CivilPostalAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextFmnHQ = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SalaryAcctNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ConfirmSalaryAcctNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IfsCode = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    NameOfBank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameOfBankBranch = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    House_Building_Advance_Loan = table.Column<bool>(type: "bit", nullable: true),
                    House_Repair_Advance_Loan = table.Column<bool>(type: "bit", nullable: true),
                    Conveyance_Advance_Loan = table.Column<bool>(type: "bit", nullable: true),
                    Computer_Advance_Loan = table.Column<bool>(type: "bit", nullable: true),
                    House_Building_Date_of_Loan_taken = table.Column<DateTime>(type: "datetime2", nullable: true),
                    House_Building_Duration_of_Loan = table.Column<int>(type: "int", nullable: true),
                    House_Building_Amount_Taken = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    House_Repair_Advance_Date_of_Loan_taken = table.Column<DateTime>(type: "datetime2", nullable: true),
                    House_Repair_Advance_Duration_of_Loan = table.Column<int>(type: "int", nullable: true),
                    House_Repair_Advance_Amount_Taken = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Conveyance_Date_of_Loan_taken = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Conveyance_Duration_of_Loan = table.Column<int>(type: "int", nullable: true),
                    Conveyance_Amount_Taken = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Computer_Date_of_Loan_taken = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Computer_Duration_of_Loan = table.Column<int>(type: "int", nullable: true),
                    Computer_Amount_Taken = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AmountOfWithdrawalRequired = table.Column<int>(type: "int", nullable: true),
                    Noofwithdrawal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicantType = table.Column<int>(type: "int", nullable: false),
                    WithdrawPurpose = table.Column<int>(type: "int", nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trnClaim", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_trnClaim_MApplicantTypes_ApplicantType",
                        column: x => x.ApplicantType,
                        principalTable: "MApplicantTypes",
                        principalColumn: "ApplicantTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trnClaim_MArmyPostOffices_ArmyPostOffice",
                        column: x => x.ArmyPostOffice,
                        principalTable: "MArmyPostOffices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trnClaim_MArmyPrefixes_ArmyPrefix",
                        column: x => x.ArmyPrefix,
                        principalTable: "MArmyPrefixes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trnClaim_MRanks_DdlRank",
                        column: x => x.DdlRank,
                        principalTable: "MRanks",
                        principalColumn: "RankId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trnClaim_MRegtCorps_RegtCorps",
                        column: x => x.RegtCorps,
                        principalTable: "MRegtCorps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trnClaim_MUnits_MUnitsPresentUnitId",
                        column: x => x.MUnitsPresentUnitId,
                        principalTable: "MUnits",
                        principalColumn: "UnitId");
                    table.ForeignKey(
                        name: "FK_trnClaim_MUnits_ParentUnit",
                        column: x => x.ParentUnit,
                        principalTable: "MUnits",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trnClaim_MUnits_PresentUnit",
                        column: x => x.PresentUnit,
                        principalTable: "MUnits",
                        principalColumn: "UnitId");
                    table.ForeignKey(
                        name: "FK_trnClaim_WithdrawalPurpose_WithdrawPurpose",
                        column: x => x.WithdrawPurpose,
                        principalTable: "WithdrawalPurpose",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trnEducationDetails",
                columns: table => new
                {
                    EDId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    ChildName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DOPartIINo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DoPartIIDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PresentlyStudyingIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseForWithdrawal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollegeInstitution = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalExpenditure = table.Column<double>(type: "float", nullable: false),
                    IsAttachPartIIOrderPdf = table.Column<bool>(type: "bit", nullable: false),
                    IsAttachBonafideLetterPdf = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trnEducationDetails", x => x.EDId);
                    table.ForeignKey(
                        name: "FK_trnEducationDetails_trnClaim_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "trnClaim",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trnMarriageward",
                columns: table => new
                {
                    MWId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    NameOfChild = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DOPartIINo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DoPartIIDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AgeOfWard = table.Column<int>(type: "int", nullable: false),
                    DateofMarriage = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAttachPartIIOrderPdf = table.Column<bool>(type: "bit", nullable: false),
                    IsAttachInvitationcardPdf = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trnMarriageward", x => x.MWId);
                    table.ForeignKey(
                        name: "FK_trnMarriageward_trnClaim_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "trnClaim",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trnPropertyRenovation",
                columns: table => new
                {
                    PrId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    AddressOfProperty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyHolderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedCost = table.Column<double>(type: "float", nullable: false),
                    IsTotalExpenditureFilePdf = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trnPropertyRenovation", x => x.PrId);
                    table.ForeignKey(
                        name: "FK_trnPropertyRenovation_trnClaim_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "trnClaim",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trnSplWaiver",
                columns: table => new
                {
                    SWId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    OtherReasons = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trnSplWaiver", x => x.SWId);
                    table.ForeignKey(
                        name: "FK_trnSplWaiver_trnClaim_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "trnClaim",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trnClaim_ApplicantType",
                table: "trnClaim",
                column: "ApplicantType");

            migrationBuilder.CreateIndex(
                name: "IX_trnClaim_ArmyPostOffice",
                table: "trnClaim",
                column: "ArmyPostOffice");

            migrationBuilder.CreateIndex(
                name: "IX_trnClaim_ArmyPrefix",
                table: "trnClaim",
                column: "ArmyPrefix");

            migrationBuilder.CreateIndex(
                name: "IX_trnClaim_DdlRank",
                table: "trnClaim",
                column: "DdlRank");

            migrationBuilder.CreateIndex(
                name: "IX_trnClaim_MUnitsPresentUnitId",
                table: "trnClaim",
                column: "MUnitsPresentUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_trnClaim_ParentUnit",
                table: "trnClaim",
                column: "ParentUnit");

            migrationBuilder.CreateIndex(
                name: "IX_trnClaim_PresentUnit",
                table: "trnClaim",
                column: "PresentUnit");

            migrationBuilder.CreateIndex(
                name: "IX_trnClaim_RegtCorps",
                table: "trnClaim",
                column: "RegtCorps");

            migrationBuilder.CreateIndex(
                name: "IX_trnClaim_WithdrawPurpose",
                table: "trnClaim",
                column: "WithdrawPurpose");

            migrationBuilder.CreateIndex(
                name: "IX_trnEducationDetails_ApplicationId",
                table: "trnEducationDetails",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_trnMarriageward_ApplicationId",
                table: "trnMarriageward",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_trnPropertyRenovation_ApplicationId",
                table: "trnPropertyRenovation",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_trnSplWaiver_ApplicationId",
                table: "trnSplWaiver",
                column: "ApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trnEducationDetails");

            migrationBuilder.DropTable(
                name: "trnMarriageward");

            migrationBuilder.DropTable(
                name: "trnPropertyRenovation");

            migrationBuilder.DropTable(
                name: "trnSplWaiver");

            migrationBuilder.DropTable(
                name: "trnClaim");

            migrationBuilder.DropColumn(
                name: "IOArmyNo",
                table: "trnApplications");

            migrationBuilder.AddColumn<int>(
                name: "IOUnit",
                table: "trnApplications",
                type: "int",
                nullable: true);
        }
    }
}
