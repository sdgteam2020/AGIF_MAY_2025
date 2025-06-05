using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class UserMappingDL:GenericRepositoryDL<UserMapping>,IUserMapping
    {
        protected new readonly ApplicationDbContext _context;

        public UserMappingDL(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        
        // Additional methods specific to UserMapping can be added here
    }
}
