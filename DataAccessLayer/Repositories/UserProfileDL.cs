using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<List<DTOUserProfileResponse?>> GetAllUser(bool status)
        {
            var users = await (from user in _context.Users
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
                               }).ToListAsync();

            return users!;
        }

    }
}
