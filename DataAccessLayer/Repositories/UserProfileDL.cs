using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer.Repositories
{
    public class UserProfileDL:GenericRepositoryDL<UserProfile>, IUserProfile
    {
        protected new readonly ApplicationDbContext _context;

        public UserProfileDL(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<UserProfile?> GetByUserName(string userName)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(x => x.userName == userName);
        }

        //public async Task<List<DTOUserProfileResponse>> GetAllUser(bool status)
        //{
        //    var users = await (from user in _context.Users
        //                       join mapping in _context.trnUserMappings on user.Id equals mapping.UserId
        //                       join unit in _context.MUnits on mapping.UnitId equals unit.UnitId
        //                       join profile in _context.UserProfiles on mapping.ProfileId equals profile.ProfileId
        //                       join appt in _context.MAppointments on profile.ApptId equals appt.ApptId
        //                       join rank in _context.MRanks on profile.rank equals rank.RankId
        //                       join regt in _context.MRegtCorps on profile.regtCorps equals regt.Id
        //                       join role in _context.UserRoles on user.Id equals role.UserId
        //                       where mapping.IsActive == status && role.RoleId == 2
        //                       orderby user.UpdatedOn
        //                       select new DTOUserProfileResponse
        //                       {
        //                           DomainId = profile.userName,
        //                           ProfileName = rank.RankName + " " + profile.Name,
        //                           AppointmentName = appt.AppointmentName,
        //                           ArmyNo = profile.ArmyNo,
        //                           EmailId = profile.Email,
        //                           MobileNo = profile.MobileNo,
        //                           UnitName = unit.UnitName,
        //                           RankName = rank.RankName,
        //                           RegtName = regt.RegtName,
        //                           IsActive = status,
        //                           IsPrimary = mapping.IsPrimary,
        //                           IsFmn = mapping.IsFmn,
        //                       }).ToListAsync();

        //    return users;
        //}

        public IQueryable<DTOUserProfileResponse> GetAllUser(bool status)
        {
            var users = from user in _context.Users
                        join mapping in _context.trnUserMappings on user.Id equals mapping.UserId
                        join unit in _context.MUnits on mapping.UnitId equals unit.UnitId
                        join profile in _context.UserProfiles on mapping.ProfileId equals profile.ProfileId
                        join appt in _context.MAppointments on profile.ApptId equals appt.ApptId
                        join rank in _context.MRanks on profile.rank equals rank.RankId
                        join regt in _context.MRegtCorps on profile.regtCorps equals regt.Id
                        join role in _context.UserRoles on user.Id equals role.UserId
                        where mapping.IsActive == status && role.RoleId == 2
                        orderby user.UpdatedOn
                        select new DTOUserProfileResponse
                        {
                            DomainId = profile.userName,
                            ProfileName = rank.RankName + " " + profile.Name,
                            AppointmentName = appt.AppointmentName,
                            ArmyNo = profile.ArmyNo,
                            EmailId = profile.Email,
                            MobileNo = profile.MobileNo,
                            UnitName = unit.UnitName,
                            RankName = rank.RankName,
                            RegtName = regt.RegtName,
                            IsActive = status,
                            IsPrimary = mapping.IsPrimary,
                            IsFmn = mapping.IsFmn,
                        };

            return users;
        }

        public async Task<DTOUserProfileResponse?> GetUserAllDetails(string userName)
        {
            var userDetails = await (
                from user in _context.Users
                where user.UserName == userName // move filter up for efficiency

                join mapping in _context.trnUserMappings on user.Id equals mapping.UserId
                join unit in _context.MUnits on mapping.UnitId equals unit.UnitId
                join profile in _context.UserProfiles on mapping.ProfileId equals profile.ProfileId
                join appt in _context.MAppointments on profile.ApptId equals appt.ApptId
                join rank in _context.MRanks on profile.rank equals rank.RankId
                join regt in _context.MRegtCorps on profile.regtCorps equals regt.Id
                join role in _context.UserRoles on user.Id equals role.UserId

                // Consider using distinct or selecting only the most recent if needed
                orderby user.UpdatedOn descending

                select new DTOUserProfileResponse
                {
                    DomainId = profile.userName,
                    MappingId = mapping.MappingId,
                    IsCOActive = mapping.IsActive,
                    ProfileId = profile.ProfileId,
                    ProfileName = rank.RankName + " " + profile.Name,
                    AppointmentName = appt.AppointmentName,
                    ArmyNo = profile.ArmyNo,
                    EmailId = profile.Email,
                    MobileNo = profile.MobileNo,
                    UnitName = unit.UnitName,
                    RankName = rank.RankName,
                    RegtName = regt.RegtName,
                    IsPrimary = mapping.IsPrimary,
                    IsFmn = mapping.IsFmn,
                    RankId = rank.RankId,
                    RegtId = regt.Id,
                    ApptId = profile.ApptId,
                    UnitId = unit.UnitId,
                    username = profile.Name
                }
            ).FirstOrDefaultAsync();

            return userDetails;
        }


        public async Task<DTOUserProfileResponse?> GetAdminDetails()
        {
            var admin = await (
                from userRole in _context.UserRoles
                where userRole.RoleId == 1
                join profile in _context.UserProfiles on userRole.UserId equals profile.ProfileId
                join mapping in _context.trnUserMappings on profile.ProfileId equals mapping.ProfileId
                select new DTOUserProfileResponse
                {
                    UserId = mapping.UserId,
                    ProfileId = profile.ProfileId
                }
            ).FirstOrDefaultAsync();

            return admin;
        }

        public async Task<bool> SaveTrnFwdRecords(TrnFwd trnFwd)
        {
            try
            {
                await _context.TrnFwd.AddAsync(trnFwd);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SaveApprovedLogs(string DomainId, string Ip, bool isActive, string coDomainId, int coProfileId)
        {
            var user = await _context.UserProfiles
                .Where(u => u.userName == DomainId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return false;
            }

            var approvedLog = new TrnApprovedLog   // Entity mapped to table `trnApprovedLogs`
            {
                Name = user.Name,
                DomainId = user.userName,
                IpAddress = Ip,
                IsApproved = isActive,
                UpdatedOn = DateTime.Now,
                coDomainId = coDomainId,
                coProfileId = coProfileId
            };

            _context.TrnApprovedLogs.Add(approvedLog);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
