using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Identitytable;
using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer.Repositories
{
    public class UsersApplicationDL : IUsersApplications
    {
        protected readonly ApplicationDbContext _db;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;

        public UsersApplicationDL(ApplicationDbContext db, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<List<DTOGetApplResponse>> GetApplicationByDate(DateTime date)
        {
            var result = await (from appl in _db.trnApplications
                                where EF.Functions.DateDiffDay(appl.UpdatedOn, date) == 0 && appl.StatusCode == 2
                                select new DTOGetApplResponse
                                {
                                    ApplicationId = appl.ApplicationId,
                                }).ToListAsync();

            return result;
        }


        public async Task<List<DTOGetApplResponse>> GetUsersApplication(int Mapping, int status)
        {
            int actualStatus = (status == 2 || status > 3) ? 2 : status;
            var UsersApplicationList = await (from appl in _db.trnApplications
                                   join user in _db.trnUserMappings on appl.PresentUnit equals user.UnitId
                                   join prefix in _db.MArmyPrefixes on appl.ArmyPrefix equals prefix.Id
                                   join applType in _db.MApplicationTypes on appl.ApplicationType equals applType.ApplicationTypeId
                                   where user.MappingId == Mapping && (appl.StatusCode == status || (status == 2 && appl.StatusCode > 3)) && user.IsPrimary == true
                                   orderby appl.UpdatedOn descending
                                   select new DTOGetApplResponse
                                   {
                                       ApplicationId = appl.ApplicationId,
                                       ArmyNo = prefix.Prefix + appl.Number + appl.Suffix,
                                       Name = appl.ApplicantName,
                                       ApplicationType = applType.ApplicationTypeName,
                                       DateOfBirth = appl.DateOfBirth.HasValue ? appl.DateOfBirth.Value.ToString("dd/MM/yyyy") : string.Empty,
                                       AppliedDate = appl.UpdatedOn.HasValue ? appl.UpdatedOn.Value.ToString("dd/MM/yyyy") : string.Empty,
                                       IsMergePdf = appl.IsMergePdf,
                                       UpdatedOn = appl.UpdatedOn
                                   }).ToListAsync();

            var COApplicationList = await (from appl in _db.trnApplications
                                   join profile in _db.UserProfiles on appl.IOArmyNo equals profile.ArmyNo
                                   join prefix in _db.MArmyPrefixes on appl.ArmyPrefix equals prefix.Id
                                   join applType in _db.MApplicationTypes on appl.ApplicationType equals applType.ApplicationTypeId
                                           where (appl.StatusCode == status || (status == 2 && appl.StatusCode > 3))
                                           orderby appl.UpdatedOn descending
                                   select new DTOGetApplResponse
                                   {
                                       ApplicationId = appl.ApplicationId,
                                       ArmyNo = prefix.Prefix + appl.Number + appl.Suffix,
                                       Name = appl.ApplicantName,
                                       ApplicationType = applType.ApplicationTypeName,
                                       DateOfBirth = appl.DateOfBirth.HasValue ? appl.DateOfBirth.Value.ToString("dd/MM/yyyy") : string.Empty,
                                       AppliedDate = appl.UpdatedOn.HasValue ? appl.UpdatedOn.Value.ToString("dd/MM/yyyy") : string.Empty,
                                       IsMergePdf = appl.IsMergePdf,
                                       UpdatedOn = appl.UpdatedOn
                                   }).ToListAsync();
            var applicationList = UsersApplicationList
                          .Union(COApplicationList)
                          .OrderByDescending(a => a.UpdatedOn)
                          .ToList();
            return applicationList!;
        }

        public async Task<List<DTOGetApplResponse>> GetUsersApplicationForAdmin(int status)
        {
            var UsersApplicationListToAdmin = await(from appl in _db.trnApplications
                                                    where appl.StatusCode == status
                                                    join prefix in _db.MArmyPrefixes on appl.ArmyPrefix equals prefix.Id
                                                    join oldPrefix in _db.MArmyPrefixes on appl.OldArmyPrefix equals oldPrefix.Id
                                                    join regt in _db.MRegtCorps on appl.RegtCorps equals regt.Id
                                                    join unit in _db.MUnits on appl.PresentUnit equals unit.UnitId
                                                    join statusName in _db.StatusTable on appl.StatusCode equals statusName.StatusCode
                                             join applType in _db.MApplicationTypes on appl.ApplicationType equals applType.ApplicationTypeId
                                             orderby appl.UpdatedOn descending
                                             select new DTOGetApplResponse
                                             {
                                                 ApplicationId = appl.ApplicationId,
                                                 PresentStatus = statusName.StatusName,
                                                 ArmyNo = prefix.Prefix + appl.Number + appl.Suffix,
                                                 Name = appl.ApplicantName,
                                                 OldArmyNo = oldPrefix.Prefix + appl.OldNumber + appl.OldSuffix,
                                                 RegtCorps = regt.RegtName,
                                                 PresentUnit = unit.UnitName,
                                                 PcdaPao = appl.pcda_pao,
                                                 AppliedDate = appl.UpdatedOn.HasValue ? appl.UpdatedOn.Value.ToString("dd/MM/yyyy") : string.Empty,
                                                 ApplicationType = appl.ApplicationType.ToString(),
                                                 UpdatedOn = appl.UpdatedOn,
                                                 DownloadedOn = appl.DownloadedOn,
                                                 DownloadCount = appl.DownloadCount,
                                             }).ToListAsync();
            return UsersApplicationListToAdmin!;
        }

        public async Task<bool> UpdateStatus(DTOExportRequest dtoExport)
        {
            var applications = _db.trnApplications
                .Where(a => dtoExport.Id.Contains(a.ApplicationId))
                .ToList();

            if (applications == null || applications.Count == 0)
                return false;

            foreach (var app in applications)
            {
                app.DownloadCount += 1;
                app.StatusCode = 4;
                app.DownloadedOn = DateTime.Now;
            }

            _db.trnApplications.UpdateRange(applications);

            int result = await _db.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> InsertStatusCounter(DTOExportRequest dtoExport)
        {
            if (dtoExport == null || dtoExport.Id == null || !dtoExport.Id.Any())
                return false;

            var statusCounters = dtoExport.Id.Select(appId => new TrnStatusCounter
            {
                StatusId = 5, // Assuming status 4 means "Downloaded"
                ApplicationId = appId,
                ActionOn = DateTime.Now
            }).ToList();

            await _db.TrnStatusCounter.AddRangeAsync(statusCounters);
            int result = await _db.SaveChangesAsync();

            return result > 0;
        }

        public async Task<List<DTOGetApplResponse>> GetMaturityUsersApplication(int Mapping, int status)
        {
            int actualStatus = (status == 2 || status > 3) ? 2 : status;
            var UsersApplicationList = await (from appl in _db.trnClaim
                                              join user in _db.trnUserMappings on appl.PresentUnit equals user.UnitId
                                              join prefix in _db.MArmyPrefixes on appl.ArmyPrefix equals prefix.Id
                                              join applType in _db.WithdrawalPurpose on appl.WithdrawPurpose equals applType.Id
                                              where user.MappingId == Mapping && (appl.StatusCode == status || (status == 2 && appl.StatusCode > 3)) && user.IsPrimary == true
                                              orderby appl.UpdatedOn descending
                                              select new DTOGetApplResponse
                                              {
                                                  ApplicationId = appl.ApplicationId,
                                                  ArmyNo = prefix.Prefix + appl.Number + appl.Suffix,
                                                  Name = appl.ApplicantName,
                                                  ApplicationType = applType.Name,
                                                  DateOfBirth = appl.DateOfBirth.HasValue ? appl.DateOfBirth.Value.ToString("dd/MM/yyyy") : string.Empty,
                                                  AppliedDate = appl.UpdatedOn.HasValue ? appl.UpdatedOn.Value.ToString("dd/MM/yyyy") : string.Empty,
                                                  IsMergePdf = appl.IsMergePdf,
                                                  UpdatedOn = appl.UpdatedOn
                                              }).ToListAsync();

            var COApplicationList = await (from appl in _db.trnClaim
                                           join profile in _db.UserProfiles on appl.IOArmyNo equals profile.ArmyNo
                                           join prefix in _db.MArmyPrefixes on appl.ArmyPrefix equals prefix.Id
                                           join applType in _db.WithdrawalPurpose on appl.WithdrawPurpose equals applType.Id
                                           where (appl.StatusCode == status || (status == 2 && appl.StatusCode > 3))
                                           orderby appl.UpdatedOn descending
                                           select new DTOGetApplResponse
                                           {
                                               ApplicationId = appl.ApplicationId,
                                               ArmyNo = prefix.Prefix + appl.Number + appl.Suffix,
                                               Name = appl.ApplicantName,
                                               ApplicationType = applType.Name,
                                               DateOfBirth = appl.DateOfBirth.HasValue ? appl.DateOfBirth.Value.ToString("dd/MM/yyyy") : string.Empty,
                                               AppliedDate = appl.UpdatedOn.HasValue ? appl.UpdatedOn.Value.ToString("dd/MM/yyyy") : string.Empty,
                                               IsMergePdf = appl.IsMergePdf,
                                               UpdatedOn = appl.UpdatedOn
                                           }).ToListAsync();
            var applicationList = UsersApplicationList
                          .Union(COApplicationList)
                          .OrderByDescending(a => a.UpdatedOn)
                          .ToList();
            return applicationList!;
        }

        public async Task<List<DTOGetApplResponse>> GetClaimUsersApplicationForAdmin(int status)
        {
            var UsersApplicationListToAdmin = await (from appl in _db.trnClaim
                                                     where appl.StatusCode == status
                                                     join prefix in _db.MArmyPrefixes on appl.ArmyPrefix equals prefix.Id
                                                     join oldPrefix in _db.MArmyPrefixes on appl.OldArmyPrefix equals oldPrefix.Id
                                                     join regt in _db.MRegtCorps on appl.RegtCorps equals regt.Id
                                                     join unit in _db.MUnits on appl.PresentUnit equals unit.UnitId
                                                     join statusName in _db.StatusTable on appl.StatusCode equals statusName.StatusCode
                                                     join applType in _db.WithdrawalPurpose on appl.WithdrawPurpose equals applType.Id
                                                     orderby appl.UpdatedOn descending
                                                     select new DTOGetApplResponse
                                                     {
                                                         ApplicationId = appl.ApplicationId,
                                                         PresentStatus = statusName.StatusName,
                                                         ArmyNo = prefix.Prefix + appl.Number + appl.Suffix,
                                                         Name = appl.ApplicantName,
                                                         OldArmyNo = oldPrefix.Prefix + appl.OldNumber + appl.OldSuffix,
                                                         RegtCorps = regt.RegtName,
                                                         PresentUnit = unit.UnitName,
                                                         PcdaPao = appl.pcda_pao,
                                                         AppliedDate = appl.UpdatedOn.HasValue ? appl.UpdatedOn.Value.ToString("dd/MM/yyyy") : string.Empty,
                                                         ApplicationType = applType.Name,
                                                         UpdatedOn = appl.UpdatedOn,
                                                         DownloadedOn = null,
                                                         DownloadCount = 0,
                                                     }).ToListAsync();
            return UsersApplicationListToAdmin!;
        }


        public async Task<bool> UpdateClaimStatus(DTOExportRequest dtoExport)
        {
            var applications = _db.trnClaim
                .Where(a => dtoExport.Id.Contains(a.ApplicationId))
                .ToList();

            if (applications == null || applications.Count == 0)
                return false;

            foreach (var app in applications)
            {
                app.DownloadCount += 1;
                app.StatusCode = 4;
                app.DownloadedOn = DateTime.Now;
            }

            _db.trnClaim.UpdateRange(applications);

            int result = await _db.SaveChangesAsync();

            return result > 0;
        }

        public async Task<List<DTOGetApplResponse>> GetClaimApplicationByDate(DateTime date)
        {
            var result = await (from appl in _db.trnClaim
                                where EF.Functions.DateDiffDay(appl.UpdatedOn, date) == 0 && appl.StatusCode == 2
                                select new DTOGetApplResponse
                                {
                                    ApplicationId = appl.ApplicationId,
                                }).ToListAsync();

            return result;
        }


        public async Task<bool> UpdateUserDetails(SessionUserDTO sessionUserDTO)
        {
            bool updated = false;

            // Update UserProfile
            var profile = await _db.UserProfiles.FirstOrDefaultAsync(x => x.ProfileId == sessionUserDTO.ProfileId);
            if (profile != null)
            {
                profile.Name = sessionUserDTO.name;
                profile.Email = sessionUserDTO.EmailId;
                profile.rank = sessionUserDTO.RankId;
                profile.MobileNo = sessionUserDTO.MobileNo;
                updated = true;
            }

            // Update UserMapping
            var mapping = await _db.trnUserMappings.FirstOrDefaultAsync(x => x.MappingId == sessionUserDTO.ProfileId);
            if (mapping != null)
            {
                mapping.IsFmn = sessionUserDTO.DteFmn;
                updated = true;
            }

            if (updated)
            {
                await _db.SaveChangesAsync();
            }

            // Update Identity User
            var user = await _userManager.FindByIdAsync(sessionUserDTO.ProfileId.ToString());
            if (user != null)
            {
                user.Email = sessionUserDTO.EmailId;
                user.PhoneNumber = sessionUserDTO.MobileNo;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> CheckAllApplnIdPresent(List<string> applicationIds)
        {
            if (applicationIds == null || !applicationIds.Any())
                return false;

            var allPresent = await _db.trnApplications
                .Where(a => applicationIds.Contains(a.ApplicationId.ToString()))
                .CountAsync();

            return allPresent == applicationIds.Count;
        }

        public async Task<List<string>> GetAllStatusCode()
        {
            return await _db.StatusTable
                .Select(s => s.StatusCode.ToString())
                .ToListAsync();
        }
        public async Task<List<string>> GetNotApplId(List<string> applicationIds)
        {
            if (applicationIds == null || !applicationIds.Any())
                return new List<string>();

            var presentApplicationIds = await _db.trnApplications
                .Where(a => applicationIds.Contains(a.ApplicationId.ToString()))
                .Select(a => a.ApplicationId.ToString())
                .ToListAsync();

            var notPresentApplicationIds = applicationIds.Except(presentApplicationIds).ToList();

            return notPresentApplicationIds;
        }

        public async Task<(bool, string)> ProcessBulkApplicationUpdates(System.Data.DataTable applicationUpdates)
        {
            var connection = _db.Database.GetDbConnection();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "dbo.ProcessBulkApplicationUpdates";
                command.CommandType = CommandType.StoredProcedure;

                // Add table-valued parameter
                var parameter = new SqlParameter("@ApplicationUpdates", applicationUpdates)
                {
                    SqlDbType = SqlDbType.Structured,
                    TypeName = "dbo.BulkApplicationUpdateType"
                };
                command.Parameters.Add(parameter);

                // Ensure connection is open
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var result = reader["Result"].ToString();
                        var message = reader["Message"].ToString();

                        return (result == "Success", message);
                    }
                }
            }

            return (false, "No response from stored procedure");
        }

        public DataTable CreateApplicationUpdatesDataTable(List<DTOApplStatusBulkUpload> applications)
        {
            DataTable table = new DataTable();
            table.Columns.Add("ApplId", typeof(int));
            table.Columns.Add("StatusCode", typeof(int));
            table.Columns.Add("Remarks", typeof(string));

            foreach (var app in applications)
            {
                table.Rows.Add(
                    app.ApplId,
                    app.Status_Code,
                    string.IsNullOrEmpty(app.Remarks) ? (object)DBNull.Value : app.Remarks
                );
            }

            return table;
        }
    }
}
