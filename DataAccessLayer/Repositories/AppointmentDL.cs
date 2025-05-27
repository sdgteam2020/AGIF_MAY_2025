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
    public class AppointmentDL : GenericRepositoryDL<MAppointment>, IAppointment
    {
        protected new readonly ApplicationDbContext _context;

        public AppointmentDL(ApplicationDbContext context):base(context)
        {
            _context = context;

        }
       
    }
}
