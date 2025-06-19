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

        public async Task<List<DTOGetApplResponse>> GetUsersApplication(int Mapping, int status)
        {

            var applicationList = (from appl in _db.trnApplications
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
                                   }).ToList();
            return applicationList!;
            //throw new NotImplementedException();
        }
    }
}
