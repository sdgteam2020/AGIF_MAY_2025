using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace DataAccessLayer.Repositories
{
    public class UserMappingDL:GenericRepositoryDL<UserMapping>,IUserMapping
    {
        protected new readonly ApplicationDbContext _context;

        public UserMappingDL(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<List<UserMapping>> GetAllUser(bool status)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserMapping>> GetByApplicationId(int applicationId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserMapping>> GetByProfileId(int profileId)
        {
            var userMappings = await _context.trnUserMappings
                .Where(um => um.ProfileId == profileId)
                .ToListAsync();
            return userMappings;
        }

        public Task<List<UserMapping>> GetByProfileIdAndApplicationId(int profileId, int applicationId)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserMapping>> GetByProfileIdAndStatus(int profileId, int status)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserMapping>> GetByUserId(int userId)
        {
            var userMappings = await _context.trnUserMappings
                .Where(um => um.UserId == userId)
                .ToListAsync();
            return userMappings;
        }

        public Task<UserMapping> GetByUserName(string userName)
        {
            throw new NotImplementedException();
        }
    }
}
