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
    public class PropertyDL: GenericRepositoryDL<PropertyRenovationModel>, IProperty
    {
        protected new readonly ApplicationDbContext _context;

        public PropertyDL(ApplicationDbContext context) : base(context)
        {
            _context = context;

        }

        public async Task<PropertyRenovationModel?> GetByApplicationId(int ApplicationId)
        {
            return await _context.trnPropertyRenovation.FirstOrDefaultAsync(x => x.ApplicationId == ApplicationId);
        }
    }
}
