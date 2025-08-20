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
    public class HomeDL : IHome
    {
        private readonly ApplicationDbContext _context;
        public HomeDL(ApplicationDbContext _context)
        {
            this._context = _context;
        }

        public async Task<List<DTOUserCountResponse>> GetApplicationCount(int userId)
        {
            var counts = await (
                from fwd in _context.TrnFwdCO
                join appl in _context.trnApplications
                    on fwd.ApplicationId equals appl.ApplicationId
                where fwd.COUserId == userId &&
                      (appl.StatusCode == 1 || appl.StatusCode == 2 || appl.StatusCode == 3)
                group appl by appl.StatusCode into g
                select new DTOUserCountResponse
                {
                    Status = g.Key == 1 ? "Pending"
                           : g.Key == 2 ? "Approved"
                           : g.Key == 3 ? "Rejected"
                           : "Unknown",
                    Count = g.Count()
                }
            ).ToListAsync();

            return counts;
        }

        public async Task<List<DTOApprovedLogs>> GetApprovedLogs()
        {
            var res = await(
                from logs in _context.TrnApprovedLogs
                orderby logs.UpdatedOn descending
                select new DTOApprovedLogs
                {
                    Name = logs.Name,
                    DomainId = logs.DomainId,
                    IpAddress = logs.IpAddress,
                    IsApproved = logs.IsApproved,
                    UpdatedOn=logs.UpdatedOn
                }).ToListAsync();
            return res;
        }

        public async Task<List<DTOUserCountResponse>> GetUserCount()
        {
            var counts = await _context.trnUserMappings
                .GroupBy(m => m.IsActive)
                .Select(g => new DTOUserCountResponse
                {
                    Status = g.Key == true ? "Active" : "Inactive",
                    Count = g.Count()
                })
                .ToListAsync();

            return counts;
        }
    }
}
