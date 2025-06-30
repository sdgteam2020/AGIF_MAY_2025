using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class UsersApplicationDL : IUsersApplications
    {
        protected readonly ApplicationDbContext _db;
        public UsersApplicationDL(ApplicationDbContext db)
        {
            _db = db;
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
            var UsersApplicationList = await (from appl in _db.trnApplications
                                   join user in _db.trnUserMappings on appl.PresentUnit equals user.UnitId
                                   join prefix in _db.MArmyPrefixes on appl.ArmyPrefix equals prefix.Id
                                   join applType in _db.MApplicationTypes on appl.ApplicationType equals applType.ApplicationTypeId
                                   where user.MappingId == Mapping && appl.StatusCode == status && user.IsPrimary == true
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
                                   where appl.StatusCode == status
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
            //throw new NotImplementedException();
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
                                                 UpdatedOn = appl.UpdatedOn
                                             }).ToListAsync();
            return UsersApplicationListToAdmin!;
        }
    }
}
