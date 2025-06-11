using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DomainId = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    ExceptionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MApplicantTypes",
                columns: table => new
                {
                    ApplicantTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MApplicantTypes", x => x.ApplicantTypeId);
                });

            migrationBuilder.CreateTable(
                name: "MApplicationTypes",
                columns: table => new
                {
                    ApplicationTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MApplicationTypes", x => x.ApplicationTypeId);
                });

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
                    ApptId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MAppointments", x => x.ApptId);
                });

            migrationBuilder.CreateTable(
                name: "MArmyPostOffices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArmyPostOffice = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MArmyPostOffices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MArmyPrefixes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MArmyPrefixes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MLoanFreqs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanFreq = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MLoanFreqs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MRegtCorps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegtName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PCDA_PAO = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRegtCorps", x => x.Id);
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
                name: "trnDocumentUpload",
                columns: table => new
                {
                    UploadId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    CancelledCheque = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaySlipPdf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuotationPdf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DrivingLicensePdf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeviceExtnPdf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCancelledCheque = table.Column<bool>(type: "bit", nullable: false),
                    IsPaySlipPdf = table.Column<bool>(type: "bit", nullable: false),
                    IsQuotationPdf = table.Column<bool>(type: "bit", nullable: false),
                    IsDrivingLicensePdf = table.Column<bool>(type: "bit", nullable: false),
                    IsSeviceExtnPdf = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trnDocumentUpload", x => x.UploadId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MLoanTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoanTypeCode = table.Column<int>(type: "int", nullable: false),
                    ApplicationType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MLoanTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MLoanTypes_MApplicationTypes_ApplicationType",
                        column: x => x.ApplicationType,
                        principalTable: "MApplicationTypes",
                        principalColumn: "ApplicationTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MRanks",
                columns: table => new
                {
                    RankId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RankName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    RankAbbreviation = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    Orderby = table.Column<short>(type: "smallint", nullable: false),
                    ApplyForId = table.Column<byte>(type: "tinyint", nullable: false),
                    rank_cd = table.Column<int>(type: "int", nullable: true),
                    RetirementAge = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRanks", x => x.RankId);
                    table.ForeignKey(
                        name: "FK_MRanks_MApplyFor_ApplyForId",
                        column: x => x.ApplyForId,
                        principalTable: "MApplyFor",
                        principalColumn: "ApplyForId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trnApplications",
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
                    ApplicationType = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trnApplications", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_trnApplications_MArmyPostOffices_ArmyPostOffice",
                        column: x => x.ArmyPostOffice,
                        principalTable: "MArmyPostOffices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trnApplications_MArmyPrefixes_ArmyPrefix",
                        column: x => x.ArmyPrefix,
                        principalTable: "MArmyPrefixes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trnApplications_MRanks_DdlRank",
                        column: x => x.DdlRank,
                        principalTable: "MRanks",
                        principalColumn: "RankId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trnApplications_MRegtCorps_RegtCorps",
                        column: x => x.RegtCorps,
                        principalTable: "MRegtCorps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trnApplications_MUnits_MUnitsPresentUnitId",
                        column: x => x.MUnitsPresentUnitId,
                        principalTable: "MUnits",
                        principalColumn: "UnitId");
                    table.ForeignKey(
                        name: "FK_trnApplications_MUnits_ParentUnit",
                        column: x => x.ParentUnit,
                        principalTable: "MUnits",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trnApplications_MUnits_PresentUnit",
                        column: x => x.PresentUnit,
                        principalTable: "MUnits",
                        principalColumn: "UnitId");
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    ProfileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArmyNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    userName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    rank = table.Column<int>(type: "int", nullable: false),
                    regtCorps = table.Column<int>(type: "int", nullable: false),
                    ApptId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.ProfileId);
                    table.ForeignKey(
                        name: "FK_UserProfiles_MAppointments_ApptId",
                        column: x => x.ApptId,
                        principalTable: "MAppointments",
                        principalColumn: "ApptId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProfiles_MRanks_rank",
                        column: x => x.rank,
                        principalTable: "MRanks",
                        principalColumn: "RankId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProfiles_MRegtCorps_regtCorps",
                        column: x => x.regtCorps,
                        principalTable: "MRegtCorps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trnCar",
                columns: table => new
                {
                    CarId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    DealerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Veh_Loan_Type = table.Column<int>(type: "int", nullable: false),
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
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trnCar", x => x.CarId);
                    table.ForeignKey(
                        name: "FK_trnCar_trnApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "trnApplications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trnHBA",
                columns: table => new
                {
                    HbaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    PropertyType = table.Column<int>(type: "int", nullable: false),
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
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trnHBA", x => x.HbaId);
                    table.ForeignKey(
                        name: "FK_trnHBA_trnApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "trnApplications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trnPCA",
                columns: table => new
                {
                    PcaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    PCA_dealerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    computer_Loan_Type = table.Column<int>(type: "int", nullable: false),
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
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trnPCA", x => x.PcaId);
                    table.ForeignKey(
                        name: "FK_trnPCA_trnApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "trnApplications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trnUserMappings",
                columns: table => new
                {
                    MappingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Updatedby = table.Column<int>(type: "int", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trnUserMappings", x => x.MappingId);
                    table.ForeignKey(
                        name: "FK_trnUserMappings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trnUserMappings_MUnits_UnitId",
                        column: x => x.UnitId,
                        principalTable: "MUnits",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trnUserMappings_UserProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MLoanTypes_ApplicationType",
                table: "MLoanTypes",
                column: "ApplicationType");

            migrationBuilder.CreateIndex(
                name: "IX_MRanks_ApplyForId",
                table: "MRanks",
                column: "ApplyForId");

            migrationBuilder.CreateIndex(
                name: "IX_trnApplications_ArmyPostOffice",
                table: "trnApplications",
                column: "ArmyPostOffice");

            migrationBuilder.CreateIndex(
                name: "IX_trnApplications_ArmyPrefix",
                table: "trnApplications",
                column: "ArmyPrefix");

            migrationBuilder.CreateIndex(
                name: "IX_trnApplications_DdlRank",
                table: "trnApplications",
                column: "DdlRank");

            migrationBuilder.CreateIndex(
                name: "IX_trnApplications_MUnitsPresentUnitId",
                table: "trnApplications",
                column: "MUnitsPresentUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_trnApplications_ParentUnit",
                table: "trnApplications",
                column: "ParentUnit");

            migrationBuilder.CreateIndex(
                name: "IX_trnApplications_PresentUnit",
                table: "trnApplications",
                column: "PresentUnit");

            migrationBuilder.CreateIndex(
                name: "IX_trnApplications_RegtCorps",
                table: "trnApplications",
                column: "RegtCorps");

            migrationBuilder.CreateIndex(
                name: "IX_trnCar_ApplicationId",
                table: "trnCar",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_trnHBA_ApplicationId",
                table: "trnHBA",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_trnPCA_ApplicationId",
                table: "trnPCA",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_trnUserMappings_ProfileId",
                table: "trnUserMappings",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_trnUserMappings_UnitId",
                table: "trnUserMappings",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_trnUserMappings_UserId",
                table: "trnUserMappings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_ApptId",
                table: "UserProfiles",
                column: "ApptId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_rank",
                table: "UserProfiles",
                column: "rank");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_regtCorps",
                table: "UserProfiles",
                column: "regtCorps");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "MApplicantTypes");

            migrationBuilder.DropTable(
                name: "MLoanFreqs");

            migrationBuilder.DropTable(
                name: "MLoanTypes");

            migrationBuilder.DropTable(
                name: "trnCar");

            migrationBuilder.DropTable(
                name: "trnDocumentUpload");

            migrationBuilder.DropTable(
                name: "trnHBA");

            migrationBuilder.DropTable(
                name: "trnPCA");

            migrationBuilder.DropTable(
                name: "trnUserMappings");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "MApplicationTypes");

            migrationBuilder.DropTable(
                name: "trnApplications");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropTable(
                name: "MArmyPostOffices");

            migrationBuilder.DropTable(
                name: "MArmyPrefixes");

            migrationBuilder.DropTable(
                name: "MUnits");

            migrationBuilder.DropTable(
                name: "MAppointments");

            migrationBuilder.DropTable(
                name: "MRanks");

            migrationBuilder.DropTable(
                name: "MRegtCorps");

            migrationBuilder.DropTable(
                name: "MApplyFor");
        }
    }
}
