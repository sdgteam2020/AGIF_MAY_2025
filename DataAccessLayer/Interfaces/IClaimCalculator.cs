using DataTransferObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IClaimCalculator :IGenericRepositoryDL<InvestmentChange_JCO_OR>
    {
        public Task<(decimal currentBalance, decimal balCount, decimal saveEL)> CalculateTotalInvestment(int month, int year, int categoryValue);
    }
}
