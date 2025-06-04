using DataTransferObject.Identitytable;
using DataTransferObject.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
        public virtual DbSet<OnlineApplications> OnlineApplications { get; set; }
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

        public virtual DbSet<CarApplicationModel> Car { get; set; }

        public virtual DbSet<PCAApplicationModel> PCA { get; set; }

        public virtual DbSet<HBAApplicationModel> HBA { get; set; }

        public virtual DbSet<CommonDataModel> Applications { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }

}
