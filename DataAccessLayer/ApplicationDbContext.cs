using DataTransferObject.Identitytable;
using DataTransferObject.Model;
using DataTransferObject.Request;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public virtual DbSet<MAppointment> MAppointments { get; set; }
        public virtual DbSet<MUnit> MUnits { get; set; }
        public virtual DbSet<MApplyFor> MApplyFor { get; set; }
        public virtual DbSet<MRank> MRanks { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<MApplicationType> MApplicationTypes { get; set; }
        public virtual DbSet<MApplicantType> MApplicantTypes { get; set; }
        public virtual DbSet<MArmyPrefix> MArmyPrefixes { get; set; }
        public virtual DbSet<MRegtCorps> MRegtCorps { get; set; }
        public virtual DbSet<MArmyPostOffice> MArmyPostOffices { get; set; }
        public virtual DbSet<MLoanFreq> MLoanFreqs { get; set; }
        public virtual DbSet<MLoanType> MLoanTypes { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<UserMapping> trnUserMappings { get; set; }

        public virtual DbSet<CarApplicationModel> trnCar { get; set; }

        public virtual DbSet<PCAApplicationModel> trnPCA { get; set; }

        public virtual DbSet<HBAApplicationModel> trnHBA { get; set; }

        public virtual DbSet<CommonDataModel> trnApplications { get; set; }

        public virtual DbSet<DocumentUpload> trnDocumentUpload { get; set; }
        public virtual DbSet<MStatusTable> StatusTable { get; set; }
        public virtual DbSet<DigitalSignRecords> trnDigitalSignRecords { get; set; }
        public virtual DbSet<TrnFwd> TrnFwd { get; set; }
        public virtual DbSet<TrnFwdCO> TrnFwdCO { get; set; }

        public virtual DbSet<WithdrawalPurpose> WithdrawalPurpose { get; set; }

        public virtual DbSet<ClaimCommonModel> trnClaim { get; set; }

        public virtual DbSet<EducationDetailsModel> trnEducationDetails { get; set; }

        public virtual DbSet<MarriagewardModel> trnMarriageward { get; set; }

        public virtual DbSet<PropertyRenovationModel> trnPropertyRenovation { get; set; }

        public virtual DbSet<SplWaiverModel> trnSplWaiver { get; set; }

        public virtual DbSet<ClaimDocumentUpload> trnClaimDocumentUpload { get; set; }

        public virtual DbSet<TrnStatusCounter> TrnStatusCounter { get; set; }

        public virtual DbSet<MAgeMapping> MAgeMapping { get; set; }
        public virtual DbSet<MVehType> MVehType { get; set; }

        public virtual DbSet<AddressDetailsModel> trnAddressDetails { get; set; }
        public virtual DbSet<AccountDetailsModel> trnAccountDetails { get; set; }

        public virtual DbSet<ClaimAddressDetailsModel> trnClaimAddressDetails { get; set; }

        public virtual DbSet<ClaimAccountDetailsModel> trnClaimAccountDetails { get; set; }

        public virtual DbSet<ClaimDigitalSignRecords> trnClaimDigitalSignRecords { get; set; }

        public virtual DbSet<TrnClaimStatusCounter> TrnClaimStatusCounter { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CommonDataModel>()
                .HasOne<MUnit>()
                .WithMany()
                .HasForeignKey(a => a.PresentUnit)
                .HasPrincipalKey(u => u.UnitId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_trnApplications_MUnits_PresentUnit");


            builder.Entity<ClaimCommonModel>()
                .HasOne<MUnit>()
                .WithMany()
                .HasForeignKey(a => a.PresentUnit)
                .HasPrincipalKey(u => u.UnitId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_trnClaim_MUnits_PresentUnit");

            base.OnModelCreating(builder);
        }
    }

}
