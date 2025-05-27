using DataAccessLayer.Repositories;
using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IUnitOfWork 
    {
        IAppointment Appointment { get; }
        Task<List<DTOMasterResponse>> GetAllMMaster(DTOMasterRequest Data);

    }

}
