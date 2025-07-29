using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class AccountDL: GenericRepositoryDL<AccountDetailsModel>, IAccount
    {
        protected new readonly ApplicationDbContext _context;
        public AccountDL(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
    
}
