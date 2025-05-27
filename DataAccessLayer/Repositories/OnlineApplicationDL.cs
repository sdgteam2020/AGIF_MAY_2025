using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class OnlineApplicationDL : GenericRepositoryDL<OnlineApplications>, IOnlineApplication
    {
        protected new readonly ApplicationDbContext _context;

        public OnlineApplicationDL(ApplicationDbContext context) : base(context)
        {
            _context = context;
           
        }

        public Task<bool> IsUser(string AadharNo)
        {
           _context.OnlineApplications.ToList();
            var user = _context.OnlineApplications.FirstOrDefault(x => x.AadharNo == AadharNo);
            if (user != null)
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
        public Task<DateTime> GetRetirementDate(int rankId, int Prefix, DateTime dateTime)
        {
            var userType = _context.MArmyPrefixes.FirstOrDefault(x => x.Id == Prefix);
            var retAge = _context.MRanks.FirstOrDefault(x => x.RankId == rankId);
            var retirementAge = retAge.RetirementAge;
            var userTypeId = userType.UserType;

            var ret = (from prefix in _context.MArmyPrefixes
                       join rank in _context.MRanks on rankId equals Prefix
                       where prefix.Id == Prefix
                       select new
                       {
                           userType = prefix.UserType,
                           retAge = rank.RetirementAge

                       });


            if (retirementAge > 0 && userTypeId != 0)
            {
                DateTime retirementDate = DateTime.Now.AddYears((int)retirementAge);
                return Task.FromResult(retirementDate);
            }
            else
            {
                return Task.FromResult(DateTime.MinValue);
            }
        }
    }

}
