using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDatatype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArmyPrefix = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Suffix = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    OldArmyPrefix = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    OldNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    OldSuffix = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DdlRank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                    RegtCorps = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    pcda_pao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    pcda_AcctNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ParentUnit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PresentUnit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PresentUnitPin = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    ArmyPostOffice = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CivilPostalAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextFmnHQ = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Vill_Town = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PostOffice = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Distt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    SalaryAcctNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ConfirmSalaryAcctNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IfsCode = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    NameOfBank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameOfBankBranch = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MonthlyPaySlip = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BasicPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    dsop_afpp = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    rank_gradePay = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    agif_Subs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Msp = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncomeTaxMonthly = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CI_Pay = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EducationCess = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    npax_Pay = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Pli = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TechPay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    misc_Deduction = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Da = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    loanEMI_Outside = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Pmha = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LoanEmi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Lra = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MiscPay = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    salary_After_Deductions = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ApplicationId);
                });

            migrationBuilder.CreateTable(
                name: "Car",
                columns: table => new
                {
                    CarId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    DealerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Veh_Loan_Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VehicleCost = table.Column<int>(type: "int", nullable: false),
                    CA_LoanFreq = table.Column<int>(type: "int", nullable: false),
                    CA_Amt_Eligible_for_loan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CA_EMI_Eligible = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CA_repayingCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CA_Amount_Applied_For_Loan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CA_EMI_Applied = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CA_approxEMIAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DrivingLicenseNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Validity_Date_DL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DL_IssuingAuth = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CA_approxDisbursementAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Car", x => x.CarId);
                });

            migrationBuilder.CreateTable(
                name: "HBA",
                columns: table => new
                {
                    HbaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    PropertyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PropertySeller = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropertyAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    PropertyCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HBA_LoanFreq = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HBA_repayingCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HBA_Amt_Eligible_for_loan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HBA_EMI_Eligible = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HBA_Amount_Applied_For_Loan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HBA_EMI_Applied = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HBA_approxEMIAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HBA_approxDisbursementAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HBA", x => x.HbaId);
                });

            migrationBuilder.CreateTable(
                name: "PCA",
                columns: table => new
                {
                    PcaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    PCA_dealerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    computer_Loan_Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PCA_companyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PCA_modelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    computerCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PCA_LoanFreq = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PCA_Amt_Eligible_for_loan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PCA_EMI_Eligible = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PCA_repayingCapacity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PCA_Amount_Applied_For_Loan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PCA_EMI_Applied = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PCA_approxEMIAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PCA_approxDisbursementAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    totalResidualMonth = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCA", x => x.PcaId);
                });

            migrationBuilder.CreateTable(
                name: "UserMappings",
                columns: table => new
                {
                    MappingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMappings", x => x.MappingId);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    ProfileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArmyNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    userName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rank = table.Column<int>(type: "int", nullable: false),
                    regtCorps = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApptId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.ProfileId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Car");

            migrationBuilder.DropTable(
                name: "HBA");

            migrationBuilder.DropTable(
                name: "PCA");

            migrationBuilder.DropTable(
                name: "UserMappings");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
