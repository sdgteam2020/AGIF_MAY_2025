using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ClaimDigitalDL: GenericRepositoryDL<ClaimDigitalSignRecords>, IClaimApplication
    {
        private readonly ApplicationDbContext _context;
        public ClaimDigitalDL(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
