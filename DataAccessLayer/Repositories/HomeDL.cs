using DataAccessLayer.Interfaces;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
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
            //chart 1
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

            //chart 2
            var topRanks = await (
                from app in _context.trnApplications
                join rank in _context.MRanks on app.DdlRank equals rank.RankId
                where app.UpdatedOn.HasValue && app.UpdatedOn.Value.Year == year
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

            //chart 3
            var topRegt = await (
                from app in _context.trnApplications
                join regt in _context.MRegtCorps on app.RegtCorps equals regt.Id
                where app.UpdatedOn.HasValue && app.UpdatedOn.Value.Year == year
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

            //chart 4
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

            //chart 5
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

            //chart 6
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

            //chart 7
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
                where c.IsActive && c.UpdatedOn.Value.Year == year && !string.IsNullOrEmpty(c.DealerName)
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
                where p.IsActive && p.UpdatedOn.Value.Year == year && !string.IsNullOrEmpty(p.PCA_dealerName)
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

            //chart 8
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

            //chart 9
           var topApplicantsByRank = await (
                                             from app in _context.trnApplications
                                             join rank in _context.MRanks
                                                 on app.DdlRank equals rank.RankId
                                             where app.IsActive && app.StatusCode == 2
                                             select new
                                             {
                                                 ApplicantRank = rank.RankName,
                                                 ApplicantName= app.ApplicantName,
                                                 // Loan counts
                                                 CarLoanCount = _context.trnCar.Count(c => c.ApplicationId == app.ApplicationId && c.IsActive && c.IsActive && c.UpdatedOn.Value.Year == year),
                                                 PcaLoanCount = _context.trnPCA.Count(p => p.ApplicationId == app.ApplicationId && p.IsActive && p.IsActive && p.UpdatedOn.Value.Year == year),
                                                 HbaLoanCount = _context.trnHBA.Count(h => h.ApplicationId == app.ApplicationId && h.IsActive && h.IsActive && h.UpdatedOn.Value.Year == year),

                                                 // Loan amounts applied
                                                 TotalCarLoanAmount = _context.trnCar
                                                                         .Where(c => c.ApplicationId == app.ApplicationId && c.IsActive && c.UpdatedOn.Value.Year == year)
                                                                         .Sum(c => (decimal?)c.CA_Amount_Applied_For_Loan) ?? 0m,

                                                 TotalPcaLoanAmount = _context.trnPCA
                                                                         .Where(p => p.ApplicationId == app.ApplicationId && p.IsActive && p.UpdatedOn.Value.Year == year)
                                                                         .Sum(p => (decimal?)p.PCA_Amount_Applied_For_Loan) ?? 0m,

                                                 TotalHbaLoanAmount = _context.trnHBA
                                                                         .Where(h => h.ApplicationId == app.ApplicationId && h.IsActive && h.UpdatedOn.Value.Year == year)
                                                                         .Sum(h => (decimal?)h.HBA_Amount_Applied_For_Loan) ?? 0m
                                             }
                                             )
                                             .Select(x => new DTOAnalyticsResponse
                                             {
                                                 Rank= x.ApplicantRank,
                                                 CACount=x.CarLoanCount,
                                                 PCACount=x.PcaLoanCount,
                                                 HBACount=x.HbaLoanCount,
                                                 LoanCount =x.CarLoanCount + x.PcaLoanCount + x.HbaLoanCount,
                                                 TotalLoanAmount = x.TotalCarLoanAmount + x.TotalPcaLoanAmount + x.TotalHbaLoanAmount,                                                   
                                                 ApplicantName=x.ApplicantName,
                                                 TotalCarLoan = x.TotalCarLoanAmount,
                                                 TotalPcaLoan = x.TotalPcaLoanAmount,
                                                 TotalHbaLoan = x.TotalHbaLoanAmount
                                             })
                                             .OrderByDescending(x => x.TotalLoanAmount)
                                             .Take(20)
                                             .ToListAsync();

            //chart 10
            var statuslist = await _context.trnApplications
                              .Where(a => a.IsActive && a.UpdatedOn.Value.Year==year)
                              .GroupBy(a => 1) // Single group to aggregate all rows
                              .Select(g => new DTOAnalyticsResponse
                              {
                                  PendingCount = g.Count(a => a.StatusCode == 1),
                                  ApprovedCount = g.Count(a => a.StatusCode == 2),
                                  RejectedCount = g.Count(a => a.StatusCode == 3)
                              })
                              .ToListAsync();
            
            var ageGroupsData = await _context.trnApplications
                                .Where(a => a.IsActive && a.UpdatedOn.Value.Year == year)
                                .Select(a => new {
                                    Age = EF.Functions.DateDiffYear(a.DateOfBirth, DateTime.Now)
                                })
                                .GroupBy(a => a.Age < 25 ? "<25" :
                                              a.Age >= 25 && a.Age <= 34 ? "25-34" :
                                              a.Age >= 35 && a.Age <= 44 ? "35-44" :
                                              a.Age >= 45 && a.Age <= 54 ? "45-54" :
                                              "55+")
                                .Select(g => new DTOAnalyticsResponse
                                {
                                    AgeGroup = g.Key,
                                    TotalApplications = g.Count()
                                })
                                 .OrderBy(x => x.AgeGroup)
                                 .ToListAsync();


            // Get top 20 applicants by number of loans
            var carApplicantLoans = await (
                                  from app in _context.trnApplications
                                  join car in _context.trnCar on app.ApplicationId equals car.ApplicationId
                                  where app.IsActive && car.IsActive
                                  select new
                                  {
                                      app.ApplicationId,
                                      app.ApplicantName,
                                      Rank = _context.MRanks.FirstOrDefault(r => r.RankId == app.DdlRank).RankName,
                                      LoanDate = car.UpdatedOn
                                  }
                                 ).ToListAsync();

            // 2️⃣ Get PCA loans per applicant
            var pcaApplicantLoans = await (
                from app in _context.trnApplications
                join pca in _context.trnPCA on app.ApplicationId equals pca.ApplicationId
                where app.IsActive && pca.IsActive
                select new
                {
                    app.ApplicationId,
                    app.ApplicantName,
                    Rank = _context.MRanks.FirstOrDefault(r => r.RankId == app.DdlRank).RankName,
                    LoanDate = pca.UpdatedOn
                }
            ).ToListAsync();

            // 3️⃣ Get HBA loans per applicant
            var hbaApplicantLoans = await (
                from app in _context.trnApplications
                join hba in _context.trnHBA on app.ApplicationId equals hba.ApplicationId
                where app.IsActive && hba.IsActive
                select new
                {
                    app.ApplicationId,
                    app.ApplicantName,
                    Rank = _context.MRanks.FirstOrDefault(r => r.RankId == app.DdlRank).RankName,
                    LoanDate = hba.UpdatedOn
                }
            ).ToListAsync();

            // 4️⃣ Combine all loan types
            var combinedApplicantLoans = carApplicantLoans
                .Concat(pcaApplicantLoans)
                .Concat(hbaApplicantLoans)
                .GroupBy(x => new { x.ApplicantName, x.Rank })
                .Select(g => new DTOAnalyticsResponse
                {
                    ApplicantName = g.Key.ApplicantName,
                    Rank = g.Key.Rank,
                    LoanDates = g.Select(x => x.LoanDate).OrderBy(d => d).ToList(),
                    LoanCount = g.Count()
                })
                .OrderByDescending(x => x.LoanCount)
                .Take(20)
                .ToList();
            
            var carLoanCountsByUnit = await (
                from unit in _context.MUnits
                join app in _context.trnApplications on unit.UnitId equals app.PresentUnit
                join car in _context.trnCar on app.ApplicationId equals car.ApplicationId
                where app.IsActive && car.IsActive
                group car by unit.UnitName into g
                select new DTOAnalyticsResponse
                {
                    UnitName = g.Key,
                    CACount = g.Count(),
                    PCACount = 0,
                    HBACount = 0
                }
            ).ToListAsync();

            var pcaLoanCountsByUnit = await (
                from unit in _context.MUnits
                join app in _context.trnApplications on unit.UnitId equals app.PresentUnit
                join pca in _context.trnPCA on app.ApplicationId equals pca.ApplicationId
                where app.IsActive && pca.IsActive
                group pca by unit.UnitName into g
                select new DTOAnalyticsResponse
                {
                    UnitName = g.Key,
                    CACount = 0,
                    PCACount = g.Count(),
                    HBACount = 0
                }
            ).ToListAsync();

            var hbaLoanCountsByUnit = await (
                from unit in _context.MUnits
                join app in _context.trnApplications on unit.UnitId equals app.PresentUnit
                join hba in _context.trnHBA on app.ApplicationId equals hba.ApplicationId
                where app.IsActive && hba.IsActive
                group hba by unit.UnitName into g
                select new DTOAnalyticsResponse
                {
                    UnitName = g.Key,
                    CACount = 0,
                    PCACount = 0,
                    HBACount = g.Count()
                }
            ).ToListAsync();

            // Combine: Merge the results by UnitName
            var combinedLoanCountsByUnit = carLoanCountsByUnit
                .Concat(pcaLoanCountsByUnit)
                .Concat(hbaLoanCountsByUnit)
                .GroupBy(x => x.UnitName)
                .Select(g => new DTOAnalyticsResponse
                {
                    UnitName = g.Key,
                    CACount = g.Sum(x => x.CACount),
                    PCACount = g.Sum(x => x.PCACount),
                    HBACount = g.Sum(x => x.HBACount)
                })
                .OrderBy(x => x.UnitName)
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
                topLoanDealers= combinedLoans,
                topPersonnel= topApplicantsByRank,
                statusCounts= statuslist,
                AgeGroups= ageGroupsData,
                MultipleLoans= combinedApplicantLoans,
                LoanTypes= combinedLoanCountsByUnit
            };
        }
    }

}
