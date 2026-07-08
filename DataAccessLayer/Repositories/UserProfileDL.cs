using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using DataTransferObject.Request;
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
            return await (
                from up in _context.UserProfiles
                join tum in _context.trnUserMappings
                    on up.ProfileId equals tum.ProfileId
                join au in _context.Users
                    on tum.UserId equals au.Id
                where au.UserName == userName
                select up
            ).FirstOrDefaultAsync();
        }



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
                        orderby user.UpdatedOn descending
                        select new DTOUserProfileResponse
                        {
                            DomainId = user.UserName,
                            ProfileName = rank.RankName + " " + profile.Name,
                            AppointmentName = appt.AppointmentName,
                            ArmyNo = profile.ArmyNo,
                            EmailId = user.Email,
                            MobileNo = user.PhoneNumber,
                            UnitName = unit.UnitName,
                            RankName = rank.RankName,
                            RegtName = regt.RegtName,
                            IsActive = status,
                            IsPrimary = mapping.IsPrimary,
                            IsFmn = mapping.IsFmn,
                            UpdatedOn = user.UpdatedOn,
                            ProfileId = profile.ProfileId,
                            status = _context.TrnFwdCO.Any(fwd => fwd.COUserId == user.Id),
                            UserId = user.Id
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

                orderby user.UpdatedOn descending

                select new DTOUserProfileResponse
                {
                    DomainId = user.UserName,
                    MappingId = mapping.MappingId,
                    IsCOActive = mapping.IsActive,
                    ProfileId = profile.ProfileId,
                    ProfileName = rank.RankName + " " + profile.Name,
                    AppointmentName = appt.AppointmentName,
                    ArmyNo = profile.ArmyNo,
                    EmailId = user.Email,
                    MobileNo = user.PhoneNumber,
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

        public async Task<bool> SaveApprovedLogs(int userId, string ip, bool isActive, int coProfileId)
        {
            var userMapping = await (
                from up in _context.UserProfiles
                join um in _context.trnUserMappings
                    on up.ProfileId equals um.ProfileId
                where um.UserId == userId
                select new
                {
                    up.ProfileId
                }
            ).FirstOrDefaultAsync();
            if (userMapping == null)
            {
                return false;
            }
            var coUserMapping = await (
                from um in _context.trnUserMappings
                   
                where um.ProfileId == coProfileId
                select new
                {
                    um.UserId
                }
            ).FirstOrDefaultAsync();

            if (coUserMapping == null)
            {
                return false;
            }

            var approvedLog = new TrnApprovedLog
            {
                AdminProfileId = userMapping.ProfileId,
                IpAddress = ip,
                IsApproved = isActive,
                UpdatedOn = DateTime.Now,
                CoProfileId = coProfileId,
                AdminUserId = userId,
                CoUserId = coUserMapping.UserId
            };

            _context.TrnApprovedLogs.Add(approvedLog);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> DeleteUserAsync(string domainId, int profileId)
        {
            try
            {
                var userProfile = await _context.UserProfiles
                    .FirstOrDefaultAsync(p => p.ProfileId == profileId);

                if (userProfile == null)
                {
                    return false;
                }

                var userMapping = await _context.trnUserMappings
                    .FirstOrDefaultAsync(m => m.ProfileId == profileId);

                if (userMapping == null)
                {
                    return false;
                }

                var userId = userMapping.UserId;

                var identityUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (identityUser == null)
                {
                    return false;
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var userRoles = _context.UserRoles.Where(ur => ur.UserId == userId);
                        _context.UserRoles.RemoveRange(userRoles);

                        _context.trnUserMappings.Remove(userMapping);

                        _context.UserProfiles.Remove(userProfile);

                        _context.Users.Remove(identityUser);

                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return true;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SaveLoginLogs(DTOLoginLogs loginLog)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var ipEntry = await _context.MIpAddresses.FirstOrDefaultAsync(ip => ip.IPAddress == loginLog.IpAddress);
                if (ipEntry == null)
                {
                    ipEntry = new MIpAddress
                    {
                        IPAddress = loginLog.IpAddress
                    };
                    _context.MIpAddresses.Add(ipEntry);
                    await _context.SaveChangesAsync();
                }
                var logEntry = new trnLoginLog
                {
                    ProfileId = loginLog.ProfileId,
                    IpAddressId = ipEntry.IpAddressId,
                    LoginOn = loginLog.LoginOn
                };
                _context.TrnLoginLogs.Add(logEntry);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch 
            {
                await transaction.RollbackAsync();
                return false;
            }
            
        }
    }
}
