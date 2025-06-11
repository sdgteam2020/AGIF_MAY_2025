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

        public async Task<UserProfile?> GetAllUser(bool status)
        {
            var users = await (from user in _context.Users
                        join mapping in _context.UserMappings on user.Id equals mapping.UserID
                        join unit in _context.MUnits on mapping.UnitId equals unit.UnitId
                        join profile in _context.UserProfiles on mapping.ProfileId equals profile.ProfileId
                        join appt in _context.MAppointments on profile.ApptId equals appt.ApptId
                        where mapping.IsActive == status
                               select new
                        {
                            Id = profile.ProfileId,
                            UserId = user.Id,
                            userName = user.UserName
                          
                        }).ToListAsync();
                        
            return null;
        }
    }
}
