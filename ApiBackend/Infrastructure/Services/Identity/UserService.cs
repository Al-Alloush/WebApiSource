using Core.Entities.Identity;
using Core.Interfaces.AppService;
using Core.Interfaces.AppService.RepositoriesAndPatterns;
using Core.SpecificationsQueries.Identity;
using Infrastructure.DataApp;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Identity
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AppUserManager _userManager;

        public UserService(
            IUnitOfWork _unitOfWork,
            AppDbContext context,
            ITokenService tokenService,
            AppUserManager userManager)
        {
            this._unitOfWork = _unitOfWork;
            _context = context;
            _tokenService = tokenService;
            _userManager = userManager;
        }


        /// <summary>
        /// query a user by email
        /// </summary>
        /// <param  name="email">User's Email</param >
        /// <param  name="include">Include ICollection AppUser Table</param >
        /// <returns>return AppUser if exist, else retur null </returns>
        /// 
        public async Task<AppUser> FindUserByEmailAsync(SpeciUser_FindUserByEmail speci)
        {
            AppUser _user = await _unitOfWork.Repository<AppUser>().EntityAsync(speci);
            return _user;
        }

        /// <summary>
        /// query a user by UsrtName
        /// </summary>
        /// <param name="userName">User's UserName</param>
        /// <param  name="include">Include ICollection AppUser Table</param >
        /// <returns>return AppUser if exist, else retur null </returns>
        /// 
        public async Task<AppUser> FindUserByUserNameAsync(SpeciUser_FindUserByUserName speci)
        {
            AppUser _user = await _unitOfWork.Repository<AppUser>().EntityAsync(speci);
            return _user;
        }

        /// <summary>
        /// query a user by Confirmed PhoneNumber
        /// </summary>
        /// <param name="phoneNumber">User's PhoneNumber</param>
        /// <param  name="include">Include ICollection AppUser Table</param >
        /// <returns>return AppUser if exist, else retur null </returns>
        ///
        public async Task<AppUser> FindUserByPhoneNumberAsync(SpeciUser_FindUserByPhoneNumber speci)
        {
            AppUser _user = await _unitOfWork.Repository<AppUser>().EntityAsync(speci);
            return _user;
        }

        /// <summary>
        /// query a user by Id
        /// </summary>
        /// <param name="id">User's Id</param>
        /// <param  name="include">Include ICollection AppUser Table</param >
        /// <returns>return AppUser if exist, else retur null </returns>
        ///
        public async Task<AppUser> FindUserByIdAsync(SpeciUser_FindUserById speci)
        {
            AppUser _user = await _unitOfWork.Repository<AppUser>().EntityAsync(speci);
            return _user;
        }

        /// <summary>
        /// find user by Email, Id, Confirmed PhoneNumber or UserName
        /// </summary>
        /// <param name="value">its can be Email, UserName, Confirmed PhoneNumber and user's Id</param>
        /// <param name="include">Include ICollection AppUser Table</param >
        /// <returns>return AppUser if exist, else retur null </returns>
        /// 
        public async Task<AppUser> FindUserAsync(SpeciUser_FindUser speci, string include = null)
        {
            AppUser user = await _unitOfWork.Repository<AppUser>().EntityAsync(speci);
            return user;
        }

        /// <summary>
        /// getting the current user who has been mack this request
        /// </summary>
        /// <returns>return AppUser if exist, else retur null </returns>
        public async Task<AppUser> FindCurrentUserAsync(string include = null)
        {
            // get an email form Token Claim, that has been added in TockenServices.cs
            string email = _tokenService.GetCurrentUserEmail();
            if (!string.IsNullOrEmpty(email))
            {
                var speci = new SpeciUser_FindUserByEmail(email, include);
                var user = await FindUserByEmailAsync(speci);
                return user;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get Current User' Address
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <returns>return UserAddress if exist, else return null</returns>
        public async Task<UserAddress> FindUserAddressAsync(string userId)
        {
            return await _context.UserAddresses.Where(x => x.AppUserId == userId).FirstOrDefaultAsync();
        }


        /// <summary>
        /// find the id of the SuperAdmin
        /// </summary>
        /// <returns>string, return SuperUser Id if Exist, else return null</returns>
        public async Task<string> FindSuperAdminIdAsync()
        {
            // app has just one SuperAdmin, we don't want retrun it with any query
            List<AppUser> superAdmin = new List<AppUser>(await _userManager.GetUsersInRoleAsync("SuperAdmin"));
            if (superAdmin == null || superAdmin.Count <= 0)
                return null;

            return superAdmin[0].Id;
        }

        /// <summary>
        /// Find User's Role
        /// </summary>
        /// <param name="value">User  Id, Email, UserName and PhoneNumber</param>
        /// <returns>string: return Role Name if user exist, else return null</returns>
        public async Task<string> FindUserRoleNameAsync(string value)
        {
            var speci = new SpeciUser_FindUser(value);
            var user = await FindUserAsync(speci);
            if (user == null)
                return null;

            string role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            return role;
        }

        /// <summary>
        /// find Role
        /// </summary>
        /// <param name="role">Role Name</param>
        /// <returns>AppIdentityRole: return Role if role exiet, else return null</returns>
        public async Task<AppIdentityRole> FindRoleAsync(string _role)
        {
            List<AppIdentityRole> roles = await _context.Roles.Where(r => r.NormalizedName != "SUPERADMIN").ToListAsync();

            foreach (var role in roles)
                if (role.NormalizedName == _role.ToUpper().Trim())
                    return role;

            return null;
        }

    }
}
