using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer.Repositories
{
    public class OnlineApplicationDL : GenericRepositoryDL<CommonDataModel>, IOnlineApplication
    {
        protected new readonly ApplicationDbContext _context;

        public OnlineApplicationDL(ApplicationDbContext context) : base(context)
        {
            _context = context;
           
        }

        //public Task<bool> IsUser(string AadharNo)
        //{
        //   _context.OnlineApplications.ToList();
        //    var user = _context.OnlineApplications.FirstOrDefault(x => x.AadharNo == AadharNo);
        //    if (user != null)
        //    {
        //        return Task.FromResult(true);
        //    }
        //    else
        //    {
        //        return Task.FromResult(false);
        //    }
        //}
        public Task<DateTime> GetRetirementDate(int rankId, int Prefix, DateTime dateTime)
        {
            var userType = _context.MArmyPrefixes.FirstOrDefault(x => x.Id == Prefix);
            var retAge = _context.MRanks.FirstOrDefault(x => x.RankId == rankId);
            var retirementAge = retAge.RetirementAge;
            var userTypeId = userType.UserType;

            var ret = (from prefix in _context.MArmyPrefixes
                       join rank in _context.MRanks on rankId equals Prefix
                       where prefix.Id == Prefix
                       select new
                       {
                           userType = prefix.UserType,
                           retAge = rank.RetirementAge

                       });


            if (retirementAge > 0 && userTypeId != 0)
            {
                DateTime retirementDate = DateTime.Now.AddYears((int)retirementAge);
                return Task.FromResult(retirementDate);
            }
            else
            {
                return Task.FromResult(DateTime.MinValue);
            }
        }

        public Task<CommonOnlineApplicationResponse?> GetApplicationDetails(int applicationId,string formtype)
        {
            int Applicationtype = 0;

            CommonOnlineApplicationResponse data= new CommonOnlineApplicationResponse();

                var result = (from common in _context.Applications
                              join prefix in _context.MArmyPrefixes on common.ArmyPrefix equals prefix.Id into prefixGroup
                              from prefix in prefixGroup.DefaultIfEmpty()
                              join oldPrefix in _context.MArmyPrefixes on common.OldArmyPrefix equals oldPrefix.Id into oldPrefixGroup
                              from oldPrefix in oldPrefixGroup.DefaultIfEmpty()
                              join rank in _context.MRanks on common.DdlRank equals rank.RankId into rankGroup
                              from rank in rankGroup.DefaultIfEmpty()
                              join armyPostOffice in _context.MArmyPostOffices on common.ArmyPostOffice equals armyPostOffice.Id into armyPostOfficeGroup
                              from armyPostOffice in armyPostOfficeGroup.DefaultIfEmpty()
                              join regCorps in _context.MRegtCorps on common.RegtCorps equals regCorps.Id into regCorpsGroup
                              from regCorps in regCorpsGroup.DefaultIfEmpty()
                              join parentUnit in _context.MUnits on common.ParentUnit equals parentUnit.UnitId into parentUnitGroup
                              from parentUnit in parentUnitGroup.DefaultIfEmpty()
                              join presentUnit in _context.MUnits on common.PresentUnit equals presentUnit.UnitId into presentUnitGroup
                              from presentUnit in presentUnitGroup.DefaultIfEmpty()
                              select new OnlineApplicationResponse
                              {
                                  //PrefixName = prefix != null ? prefix.Prefix : string.Empty,
                               
                                  ParentUnit = parentUnit != null ? parentUnit.UnitName : string.Empty,
                                  PresentUnit = presentUnit != null ? presentUnit.UnitName : string.Empty,
                                  ApplicationId= common.ApplicationId,
                                  ArmyPrefix = common.ArmyPrefix,
                                  Number= prefix != null ? prefix.Prefix : string.Empty

                              }).FirstOrDefault();


            if (result != null)
            {
                var Hbamodel = _context.HBA.Where(x => x.ApplicationId == applicationId);
                var Carmodel = _context.Car.FirstOrDefault(x => x.ApplicationId == applicationId);
                var Pcamodel = _context.PCA.FirstOrDefault(x => x.ApplicationId == applicationId);
                

                 data.OnlineApplicationResponse = result;
               // data.CarApplicationResponse = Carmodel;

            }
           






            return null;
            
        }
    }

}
