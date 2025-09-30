using DataAccessLayer.Interfaces;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

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
            var res = await (
                from logs in _context.TrnApprovedLogs
                orderby logs.UpdatedOn descending
                select new DTOApprovedLogs
                {
                    Name = logs.Name,
                    DomainId = logs.DomainId,
                    IpAddress = logs.IpAddress,
                    CoDomainId = logs.coDomainId,
                    CoProfileId = logs.coProfileId,
                    IsApproved = logs.IsApproved,
                    UpdatedOn = logs.UpdatedOn
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


        public async Task<DTOAnalyticsResult> GetTotalMonthlyApplications(int year)
        {
            var carCounts = await (
                from car in _context.trnCar
                where car.UpdatedOn.HasValue && car.UpdatedOn.Value.Year == year
                group car by car.UpdatedOn.Value.Month into g
                select new DTOAnalyticsResponse
                {
                    Month = g.Key.ToString(),
                    CACount = g.Count(),
                    PCACount = 0,
                    HBACount = 0,
                    TotalApplications = 0
                }
            ).ToListAsync();

            var pcCounts = await (
                from pca in _context.trnPCA
                where pca.UpdatedOn.HasValue && pca.UpdatedOn.Value.Year == year
                group pca by pca.UpdatedOn.Value.Month into g
                select new DTOAnalyticsResponse
                {
                    Month = g.Key.ToString(),
                    CACount = 0,
                    PCACount = g.Count(),
                    HBACount = 0,
                    TotalApplications = 0
                }
            ).ToListAsync();

            var hbaCounts = await (
                from hba in _context.trnHBA
                where hba.UpdatedOn.HasValue && hba.UpdatedOn.Value.Year == year
                group hba by hba.UpdatedOn.Value.Month into g
                select new DTOAnalyticsResponse
                {
                    Month = g.Key.ToString(),
                    CACount = 0,
                    PCACount = 0,
                    HBACount = g.Count(),
                    TotalApplications = 0
                }
            ).ToListAsync();

            // Merge the data
            var combined = carCounts
                .Union(pcCounts)
                .Union(hbaCounts)
                .GroupBy(m => m.Month)
                .Select(g => new DTOAnalyticsResponse
                {
                    Month = g.Key,
                    CACount = g.Sum(x => x.CACount),
                    PCACount = g.Sum(x => x.PCACount),
                    HBACount = g.Sum(x => x.HBACount),
                    TotalApplications = g.Sum(x => x.CACount + x.PCACount + x.HBACount)
                })
            .OrderBy(m => int.Parse(m.Month))
            .ToList();

            var topRanks = await (
                from app in _context.trnApplications
                join rank in _context.MRanks on app.DdlRank equals rank.RankId
                group app by new { app.DdlRank, rank.RankName } into g
                orderby g.Count() descending
                select new DTOAnalyticsResponse
                {
                    Rank = g.Key.RankName,
                    RankCount = g.Count()
                }
            )
            .Take(10)
            .ToListAsync();

            var topRegt = await (
                from app in _context.trnApplications
                join regt in _context.MRegtCorps on app.RegtCorps equals regt.Id
                group app by new { app.RegtCorps, regt.RegtName } into g
                orderby g.Count() descending
                select new DTOAnalyticsResponse
                {
                    Regt = g.Key.RegtName,
                    RegtCount = g.Count()
                }
            )
            .Take(10)
            .ToListAsync();


            return new DTOAnalyticsResult
            {
                MonthlyApplications = combined,
                TopRanks = topRanks,
                TopRegiments = topRegt
            };
        }
    }

}
