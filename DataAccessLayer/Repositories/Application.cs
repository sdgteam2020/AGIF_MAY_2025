using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class Application : GenericRepositoryDL<DigitalSignRecords>, IApplication
    {
        private readonly ApplicationDbContext _context;
        public Application(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
