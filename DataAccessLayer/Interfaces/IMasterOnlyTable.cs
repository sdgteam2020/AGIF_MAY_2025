using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IMasterOnlyTable
    {
        Task<List<DTOMasterResponse>> GetAllPreFix();
        Task<List<DTOMasterResponse>> GetAllPreFixByType(int type);
        Task<List<DTOMasterResponse>> GetAllRegtCorps();
        Task<List<DTOMasterResponse>> GetAllUnit();
        Task<List<DTOMasterResponse>> GetAllRankByType(int type);
        Task<List<DTOMasterResponse>> GetApplicationType();
        Task<List<DTOMasterResponse>> GetApplicantType();
        Task<List<DTOMasterResponse>> GetRetirementAge(int rankId);
        Task<List<DTOMasterResponse>> GetUserType(int Prefix);
        Task<List<DTOMasterResponse>> GetPCDA_PAO(int regt);
        Task<List<DTOMasterResponse>> GetArmyPostOffice();
        Task<List<DTOMasterResponse>> GetLoanFreq();
        Task<List<DTOMasterResponse>> GetLoanType(int type);
        
    }
}
