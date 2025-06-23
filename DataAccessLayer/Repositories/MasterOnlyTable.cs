using DataAccessLayer.Interfaces;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class MasterOnlyTable : IMasterOnlyTable
    {
        protected new readonly ApplicationDbContext _context;

        public MasterOnlyTable(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DTOMasterResponse>> GetAllPreFix()
        {
            var ret = await (from pre in _context.MArmyPrefixes
                             orderby pre.UserType,pre.Id
                             select new DTOMasterResponse
                             {
                                 Id = pre.Id,
                                 Name = pre.Prefix

                             }).ToListAsync();

            return ret;
        }
        public async Task<List<DTOMasterResponse>> GetAllPreFixByType(int type)
        {
            if (type == 1)
            {
                var ret = await (from pre in _context.MArmyPrefixes
                                 where pre.UserType == 1 || pre.UserType == 2
                                 select new DTOMasterResponse
                                 {
                                     Id = pre.Id,
                                     Name = pre.Prefix

                                 }).ToListAsync();

                return ret;
            }
            else if (type == 2)
            {
                var ret = await (from pre in _context.MArmyPrefixes
                                 where pre.UserType == 3
                                 select new DTOMasterResponse
                                 {
                                     Id = pre.Id,
                                     Name = pre.Prefix

                                 }).ToListAsync();

                return ret;
            }
            else
            {
                var ret = await (from pre in _context.MArmyPrefixes
                                 where pre.UserType == 4
                                 select new DTOMasterResponse
                                 {
                                     Id = pre.Id,
                                     Name = pre.Prefix

                                 }).ToListAsync();

                return ret;
            }

        }
        public async Task<List<DTOMasterResponse>> GetAllRegtCorps()
        {
            var ret = await (from regt in _context.MRegtCorps
                             select new DTOMasterResponse
                             {
                                 Id = regt.Id,
                                 Name = regt.RegtName

                             }).ToListAsync();

            return ret;
        }
        public async Task<List<DTOMasterResponse>> GetAllUnit()
        {
            var ret = await (from unit in _context.MUnits
                             select new DTOMasterResponse
                             {
                                 Id = unit.UnitId,
                                 Name = unit.UnitName

                             }).ToListAsync();

            return ret;
        }
        public async Task<List<DTOMasterResponse>> GetAllRankByType(int type)
        {
            var ret = await (from rank in _context.MRanks
                             where rank.ApplyForId == type
                             orderby rank.Orderby
                             select new DTOMasterResponse
                             {
                                 Id = rank.RankId,
                                 Name = rank.RankName

                             }).ToListAsync();

            return ret;
        }
        public async Task<List<DTOMasterResponse>> GetApplicationType()
        {
            var ret = await (from applicationType in _context.MApplicationTypes
                             select new DTOMasterResponse
                             {
                                 Id = applicationType.ApplicationTypeId,
                                 Name = applicationType.ApplicationTypeName

                             }).ToListAsync();

            return ret;
        }
        public async Task<List<DTOMasterResponse>> GetApplicantType()
        {
            var ret = await (from applicantType in _context.MApplicantTypes
                             select new DTOMasterResponse
                             {
                                 Id = applicantType.ApplicantTypeId,
                                 Name = applicantType.ApplicantTypeName

                             }).ToListAsync();

            return ret;
        }
        public async Task<List<DTOMasterResponse>> GetRetirementAge(int rankId)
        {
            var ret = await (from rank in _context.MRanks
                             where rankId == rank.RankId
                             select new DTOMasterResponse
                             {
                                 RetirementAge = Convert.ToInt16(rank.RetirementAge)

                             }).ToListAsync();

            return ret;
        }
        public async Task<List<DTOMasterResponse>> GetUserType(int Prefix)
        {
            var ret = await (from prefix in _context.MArmyPrefixes
                             where Prefix == prefix.Id
                             select new DTOMasterResponse
                             {
                                 UserType = Convert.ToInt16(prefix.UserType)

                             }).ToListAsync();

            return ret;
        }
        public async Task<List<DTOMasterResponse>> GetPCDA_PAO(int regt)
        {
            var ret = await (from regtCorps in _context.MRegtCorps
                             where regt == regtCorps.Id
                             select new DTOMasterResponse
                             {
                                 Pcda_Pao = regtCorps.PCDA_PAO

                             }).ToListAsync();

            return ret;
        }
        public async Task<List<DTOMasterResponse>> GetArmyPostOffice()
        {
            var ret = await (from armyPostOffice in _context.MArmyPostOffices
                             select new DTOMasterResponse
                             {
                                 Id = armyPostOffice.Id,
                                 Name = armyPostOffice.ArmyPostOffice,

                             }).ToListAsync();

            return ret;
        }
        public async Task<List<DTOMasterResponse>> GetLoanFreq()
        {
            var ret = await (from loanFreq in _context.MLoanFreqs
                             select new DTOMasterResponse
                             {
                                 Id = loanFreq.ID,
                                 Name = Convert.ToString(loanFreq.LoanFreq),

                             }).ToListAsync();

            return ret;
        }
        public async Task<List<DTOMasterResponse>> GetLoanType(int type)
        {
            var ret = await (from loanType in _context.MLoanTypes
                             where loanType.ApplicationType == type
                             select new DTOMasterResponse
                             {
                                 Id = loanType.LoanTypeCode,
                                 Name = Convert.ToString(loanType.LoanType),

                             }).ToListAsync();

            return ret;
        }
        public async Task<List<DTOMasterResponse>> GetAppointment()
        {
            var ret = await (from apptType in _context.MAppointments
                             select new DTOMasterResponse
                             {
                                 Id = apptType.ApptId,
                                 Name = Convert.ToString(apptType.AppointmentName),

                             }).ToListAsync();

            return ret;
        }

        public async Task<List<DTOMasterResponse>> GetALLByUnitName(string UnitName)
        {
            var units = await _context.MUnits.Where(u => u.UnitName.ToLower().Contains(UnitName.ToLower()))
                        .Select(u => new DTOMasterResponse
                         {
                            Id = u.UnitId,
                            Name = u.UnitName,
                            Pcda_Pao=u.Suffix+""+ u.Sus_no
                        })
                         .ToListAsync();

            return units;
        }

        public async Task<List<DTOMasterResponse>> GetPurposeOfWithdrawal()
        {
            var ret = await (from WithdrawalPurpose in _context.WithdrawalPurpose
                             select new DTOMasterResponse
                             {
                                 Id = WithdrawalPurpose.Id,
                                 Name = Convert.ToString(WithdrawalPurpose.Name),

                             }).ToListAsync();

            return ret;
        }
    }
}
