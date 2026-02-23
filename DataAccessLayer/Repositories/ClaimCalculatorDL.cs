using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using Microsoft.EntityFrameworkCore;


namespace DataAccessLayer.Repositories
{
    public class ClaimCalculatorDL : GenericRepositoryDL<InvestmentChange_JCO_OR>, IClaimCalculator
    {
        protected new readonly ApplicationDbContext _context;
        public ClaimCalculatorDL(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }















































































        public async Task<(decimal currentBalance, decimal balCount, decimal saveEL)> CalculateTotalInvestment(int month, int year, int categoryValue, int? commissionMonth, int? commissionYear)
        {
            var investmentRates = new List<InvestmentChange_JCO_OR>();
            var officersInvestmentRates = new List<InvestmentChange_Officers>();

            investmentRates = await GetInvestmentRatesAsync();
            officersInvestmentRates = await GetOfficersInvestmentRatesAsync();

            var joiningDate = new DateTime(year, month, 1);

            DateTime? commissionDate = null;
            if (commissionMonth.HasValue && commissionYear.HasValue)
            {
                commissionDate = new DateTime(commissionYear.Value, commissionMonth.Value, 1);
            }

            var today = DateTime.Today;
          var tillDate = new DateTime(today.Year, today.Month, 1).AddDays(-1);

            if (joiningDate > tillDate || commissionDate > tillDate)
            {
                throw new ArgumentException("Joining date cannot be after current month");
            }

            if(commissionDate!=joiningDate)
            {
                if (commissionDate.HasValue && commissionDate.Value <= joiningDate)
                {
                    throw new ArgumentException("Commission date must be after joining date");
                }
            }
           

            decimal currentBalance = 0;
            decimal previousBalance = 0;
            decimal Balcount = 0;
            decimal newbalance = 0;
            decimal newcurrentbalance = 0;
            decimal SaveEL = 0;
            var currentDate = joiningDate;

            while (currentDate <= tillDate)
            {
                InvestmentChange_JCO_OR applicableRate = null;
                InvestmentChange_Officers applicableRateOfficers = null;
                int effectiveCategoryValue = categoryValue;
                bool isCommissionMonth = false;

                if (categoryValue == 1 && commissionDate.HasValue)
                {
                    if (currentDate.Year == commissionDate.Value.Year &&
                        currentDate.Month == commissionDate.Value.Month)
                    {
                        isCommissionMonth = true;
                        applicableRateOfficers = GetApplicableRateOfficers(officersInvestmentRates, currentDate);
                    }
                    else if (currentDate < commissionDate.Value)
                    {
                        effectiveCategoryValue = 2;
                        applicableRate = GetApplicableRate(investmentRates, currentDate);
                    }
                    else
                    {
                        applicableRateOfficers = GetApplicableRateOfficers(officersInvestmentRates, currentDate);
                    }
                }
                else if (categoryValue == 1)
                {
                    applicableRateOfficers = GetApplicableRateOfficers(officersInvestmentRates, currentDate);
                }
                else
                {
                    applicableRate = GetApplicableRate(investmentRates, currentDate);
                }

                if (isCommissionMonth && applicableRate != null && applicableRateOfficers != null)
                {
                    SaveEL += applicableRate.InvestmentAmount;
                    previousBalance = currentBalance;
                    currentBalance += applicableRate.InvestmentAmount;
                    Balcount += applicableRate.PrAmount;

                    decimal jcoBalance = Balcount - applicableRate.PrAmount;

                    var jcoMonthlyFactor = (decimal)Math.Pow((double)(1 + applicableRate.InterestRate / 100), 1.0 / 12.0);
                    decimal jcoCurrentBalance = currentBalance * jcoMonthlyFactor;

                    var jcoBonus = await GetBonusAmount(previousBalance, currentDate, jcoCurrentBalance, jcoMonthlyFactor, 2, jcoBalance);

                    if (jcoBonus != 0)
                    {
                        currentBalance = jcoBonus;
                    }
                    else
                    {
                        currentBalance = jcoCurrentBalance;
                    }

                    SaveEL += applicableRateOfficers.InvestmentAmount;
                    previousBalance = currentBalance; // Use balance after JCO/OR processing
                    currentBalance += applicableRateOfficers.InvestmentAmount;
                    Balcount += applicableRateOfficers.PrAmount;

                    decimal officerBalance = Balcount - applicableRateOfficers.PrAmount;

                    var officerMonthlyFactor = (decimal)Math.Pow((double)(1 + applicableRateOfficers.InterestRate / 100), 1.0 / 12.0);
                    decimal officerCurrentBalance = currentBalance * officerMonthlyFactor;

                    var officerBonus = await GetBonusAmount(previousBalance, currentDate, officerCurrentBalance, officerMonthlyFactor, 1, officerBalance);

                    if (officerBonus != 0)
                    {
                        currentBalance = officerBonus;
                    }
                    else
                    {
                        currentBalance = officerCurrentBalance;
                    }
                }
                else if (applicableRate != null)
                {
                    SaveEL = applicableRate.InvestmentAmount + SaveEL;
                    previousBalance = currentBalance;
                    currentBalance += applicableRate.InvestmentAmount;

                    Balcount = Balcount + applicableRate.PrAmount;
                    newbalance = Balcount - applicableRate.PrAmount;

                    var monthlyFactor = (decimal)Math.Pow((double)(1 + applicableRate.InterestRate / 100), 1.0 / 12.0);
                    newcurrentbalance = currentBalance * monthlyFactor;

                    var balance = await GetBonusAmount(previousBalance, currentDate, newcurrentbalance, monthlyFactor, 2, newbalance);

                    if (balance != 0)
                    {
                        currentBalance = balance;
                    }
                    else
                    {
                        currentBalance = newcurrentbalance;
                    }
                }
                else if (applicableRateOfficers != null)
                {
                    SaveEL = applicableRateOfficers.InvestmentAmount + SaveEL;
                    previousBalance = currentBalance;
                    currentBalance += applicableRateOfficers.InvestmentAmount;

                    Balcount = Balcount + applicableRateOfficers.PrAmount;
                    newbalance = Balcount - applicableRateOfficers.PrAmount;

                    var monthlyFactor = (decimal)Math.Pow((double)(1 + applicableRateOfficers.InterestRate / 100), 1.0 / 12.0);
                    newcurrentbalance = currentBalance * monthlyFactor;

                    var balance = await GetBonusAmount(previousBalance, currentDate, newcurrentbalance, monthlyFactor, 1, newbalance);

                    if (balance != 0)
                    {
                        currentBalance = balance;
                    }
                    else
                    {
                        currentBalance = newcurrentbalance;
                    }
                }

                currentDate = currentDate.AddMonths(1);
            }

            return (
                currentBalance: Math.Round(currentBalance, 2),
                balCount: Math.Round(Balcount, 2),
                saveEL: Math.Round(SaveEL, 2)
            );
        }
        private InvestmentChange_JCO_OR? GetApplicableRate(List<InvestmentChange_JCO_OR> rates, DateTime forDate)
        {
            return rates
                .Where(r => r.ChangeDate <= forDate)
                .OrderByDescending(r => r.ChangeDate)
                .FirstOrDefault();



        }


        private InvestmentChange_Officers? GetApplicableRateOfficers(List<InvestmentChange_Officers> rates, DateTime forDate)
        {
            return rates
                .Where(r => r.ChangeDate <= forDate)
                .OrderByDescending(r => r.ChangeDate)
                .FirstOrDefault();
        }

        public async Task<List<InvestmentChange_JCO_OR>> GetInvestmentRatesAsync()
        {
            return await _context.TrnInvestmentChange_JCO_OR
                .OrderBy(x => x.ChangeDate)
                .ToListAsync();
        }

        public async Task<List<InvestmentChange_Officers>> GetOfficersInvestmentRatesAsync()
        {
            return await _context.TrnInvestmentChange_Officers
                .OrderBy(x => x.ChangeDate)
                .ToListAsync();
        }

        public async Task<decimal> GetBonusAmount(decimal PreviousBalance, DateTime Currentdate, decimal Currentbalance, decimal Currentrate,int categoryValue,decimal newbalance)
        {
            var bonusRecord= new BonusJCO_OR();
            var bonusRecordOfficers = new BonusOfficers();
            bonusRecord = null;
            bonusRecordOfficers = null;

            if (categoryValue==1)
            {
                bonusRecordOfficers = await _context.TrnBonusOfficers
                .Where(b => EF.Functions.DateDiffDay(b.Date, Currentdate) == 0)
                .FirstOrDefaultAsync();
            }
            else
            {
                bonusRecord= await _context.TrnBonusJCO_OR
                .Where(b => EF.Functions.DateDiffDay(b.Date, Currentdate) == 0)
                .FirstOrDefaultAsync();
            }

            var cutoff = new DateTime(1999, 3, 1); 
            var newdate = new DateTime(2018, 1, 1);

            decimal BonusAmount = 0;  

            if (categoryValue==1 && bonusRecordOfficers != null)
            {
                if (Currentdate > cutoff)
                {
                    BonusAmount = (newbalance * bonusRecordOfficers.AnnualBonus) / 100;
                }
                else
                {
                    BonusAmount = (PreviousBalance * bonusRecordOfficers.AnnualBonus) / 100;
                }
                

                if (Currentdate == newdate)
                {
                    BonusAmount = Currentbalance - ((BonusAmount + bonusRecordOfficers.CumulativeBonus) * Currentrate);
                }
                else
                {
                    BonusAmount = Currentbalance + ((BonusAmount + bonusRecordOfficers.CumulativeBonus) * Currentrate);
                }

                return BonusAmount;
            }
            else if (bonusRecord != null)
            {
                if(Currentdate > cutoff)
                {
                    BonusAmount = (newbalance * bonusRecord.AnnualBonus) / 100;
                }
                else
                {
                    BonusAmount = (PreviousBalance * bonusRecord.AnnualBonus) / 100;
                }

                if(Currentdate == newdate)
                {
                    BonusAmount = Currentbalance - ((BonusAmount + bonusRecord.CumulativeBonus) * Currentrate);
                }
                else
                {
                    BonusAmount = Currentbalance + ((BonusAmount + bonusRecord.CumulativeBonus) * Currentrate);
                }
                return BonusAmount;
            }


            return 0;
        }

    }
}
