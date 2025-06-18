using DataTransferObject.Identitytable;
using DataTransferObject.Model;
using DataTransferObject.Request;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
        protected override void OnModelCreating(ModelBuilder builder)
        {

            // Configure foreign key relationship
            builder.Entity<CommonDataModel>()
                .HasOne<MUnit>()
                .WithMany()
                .HasForeignKey(a => a.PresentUnit)
                .HasPrincipalKey(u => u.UnitId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_trnApplications_MUnits_PresentUnit");

            base.OnModelCreating(builder);
        }
    }

}
