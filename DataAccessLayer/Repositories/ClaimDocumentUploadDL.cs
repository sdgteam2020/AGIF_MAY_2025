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
    public class ClaimDocumentUploadDL : GenericRepositoryDL<ClaimDocumentUpload>, IClaimDocumentUpload
    {
        protected new readonly ApplicationDbContext _context;
        public ClaimDocumentUploadDL(ApplicationDbContext context) : base(context)
        {
            _context = context;

        }

        public async Task<bool> CheckDocumentUploaded(int ApplicationID)
        {
            var document = await _context.trnClaimDocumentUpload
                .FirstOrDefaultAsync(d => d.ApplicationId == ApplicationID);
            return document != null;
        }

    }
}
