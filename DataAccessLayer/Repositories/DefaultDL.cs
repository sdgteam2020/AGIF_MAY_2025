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
        protected new readonly ApplicationDbContext _context;
        public DefaultDL(ApplicationDbContext _context)
        {
            this._context = _context;
        }

        public async Task<List<DTOApplicationStatusResponse>> GetTimeLine(int applicationId)
        {
            var timeLine = await (from statusCtr in _context.TrnStatusCounter
                                  join status in _context.StatusTable on statusCtr.StatusId equals status.StatusId
                                  where statusCtr.ApplicationId == applicationId
                                  orderby statusCtr.ActionOn
                                  select new DTOApplicationStatusResponse
                                  {
                                      StatusId = status.StatusId,
                                      Status = status.StatusName,
                                      timeLine = statusCtr.ActionOn.HasValue
                                          ? statusCtr.ActionOn.Value.ToString("dd-MM-yyyy HH:mm")
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
                                }).ToListAsync();

            return await Task.FromResult(applications);
        }

    }
}
