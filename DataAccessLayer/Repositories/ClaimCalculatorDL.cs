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
        public async Task<(decimal currentBalance, decimal balCount, decimal saveEL)> CalculateTotalInvestment(int month, int year,int categoryValue)
        {
            var investmentRates = new List<InvestmentChange_JCO_OR>();
            var officersInvestmentRates = new List<InvestmentChange_Officers>();

            var applicableRate = new InvestmentChange_JCO_OR();
            var applicableRateOfficers = new InvestmentChange_Officers();

            applicableRate = null;
            applicableRateOfficers = null;

            if (categoryValue==1)
            {
                 officersInvestmentRates = await GetOfficersInvestmentRatesAsync();
            }
            else  
            {
                 investmentRates = await GetInvestmentRatesAsync();
            }

            // Create joining date from month and year (1st day of the month)
            var joiningDate = new DateTime(year, month, 1);

            //Calculate till date(last day of previous month from current date)
            var today = DateTime.Today;
            var tillDate = new DateTime(today.Year, today.Month, 1).AddDays(-1);

            // Validate joining date
            if (joiningDate > tillDate)
            {
                throw new ArgumentException("Joining date cannot be after current month");
            }

            decimal currentBalance = 0;
            decimal previousBalance = 0;   
            decimal Balcount = 0;
            decimal newbalance = 0;
            decimal newcurrentbalance = 0;
            decimal SaveEL = 0;
            var currentDate = joiningDate;

            // Calculate month by month until till date
            while (currentDate <= tillDate)
            {
              
                // Find applicable investment rate for this month

                if (categoryValue == 1)
                {
                    applicableRateOfficers = GetApplicableRateOfficers(officersInvestmentRates, currentDate);
                }

                else
                {
                    applicableRate = GetApplicableRate(investmentRates, currentDate);
                   
                }

                if (applicableRate != null)
                {
                    SaveEL = applicableRate.InvestmentAmount + SaveEL;
                    // Add monthly investment amount
                    previousBalance = currentBalance;
                    currentBalance += applicableRate.InvestmentAmount;

                    Balcount = Balcount + applicableRate.PrAmount;

                    newbalance = Balcount - applicableRate.PrAmount;

                    // Calculate monthly interest factor: (1 + annual_rate/100)^(1/12)
                    var monthlyFactor = (decimal)Math.Pow((double)(1 + applicableRate.InterestRate / 100), 1.0 / 12.0);

                    newcurrentbalance= currentBalance * monthlyFactor;

                    var balance = await GetBonusAmount(previousBalance, currentDate, newcurrentbalance, monthlyFactor, categoryValue, newbalance);

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
                    // Add monthly investment amount
                    SaveEL = applicableRateOfficers.InvestmentAmount + SaveEL;

                    previousBalance = currentBalance;
                    currentBalance += applicableRateOfficers.InvestmentAmount;


                    Balcount = Balcount + applicableRateOfficers.PrAmount;

                    newbalance = Balcount - applicableRateOfficers.PrAmount;

                    // Calculate monthly interest factor: (1 + annual_rate/100)^(1/12)
                    var monthlyFactor = (decimal)Math.Pow((double)(1 + applicableRateOfficers.InterestRate / 100), 1.0 / 12.0);

                    newcurrentbalance = currentBalance * monthlyFactor;

                     var balance = await GetBonusAmount(previousBalance, currentDate, newcurrentbalance, monthlyFactor, categoryValue, newbalance);

                    if (balance != 0)
                    {
                        currentBalance = balance;
                    }
                    else
                    {
                        currentBalance = newcurrentbalance;
                    }

                }
               

                // Move to next month
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
            // Find the most recent rate that's effective for the given date
            return rates
                .Where(r => r.ChangeDate <= forDate)
                .OrderByDescending(r => r.ChangeDate)
                .FirstOrDefault();
        }


        private InvestmentChange_Officers? GetApplicableRateOfficers(List<InvestmentChange_Officers> rates, DateTime forDate)
        {
            // Find the most recent rate that's effective for the given date
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
