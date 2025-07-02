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
    public class EducationDL : GenericRepositoryDL<EducationDetailsModel>, IEducation
    {
        protected new readonly ApplicationDbContext _context;

        public EducationDL(ApplicationDbContext context) : base(context)
        {
            _context = context;

        }

        public async Task<EducationDetailsModel?> GetByApplicationId(int ApplicationId)
        {
            return await _context.trnEducationDetails
                .Where(x => x.ApplicationId == ApplicationId).FirstOrDefaultAsync();
        }
    }
}
