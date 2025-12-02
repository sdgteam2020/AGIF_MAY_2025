using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Identitytable;
using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.RegularExpressions;

namespace DataAccessLayer.Repositories
{
    public class UsersApplicationDL : IUsersApplications
    {
        protected readonly ApplicationDbContext _db;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private static readonly Regex ArmyNoRegex =
        new(@"^(?<prefix>[A-Za-z]{1,3})(?<number>\d{3,8})(?<suffix>[A-Za-z]?)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public UsersApplicationDL(ApplicationDbContext db, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<List<DTOGetApplResponse>> GetApplicationByDate(DateTime date)
        {
            var result = await (from appl in _db.trnApplications
                                join DigitalSignRecords in _db.trnDigitalSignRecords on appl.ApplicationId equals DigitalSignRecords.ApplId
                                where EF.Functions.DateDiffDay(DigitalSignRecords.SignOn, date) == 0 && (appl.StatusCode == 2 || appl.StatusCode == 4)
                                select new DTOGetApplResponse
                                {
                                    ApplicationId = appl.ApplicationId,
                                }).ToListAsync();

            return result;
        }


        public async Task<List<DTOGetApplResponse>> GetUsersApplication(int Mapping, int status)
        {
            int actualStatus = (status == 2 || status > 3) ? 2 : status;

            // Local function to get queryable
            IQueryable<DTOGetApplResponse> GetApplications(bool isUserMapped)
            {
                var query = from appl in _db.trnApplications
                            join prefix in _db.MArmyPrefixes on appl.ArmyPrefix equals prefix.Id
                            join applType in _db.MApplicationTypes on appl.ApplicationType equals applType.ApplicationTypeId
                            join digitalSign in _db.trnDigitalSignRecords on appl.ApplicationId equals digitalSign.ApplId into ds
                            from digitalSign in ds.DefaultIfEmpty()
                            select new { appl, prefix, applType, digitalSign };

                if (isUserMapped)
                {
                    query = from q in query
                            join user in _db.trnUserMappings on q.appl.PresentUnit equals user.UnitId
                            where user.MappingId == Mapping && user.IsPrimary == true
                                  && (q.appl.StatusCode == status || (status == 2 && q.appl.StatusCode > 3))
                            select q;
                }
                else
                {
                    query = from q in query
                            join profile in _db.UserProfiles on q.appl.IOArmyNo equals profile.ArmyNo
                            where (q.appl.StatusCode == status || (status == 2 && q.appl.StatusCode > 3))
                            select q;
                }

                return query.Select(q => new DTOGetApplResponse
                {
                    ApplicationId = q.appl.ApplicationId,
                    ArmyNo = q.prefix.Prefix + q.appl.Number + q.appl.Suffix,
                    Name = q.appl.ApplicantName ?? string.Empty,
                    ApplicationType = q.applType.ApplicationTypeName,
                    DateOfBirth = q.appl.DateOfBirth.HasValue ? q.appl.DateOfBirth.Value.ToString("dd/MM/yyyy") : string.Empty,
                    AppliedDate = q.appl.UpdatedOn.HasValue ? q.appl.UpdatedOn.Value.ToString("dd/MM/yyyy") : string.Empty,
                    IsMergePdf = q.appl.IsMergePdf,
                    UpdatedOn = q.appl.UpdatedOn,
                    DigitalSignDate = q.digitalSign != null && q.digitalSign.SignOn.HasValue ? q.digitalSign.SignOn.Value : (DateTime?)null
                });
            }

            // Execute queries
            var usersApplicationList = await GetApplications(true).ToListAsync();
            var coApplicationList = await GetApplications(false).ToListAsync();

            // Merge
            var applicationList = usersApplicationList.Union(coApplicationList);

            // Conditional order
            applicationList = (status == 2)
                ? applicationList.OrderByDescending(a => a.DigitalSignDate ?? DateTime.MinValue)
                : applicationList.OrderByDescending(a => a.UpdatedOn ?? DateTime.MinValue);

            return applicationList.ToList();
        }



        public async Task<List<DTOGetApplResponse>> GetUsersApplicationForAdmin(int status)
        {
            var UsersApplicationListToAdmin = await (from appl in _db.trnApplications
                                                     where appl.StatusCode == status
                                                     join prefix in _db.MArmyPrefixes on appl.ArmyPrefix equals prefix.Id
                                                     join oldPrefix in _db.MArmyPrefixes on appl.OldArmyPrefix equals oldPrefix.Id
                                                     join regt in _db.MRegtCorps on appl.RegtCorps equals regt.Id
                                                     join unit in _db.MUnits on appl.PresentUnit equals unit.UnitId
                                                     join statusName in _db.StatusTable on appl.StatusCode equals statusName.StatusCode
                                                     join applType in _db.MApplicationTypes on appl.ApplicationType equals applType.ApplicationTypeId
                                                     join digitalSign in _db.trnDigitalSignRecords on appl.ApplicationId equals digitalSign.ApplId
                                                     orderby digitalSign.SignOn descending
                                                     select new DTOGetApplResponse
                                                     {
                                                         ApplicationId = appl.ApplicationId,
                                                         PresentStatus = statusName.StatusName,
                                                         ArmyNo = prefix.Prefix + appl.Number + appl.Suffix,
                                                         Name = appl.ApplicantName ?? string.Empty,
                                                         OldArmyNo = oldPrefix.Prefix + appl.OldNumber + appl.OldSuffix,
                                                         RegtCorps = regt.RegtName,
                                                         PresentUnit = unit.UnitName,
                                                         PcdaPao = appl.pcda_pao ?? string.Empty,
                                                         AppliedDate = digitalSign.SignOn.HasValue ? digitalSign.SignOn.Value.ToString("dd/MM/yyyy") : string.Empty,
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

            var statusCounters = new List<TrnStatusCounter>();

            foreach (var app in applications)
            {
                app.DownloadCount += 1;
                app.StatusCode = 4;
                app.DownloadedOn = DateTime.Now;

                // Add a new entry to TrnStatusCounter for each application
                statusCounters.Add(new TrnStatusCounter
                {
                    StatusId = 4,
                    ApplicationId = app.ApplicationId,
                    ActionOn = DateTime.Now
                });
            }

            _db.trnApplications.UpdateRange(applications);

            await _db.TrnStatusCounter.AddRangeAsync(statusCounters);

            int result = await _db.SaveChangesAsync();

            return result > 0;
        }

       
        public async Task<List<DTOGetApplResponse>> GetMaturityUsersApplication(int Mapping, int status)
        {
            int actualStatus = (status == 102 || status > 103) ? 102 : status;

            // Local function to get queryable
            IQueryable<DTOGetApplResponse> GetApplications(bool isUserMapped)
            {
                var query = from appl in _db.trnClaim
                            join prefix in _db.MArmyPrefixes on appl.ArmyPrefix equals prefix.Id
                            join applType in _db.WithdrawalPurpose on appl.WithdrawPurpose equals applType.Id
                            join digitalSign in _db.trnClaimDigitalSignRecords on appl.ApplicationId equals digitalSign.ApplId into ds
                            from digitalSign in ds.DefaultIfEmpty()
                            select new { appl, prefix, applType, digitalSign };

                if (isUserMapped)
                {
                    query = from q in query
                            join user in _db.trnUserMappings on q.appl.PresentUnit equals user.UnitId
                            where user.MappingId == Mapping && user.IsPrimary == true
                                  && (q.appl.StatusCode == status || (status == 102 && q.appl.StatusCode > 103))
                            select q;
                }
                else
                {
                    query = from q in query
                            join profile in _db.UserProfiles on q.appl.IOArmyNo equals profile.ArmyNo
                            where (q.appl.StatusCode == status || (status == 102 && q.appl.StatusCode > 103))
                            select q;
                }

                return query.Select(q => new DTOGetApplResponse
                {
                    ApplicationId = q.appl.ApplicationId,
                    ArmyNo = q.prefix.Prefix + q.appl.Number + q.appl.Suffix,
                    Name = q.appl.ApplicantName ?? string.Empty,
                    ApplicationType = q.applType.Name ?? string.Empty,
                    DateOfBirth = q.appl.DateOfBirth.HasValue ? q.appl.DateOfBirth.Value.ToString("dd/MM/yyyy") : string.Empty,
                    AppliedDate = q.appl.UpdatedOn.HasValue ? q.appl.UpdatedOn.Value.ToString("dd/MM/yyyy") : string.Empty,
                    IsMergePdf = q.appl.IsMergePdf,
                    UpdatedOn = q.appl.UpdatedOn,
                    DigitalSignDate = q.digitalSign != null && q.digitalSign.SignOn.HasValue
                        ? q.digitalSign.SignOn.Value
                        : (DateTime?)null
                });
            }

            // Execute queries
            var usersApplicationList = await GetApplications(true).ToListAsync();
            var coApplicationList = await GetApplications(false).ToListAsync();

            // Merge
            var applicationList = usersApplicationList.Union(coApplicationList);

            // Conditional order
            applicationList = (status == 102)
                ? applicationList.OrderByDescending(a => a.DigitalSignDate ?? DateTime.MinValue)
                : applicationList.OrderByDescending(a => a.UpdatedOn ?? DateTime.MinValue);

            return applicationList.ToList();
        }


        public async Task<List<DTOGetApplResponse>> GetClaimUsersApplicationForAdmin(int status)
        {
            var UsersApplicationListToAdmin = await (from appl in _db.trnClaim
                                                     where appl.StatusCode == status
                                                     join prefix in _db.MArmyPrefixes on appl.ArmyPrefix equals prefix.Id
                                                     join oldPrefix in _db.MArmyPrefixes on appl.OldArmyPrefix equals oldPrefix.Id
                                                     join regt in _db.MRegtCorps on appl.RegtCorps equals regt.Id
                                                     join unit in _db.MUnits on appl.PresentUnit equals unit.UnitId
                                                     join statusName in _db.StatusTable on appl.StatusCode equals statusName.ClaimStatusCode
                                                     join applType in _db.WithdrawalPurpose on appl.WithdrawPurpose equals applType.Id
                                                     join digitalSign in _db.trnClaimDigitalSignRecords on appl.ApplicationId equals digitalSign.ApplId
                                                     orderby digitalSign.SignOn descending
                                                     select new DTOGetApplResponse
                                                     {
                                                         ApplicationId = appl.ApplicationId,
                                                         PresentStatus = statusName.StatusName,
                                                         ArmyNo = prefix.Prefix + appl.Number + appl.Suffix,
                                                         Name = appl.ApplicantName ?? string.Empty,
                                                         OldArmyNo = oldPrefix.Prefix + appl.OldNumber + appl.OldSuffix,
                                                         RegtCorps = regt.RegtName,
                                                         PresentUnit = unit.UnitName,
                                                         PcdaPao = appl.pcda_pao ?? string.Empty,
                                                         AppliedDate = digitalSign.SignOn.HasValue ? digitalSign.SignOn.Value.ToString("dd/MM/yyyy") : string.Empty,
                                                         ApplicationType = applType.Name ?? string.Empty,
                                                         UpdatedOn = appl.UpdatedOn,
                                                         DownloadedOn = appl.DownloadedOn,
                                                         DownloadCount = appl.DownloadCount,
                                                     }).ToListAsync();
            return UsersApplicationListToAdmin!;
        }


        public async Task<bool> UpdateClaimStatus(DTOExportRequest dtoExport)
        {
            var applications = _db.trnClaim
                .Where(a => dtoExport.Id.Contains(a.ApplicationId))
                .ToList();

            var statusCounters = new List<TrnClaimStatusCounter>();

            if (applications == null || applications.Count == 0)
                return false;

            foreach (var app in applications)
            {
                app.DownloadCount += 1;
                app.StatusCode = 104;
                app.DownloadedOn = DateTime.Now;

                statusCounters.Add(new TrnClaimStatusCounter
                {
                    StatusId = 104,
                    ApplicationId = app.ApplicationId,
                    ActionOn = DateTime.Now
                });
            }

            _db.trnClaim.UpdateRange(applications);

            await _db.TrnClaimStatusCounter.AddRangeAsync(statusCounters);

            int result = await _db.SaveChangesAsync();

            return result > 0;

        }

        public async Task<List<DTOGetApplResponse>> GetClaimApplicationByDate(DateTime date)
        {
            var result = await (from appl in _db.trnClaim
                                join DigitalSignRecords in _db.trnClaimDigitalSignRecords on appl.ApplicationId equals DigitalSignRecords.ApplId
                                where EF.Functions.DateDiffDay(DigitalSignRecords.SignOn, date) == 0 && (appl.StatusCode == 102 || appl.StatusCode == 104)
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
                profile.ArmyNo = sessionUserDTO.ArmyNo ?? string.Empty;
                profile.Name = sessionUserDTO.name ?? string.Empty;
                profile.Email = sessionUserDTO.EmailId ?? string.Empty;
                profile.rank = sessionUserDTO.RankId;
                profile.MobileNo = sessionUserDTO.MobileNo ?? string.Empty;
                profile.regtCorps=sessionUserDTO.RegtId;
                profile.UpdatedOn = DateTime.Now;
                updated = true;
            }

            // Update UserMapping
            var mapping = await _db.trnUserMappings.FirstOrDefaultAsync(x => x.ProfileId == sessionUserDTO.ProfileId);
            if (mapping != null)
            {
                mapping.IsFmn = sessionUserDTO.DteFmn;
                mapping.IsActive = false;
                mapping.UpdatedOn = DateTime.Now;
                updated = true;
            }

            if (updated)
            {
                await _db.SaveChangesAsync();
            }

            // Update Identity User
            var user = await _userManager.FindByIdAsync(sessionUserDTO.MappingId.ToString());
            if (user != null)
            {
                user.Email = sessionUserDTO.EmailId;
                user.PhoneNumber = sessionUserDTO.MobileNo;
                user.UpdatedOn = DateTime.Now;
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
        public async Task<List<string>> GetAllClaimStatusCode()
        {
            return await _db.StatusTable
                .Select(s => s.ClaimStatusCode.ToString())
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

        public async Task<List<string>> GetClaimNotApplId(List<string> applicationIds)
        {
            if (applicationIds == null || !applicationIds.Any())
                return new List<string>();

            var presentApplicationIds = await _db.trnClaim
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

        public async Task<(bool, string)> ClaimProcessBulkApplicationUpdates(System.Data.DataTable applicationUpdates)
        {
            var connection = _db.Database.GetDbConnection();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "dbo.ClaimProcessBulkApplicationUpdates";
                command.CommandType = CommandType.StoredProcedure;

                // Add table-valued parameter
                var parameter = new SqlParameter("@ApplicationUpdates", applicationUpdates)
                {
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

        public async Task<IReadOnlyList<DTOGetApplResponse>> GetApplicantHistoryAsync(string armyNo, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(armyNo))
                return Array.Empty<DTOGetApplResponse>();

            var normalized = armyNo.Trim().ToUpperInvariant();
            var match = ArmyNoRegex.Match(normalized);

            if (!match.Success)
                return Array.Empty<DTOGetApplResponse>();

            var prefix = match.Groups["prefix"].Value;
            var number = match.Groups["number"].Value;
            var suffix = match.Groups["suffix"].Value;

            var query =
                from appl in _db.trnApplications.AsNoTracking()
                join prefixTable in _db.MArmyPrefixes.AsNoTracking() on appl.ArmyPrefix equals prefixTable.Id
                join statusName in _db.StatusTable.AsNoTracking() on appl.StatusCode equals statusName.StatusCode
                join applType in _db.MApplicationTypes.AsNoTracking() on appl.ApplicationType equals applType.ApplicationTypeId
                join rank in _db.MRanks.AsNoTracking() on appl.DdlRank equals rank.RankId
                where prefixTable.Prefix == prefix
                      && appl.Number == number
                      && (appl.Suffix ?? "") == suffix
                orderby appl.UpdatedOn descending
                select new DTOGetApplResponse
                {
                    ApplicationId = appl.ApplicationId,
                    ArmyNo = (prefixTable.Prefix ?? "") + (appl.Number ?? "") + (appl.Suffix ?? ""),
                    Name = $"{rank.RankName ?? string.Empty} {appl.ApplicantName ?? string.Empty}".Trim(),
                    ApplicationType = applType.ApplicationTypeName,
                    PresentStatus = statusName.StatusName,
                    UpdatedOn = appl.UpdatedOn
                };

            return await query.ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<DTOGetApplResponse>> GetApplicantHistoryMaturityAsync(string armyNo, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(armyNo))
                return Array.Empty<DTOGetApplResponse>();

            var normalized = armyNo.Trim().ToUpperInvariant();
            var match = ArmyNoRegex.Match(normalized);

            if (!match.Success)
                return Array.Empty<DTOGetApplResponse>();

            var prefix = match.Groups["prefix"].Value;
            var number = match.Groups["number"].Value;
            var suffix = match.Groups["suffix"].Value;

          
            var query =
                from appl in _db.trnClaim.AsNoTracking()
                join prefixTable in _db.MArmyPrefixes.AsNoTracking() on appl.ArmyPrefix equals prefixTable.Id
                join statusName in _db.StatusTable.AsNoTracking() on appl.StatusCode equals statusName.ClaimStatusCode
                join applType in _db.WithdrawalPurpose.AsNoTracking() on appl.WithdrawPurpose equals applType.Id
                where prefixTable.Prefix == prefix
                     && appl.Number == number
                     && (appl.Suffix ?? "") == suffix
                orderby appl.UpdatedOn descending
                select new DTOGetApplResponse
                {
                    ApplicationId = appl.ApplicationId,
                    ArmyNo = (prefixTable.Prefix ?? "") + (appl.Number ?? "") + (appl.Suffix ?? ""),
                    Name =  appl.ApplicantName,
                    ApplicationType = applType.Name,
                    PresentStatus = statusName.StatusName,
                    UpdatedOn = appl.UpdatedOn
                };

            return await query.ToListAsync(cancellationToken);
        }
    }
}
