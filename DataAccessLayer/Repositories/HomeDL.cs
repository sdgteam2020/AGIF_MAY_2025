using DataAccessLayer.Interfaces;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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

            var loanStats = await (from app in _context.trnApplications
                             join car in _context.trnCar
                             on app.ApplicationId equals car.ApplicationId
                             join mt in _context.MLoanTypes
                             on car.Veh_Loan_Type equals mt.LoanTypeCode
                             where app.IsActive == true && car.IsActive == true && car.UpdatedOn.Value.Year == year
                             group car by new { car.Veh_Loan_Type, mt.LoanType } into g
                             orderby g.Count() descending
                             select new DTOAnalyticsResponse
                             {
                                 VehLoanType = g.Key.Veh_Loan_Type.ToString(),
                                 LoanType = g.Key.LoanType,
                                 LoanCount = g.Count()
                             }).ToListAsync();


                var topUnits = await (from app in _context.trnApplications
                            join u in _context.MUnits
                            on app.PresentUnit equals u.UnitId
                            where app.IsActive == true && app.UpdatedOn.Value.Year == year
                            group app by u.UnitName into g
                            orderby g.Count() descending
                            select new DTOAnalyticsResponse
                            {
                                UnitName = g.Key,
                                TotalApplications = g.Count()
                            })
                           .Take(10)
                           .ToListAsync();

            // Step 1: Get all loan data in one query
            var loanData = await (
                from a in _context.trnApplications.AsNoTracking()
                where a.IsActive
                select new
                {
                    a.ApplicationId,
                    a.PresentUnit,
                    HbaLoans = _context.trnHBA
                        .Where(h => h.IsActive && h.ApplicationId == a.ApplicationId && h.UpdatedOn.Value.Year == year)
                        .Sum(h => (decimal?)h.HBA_Amount_Applied_For_Loan) ?? 0m,
                    CarLoans = _context.trnCar
                        .Where(c => c.IsActive && c.ApplicationId == a.ApplicationId && c.UpdatedOn.Value.Year == year)
                        .Sum(c => (decimal?)c.CA_Amount_Applied_For_Loan) ?? 0m,
                    PcaLoans = _context.trnPCA
                        .Where(p => p.IsActive && p.ApplicationId == a.ApplicationId && p.UpdatedOn.Value.Year == year)
                        .Sum(p => (decimal?)p.PCA_Amount_Applied_For_Loan) ?? 0m
                }
            ).ToListAsync(); // Execute query here

            // Step 2: Group in memory
            var aggregatedData = loanData
                .GroupBy(x => x.PresentUnit)
                .Select(g => new
                {
                    PresentUnit = g.Key,
                    TotalHbaLoan = g.Sum(x => x.HbaLoans),
                    TotalCarLoan = g.Sum(x => x.CarLoans),
                    TotalPcaLoan = g.Sum(x => x.PcaLoans),
                    TotalLoanAmount = g.Sum(x => x.HbaLoans + x.CarLoans + x.PcaLoans),
                    TotalApplications = g.Count()
                })
                .OrderByDescending(x => x.TotalLoanAmount)
                .Take(10)
                .ToList();

            // Step 3: Get unit names
            var unitIds = aggregatedData.Select(x => x.PresentUnit).ToList();
            var units = await _context.MUnits
                .AsNoTracking()
                .Where(u => unitIds.Contains(u.UnitId))
                .ToDictionaryAsync(u => u.UnitId, u => u.UnitName);

            // Step 4: Map to final result
            var result = aggregatedData
                .Select(r => new DTOAnalyticsResponse
                {
                    UnitName = units.ContainsKey(r.PresentUnit) ? units[r.PresentUnit] : "Unknown",
                    TotalHbaLoan = r.TotalHbaLoan,
                    TotalCarLoan = r.TotalCarLoan,
                    TotalPcaLoan = r.TotalPcaLoan,
                    TotalLoanAmount = r.TotalLoanAmount,
                    TotalApplications = r.TotalApplications
                })
                .ToList();


            // Step 1: Get top HBA dealers
            var hbaDealers = await (
                from h in _context.trnHBA
                join t in _context.trnApplications on h.ApplicationId equals t.ApplicationId
                where h.IsActive && h.UpdatedOn.Value.Year == year && !string.IsNullOrEmpty(h.PropertySeller)
                group h by h.PropertySeller into g
                select new DTOAnalyticsResponse
                {
                    DealerName = g.Key,
                    TotalApplications = g.Count()
                }
            ).ToListAsync();

            // Step 2: Get top Car dealers
            var carDealers = await (
                from c in _context.trnCar
                join t in _context.trnApplications on c.ApplicationId equals t.ApplicationId
                where c.IsActive && c.UpdatedOn.Value.Year == year
                group c by c.DealerName into g
                select new DTOAnalyticsResponse
                {
                    DealerName = g.Key,
                    TotalApplications = g.Count()
                }
            ).ToListAsync();

            // Step 3: Get top PCA dealers
            var pcaDealers = await (
                from p in _context.trnPCA
                join t in _context.trnApplications on p.ApplicationId equals t.ApplicationId
                where p.IsActive && p.UpdatedOn.Value.Year == year
                group p by p.PCA_dealerName into g
                select new DTOAnalyticsResponse
                {
                    DealerName = g.Key,
                    TotalApplications = g.Count()
                }
            ).ToListAsync();

            // Step 4: Merge and group by DealerName, then take top 10
            var dealers = hbaDealers
                .Concat(carDealers)
                .Concat(pcaDealers)
                .GroupBy(d => d.DealerName)
                .Select(g => new DTOAnalyticsResponse
                {
                    DealerName = g.Key,
                    TotalApplications = g.Sum(x => x.TotalApplications)
                })
                .OrderByDescending(x => x.TotalApplications)
                .Take(10)
                .ToList();

            // Step 1: Car Loans
            var carLoans = await (
                from c in _context.trnCar
                join t in _context.trnApplications on c.ApplicationId equals t.ApplicationId
                where c.IsActive && c.UpdatedOn.Value.Year == year
                group c by c.DealerName into g
                select new DTOAnalyticsResponse
                {
                    DealerName = g.Key,
                    TotalLoanAmount = g.Sum(x => x.CA_Amount_Applied_For_Loan ?? 0)
                }
            ).ToListAsync();

            // Step 2: PCA Loans
            var pcaLoans = await (
                from p in _context.trnPCA
                join t in _context.trnApplications on p.ApplicationId equals t.ApplicationId
                where p.IsActive && p.UpdatedOn.Value.Year == year
                group p by p.PCA_dealerName into g
                select new DTOAnalyticsResponse
                {
                    DealerName = g.Key,
                    TotalLoanAmount = g.Sum(x => x.PCA_Amount_Applied_For_Loan ?? 0)
                }
            ).ToListAsync();

            // Step 3: HBA Loans
            var hbaLoans = await (
                from h in _context.trnHBA
                join t in _context.trnApplications on h.ApplicationId equals t.ApplicationId
                where h.IsActive && h.UpdatedOn.Value.Year == year
                group h by h.PropertySeller into g
                select new DTOAnalyticsResponse
                {
                    DealerName = g.Key,
                    TotalLoanAmount = g.Sum(x => x.HBA_Amount_Applied_For_Loan ?? 0)
                }
            ).ToListAsync();

            // Step 4: Merge and group by DealerName, then take top 10
            var combinedLoans = hbaLoans
                .Concat(carLoans)
                .Concat(pcaLoans)
                .GroupBy(d => d.DealerName)
                .Select(g => new DTOAnalyticsResponse
                {
                    DealerName = g.Key,
                    TotalLoanAmount = g.Sum(x => x.TotalLoanAmount)
                })
                .OrderByDescending(x => x.TotalLoanAmount)
                .Take(10)
                .ToList();

            return new DTOAnalyticsResult
            {
                MonthlyApplications = combined,
                TopRanks = topRanks,
                TopRegiments = topRegt,
                loanStats = loanStats,
                topUnits=topUnits,
                topUnitsByLoanAmount = result,
                topDealers=dealers,
                topLoanDealers= combinedLoans
            };
        }
    }

}
