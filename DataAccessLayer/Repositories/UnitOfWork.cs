using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
       
        public IAppointment Appointment { get; }
        public IMasterOnlyTable MasterOnlyTable { get; }
       
        public UnitOfWork(IMasterOnlyTable masterOnlyTable, IAppointment appointmentDL )
        {
            Appointment = appointmentDL;
            MasterOnlyTable = masterOnlyTable;
        }



        public async Task<List<DTOMasterResponse>> GetAllMMaster(DTOMasterRequest Data)
        {
            List<DTOMasterResponse> lst = new List<DTOMasterResponse>();
            if (Data.id == Convert.ToInt16(Constants.RankOffrs))
            {
                var ret = await MasterOnlyTable.GetAllRankByType(1);
                lst=ret;
            }
            else if (Data.id == Convert.ToInt16(Constants.RankJco))
            {
                var ret = await MasterOnlyTable.GetAllRankByType(2);
                lst = ret;
            }
            else if (Data.id == Convert.ToInt16(Constants.RankOr))
            {
                var ret = await MasterOnlyTable.GetAllRankByType(3);
                lst = ret;
            }
            else if (Data.id == Convert.ToInt16(Constants.Appt))
            {
                //var Ret = await Appointment.GetAll();
                //foreach (var Forma in Ret)
                //{

                //    DTOMasterResponse db = new DTOMasterResponse();

                //    db.Id = Forma.ApptId;
                //    db.Name = Forma.AppointmentName;
                //    lst.Add(db);
                //}
                var ret = await MasterOnlyTable.GetAppointment();
                lst = ret;
            }
            else if (Data.id == Convert.ToInt16(Constants.ApplicationType))
            {
                var ret = await MasterOnlyTable.GetApplicationType();
                lst = ret;
            }
            else if (Data.id == Convert.ToInt16(Constants.ApplicantType))
            {
                var ret = await MasterOnlyTable.GetApplicantType();
                lst = ret;

            }
            else if (Data.id == Convert.ToInt16(Constants.ArmyPrefix))
            {
                var Ret = await MasterOnlyTable.GetAllPreFix();
                lst = Ret;
            }
            else if (Data.id == Convert.ToInt16(Constants.ArmyPrefixOfficers))
            {
                var Ret = await MasterOnlyTable.GetAllPreFixByType(1);
                lst = Ret;
            }
            else if (Data.id == Convert.ToInt16(Constants.ArmyPrefixJCO))
            {
                var Ret = await MasterOnlyTable.GetAllPreFixByType(2);
                lst = Ret;
            }
            else if (Data.id == Convert.ToInt16(Constants.ArmyPrefixOR))
            {
                var Ret = await MasterOnlyTable.GetAllPreFixByType(3);
                lst = Ret;
            }
            else if (Data.id == Convert.ToInt16(Constants.RegtCorps))
            {
                var Ret = await MasterOnlyTable.GetAllRegtCorps();
                lst = Ret;
            }
            else if (Data.id == Convert.ToInt16(Constants.Unit))
            {
                var Ret = await MasterOnlyTable.GetAllUnit();
                lst = Ret;
            }
            else if(Data.id == Convert.ToInt16(Constants.ArmyPostOffice))
            {
                var Ret = await MasterOnlyTable.GetArmyPostOffice();
                lst = Ret;
            }
            else if(Data.id == Convert.ToInt16(Constants.LoanFreq))
            {
                var Ret = await MasterOnlyTable.GetLoanFreq();
                lst = Ret;
            }
            else if(Data.id == Convert.ToInt16(Constants.LoanTypeHBA))
            {
                var Ret = await MasterOnlyTable.GetLoanType(1);
                lst = Ret;
            }
            else if(Data.id == Convert.ToInt16(Constants.LoanTypeCA))
            {
                var Ret = await MasterOnlyTable.GetLoanType(2);
                lst = Ret;
            }
            else if(Data.id == Convert.ToInt16(Constants.LoanTypePCA))
            {
                var Ret = await MasterOnlyTable.GetLoanType(3);
                lst = Ret;
            }
            else if (Data.id == Convert.ToInt16(Constants.PurposeOfWithdrawal))
            {
                var Ret = await MasterOnlyTable.GetPurposeOfWithdrawal();
                lst = Ret;
            }
            else if (Data.id == Convert.ToInt16(Constants.VehType))
            {
                var Ret = await MasterOnlyTable.GetVehType();
                lst = Ret;
            }
            return lst;
        }
    }
}
