using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ErrorLogDL:GenericRepositoryDL<ErrorLog>,IErrorLog
    {
        protected new readonly ApplicationDbContext _context;
        public ErrorLogDL(ApplicationDbContext context) : base(context)
        {
            _context = context;

        }
    }
}
