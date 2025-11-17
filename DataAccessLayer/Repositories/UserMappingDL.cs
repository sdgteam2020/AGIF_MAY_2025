using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
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

        // Additional methods specific to UserMapping can be added here

        public async Task<UserMapping?> GetUnitDetails(int unitId)
        {
            var userMapping = await _context.trnUserMappings
                          .Where(um => um.UnitId == unitId && um.IsActive == true)
                          .FirstOrDefaultAsync();

            return userMapping;
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

        public async Task<List<UserMapping>> GetByUnitId(int unitId)
        {
            var userMappings = await _context.trnUserMappings
                .Where(um => um.UnitId == unitId)
                .ToListAsync();
            return userMappings;
        }
        
        public async Task<List<UserMapping>> GetActiveUnitId(int unitId)
        {
            var userMappings = await _context.trnUserMappings
                .Where(um => um.UnitId == unitId && um.IsActive == true)
                .ToListAsync();
            return userMappings;
        }

        public Task<bool> IsActiveUser(int userId)
        {
            bool res = _context.trnUserMappings
                .Where(um => um.UserId == userId && um.IsActive == true)
                .Any();
            return Task.FromResult(res);
        }
    }
}
