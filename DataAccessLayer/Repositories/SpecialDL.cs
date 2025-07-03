using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class SpecialDL : GenericRepositoryDL<SplWaiverModel>, ISpecial
    {
        protected new readonly ApplicationDbContext _context;

        public SpecialDL(ApplicationDbContext context) : base(context)
        {
            _context = context;

        }

        public async Task<SplWaiverModel?> GetByApplicationId(int ApplicationId)
        {
            return await _context.trnSplWaiver.FirstOrDefaultAsync(x => x.ApplicationId == ApplicationId);
        }
    }
}
