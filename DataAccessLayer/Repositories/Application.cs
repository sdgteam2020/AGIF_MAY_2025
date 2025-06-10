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
    public class Application : Interfaces.IApplication
    {
        private readonly ApplicationDbContext _context;
        public Application(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<DTOGetApplResponse>> GetApplicationsAsync(DTOGetApplRequest dTOGetAppl)
        {
            var query = _context.Applications;
            if (dTOGetAppl.status > 0)
            {
                query = query;
            }

            var applications = await query.Select(app => new DTOGetApplResponse
            {
                ApplicationId = app.ApplicationId,
                Name = app.ApplicantName,
            }).ToListAsync();

            return applications; // Ensure ToListAsync is available
        }
    }
}
