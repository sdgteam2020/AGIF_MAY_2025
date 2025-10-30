using DataAccessLayer.Interfaces;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class DefaultDL : IDefault
    {
        protected readonly ApplicationDbContext _context;
        public DefaultDL(ApplicationDbContext _context)
        {
            this._context = _context;
        }

        public async Task<List<DTOApplicationStatusResponse>> GetTimeLine(int applicationId)
        {
            var timeLine = await (from statusCtr in _context.TrnStatusCounter
                                  join status in _context.StatusTable on statusCtr.StatusId equals status.StatusId
                                  where statusCtr.ApplicationId == applicationId
                                  orderby statusCtr.ActionOn descending
                                  select new DTOApplicationStatusResponse
                                  {
                                      StatusId = status.StatusId,
                                      Status = status.StatusName,
                                      timeLine = statusCtr.ActionOn.HasValue
                                          ? statusCtr.ActionOn.Value.ToString("dd-MM-yyyy")
                                          : ""
                                  }).ToListAsync();

            return timeLine;
        }
        public async Task<List<DTOApplicationStatusResponse>> GetClaimTimeLine(int applicationId)
        {
            var timeLine = await (from statusCtr in _context.TrnClaimStatusCounter
                                  join status in _context.StatusTable on statusCtr.StatusId equals status.ClaimStatusCode
                                  where statusCtr.ApplicationId == applicationId
                                  orderby statusCtr.ActionOn descending
                                  select new DTOApplicationStatusResponse
                                  {
                                      StatusId = status.StatusId,
                                      Status = status.StatusName,
                                      timeLine = statusCtr.ActionOn.HasValue
                                          ? statusCtr.ActionOn.Value.ToString("dd-MM-yyyy")
                                          : ""
                                  }).ToListAsync();

            return timeLine;
        }

        public async Task<List<DTOApplicationStatusResponse>> GetUserApplicationStatusByArmyNo(string armyNo)
        {
            var applications = await (from appl in _context.trnApplications
                                join prefix in _context.MArmyPrefixes on appl.ArmyPrefix equals prefix.Id
                                join applicationType in _context.MApplicationTypes on appl.ApplicationType equals applicationType.ApplicationTypeId
                                join status in _context.StatusTable on appl.StatusCode equals status.StatusCode
                                where (prefix.Prefix + appl.Number + appl.Suffix) == armyNo
                                select new DTOApplicationStatusResponse
                                {
                                    ApplicationId = appl.ApplicationId,
                                    ApplicationType = applicationType.ApplicationTypeName,
                                    Status = status.StatusName.ToString(),
                                    StatusId = status.StatusId,
                                    Remarks= appl.AGIFRemarks ?? string.Empty
                                }).ToListAsync();

            return await Task.FromResult(applications);
        }

        public async Task<List<DTOApplicationStatusResponse>> GetClaimUserApplicationStatusByArmyNo(string armyNo)
        {
            var applications = await (from appl in _context.trnClaim
                                      join prefix in _context.MArmyPrefixes on appl.ArmyPrefix equals prefix.Id
                                     join applicationType in _context.WithdrawalPurpose on appl.WithdrawPurpose equals applicationType.Id
                                      join status in _context.StatusTable on appl.StatusCode equals status.ClaimStatusCode
                                      where (prefix.Prefix + appl.Number + appl.Suffix) == armyNo && status.ClaimStatusCode!=0
                                      select new DTOApplicationStatusResponse
                                      {
                                          ApplicationId = appl.ApplicationId,
                                          ApplicationType = applicationType.Name,
                                          Status = status.StatusName.ToString(),
                                          StatusId = status.StatusId,
                                          Remarks = appl.AGIFRemarks ?? string.Empty
                                      }).ToListAsync();

            return await Task.FromResult(applications);
        }

    }
}
