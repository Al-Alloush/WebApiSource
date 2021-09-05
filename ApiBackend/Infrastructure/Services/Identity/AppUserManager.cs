

using AutoMapper;
using Core.Entities.Identity;
using Core.EntitiesDTOs.Identity;
using Core.Helpers;
using Core.Helpers.Identity;
using Core.Interfaces.AppService;
using Core.Interfaces.AppService.RepositoriesAndPatterns;
using Core.SpecificationsQueries.Identity;
using Infrastructure.DataApp;
using Infrastructure.Services.AppServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services.Identity
{
    public class AppUserManager : UserManager<AppUser>
    {
        private readonly UserStore<AppUser, AppIdentityRole, AppDbContext, string, IdentityUserClaim<string>,
        IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>> _store;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettingsService _appSettingsService;
        private readonly AppRoleManager _roleManager;
        private readonly IMapper _mapper;

        public AppUserManager(
            IUserStore<AppUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<AppUser> passwordHasher,
            IEnumerable<IUserValidator<AppUser>> userValidators,
            IEnumerable<IPasswordValidator<AppUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<AppUser>> logger,
            IUnitOfWork unitOfWork,
            AppSettingsService appSettingsService,
            AppRoleManager roleManager,
            IMapper mapper)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _store = (UserStore<AppUser, AppIdentityRole, AppDbContext, string, IdentityUserClaim<string>,
                IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>)store;
            _unitOfWork = unitOfWork;
            _appSettingsService = appSettingsService;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<AppUser> RegisterNewUserAsync(UserRegisterDto register)
        {

            // check if  Email existing before, if exist stop Register
            var userCheck = await FindByEmailAsync(register.Email);
            if (userCheck != null)
                return null;

            // check if  UserName existing before, if exist stop Register
            userCheck = await FindByNameAsync(register.UserName);
            if (userCheck != null)
                return null;


            // init the new user
            AppUser newUser = _mapper.Map<UserRegisterDto, AppUser>(register);

            // check if this Role existing
            AppIdentityRole roleExist = await _roleManager.FindByNameAsync(register.Role.Trim());
            if (roleExist != null)
            {
                // check if this role not the default user role in appsetting.json file
                if (roleExist.Name != _appSettingsService.DefaultUserRole())
                {
                    newUser.ResetPassword = true; // add reset password for this user, because this Role is Admin,Editor, ... Roles
                    newUser.EmailConfirmed = true; // this email is confirmed
                }
            }
            else
                return null; // not allowed to create a user with out valid Role
           
            try
            {
                var result = await CreateAsync(newUser, register.Password);
                if (result.Succeeded)
                {
                    // add user Role
                    IdentityResult addRole = await AddToRoleAsync(newUser, register.Role.Trim());

                    return newUser;
                }

                throw new Exception($"CreateUserNotSuccess");
            }
            catch (Exception ex)
            {
                throw new Exception($"CreateUserNotSuccess, Message: {ex.Message}");
            }
        }


        /// <summary>
        /// find AppUser include related Tables
        /// </summary>
        /// <param name="email"></param>
        /// <param name="include"></param>
        /// <returns>AppUser if exit, else null</returns>
        /// 
        public async Task<AppUser> FindByEmailAsync(string email, string include)
        {
            AppUser user = await _unitOfWork.Repository<AppUser>().EntityAsync(new SpeciUser_FindUserByEmail(email, include));
            return user;
        }


        /// <summary>
        /// find AppUser include related Tables
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="include"></param>
        /// <returns>AppUser if exit, else null</returns>
        /// 
        public async Task<AppUser> FindByNameAsync(string userName, string include)
        {
            AppUser user = await _unitOfWork.Repository<AppUser>().EntityAsync(new SpeciUser_FindUserByUserName(userName, include));
            return user;
        }

        /// <summary>
        /// find AppUser include related Tables
        /// </summary>
        /// <param name="id"></param>
        /// <param name="include"></param>
        /// <returns>AppUser if exit, else null</returns>
        /// 
        public async Task<AppUser> FindByIdAsync(string id, string include)
        {
            AppUser user = await _unitOfWork.Repository<AppUser>().EntityAsync(new SpeciUser_FindUserById(id, include));
            return user;
        }

        /// <summary>
        /// find AppUser include related Tables
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="include"></param>
        /// <returns>AppUser if exit, else null</returns>
        /// 
        public async Task<AppUser> FindByPhoneNumberAsync(string phoneNumber, string include = null)
        {
            IReadOnlyList<AppUser> users = await _unitOfWork.Repository<AppUser>().ListAsync(new SpeciUser_FindUserByPhoneNumber(phoneNumber, include));
            if (users.Count > 1)
                throw new Exception("SomthingWrongPhoneNumbers");
            else if (users.Count <= 0)
                return null;

            return users[0];
        }

        /// <summary>
        /// find user by Email, Id, Confirmed PhoneNumber or UserName
        /// </summary>
        /// <param name="value">its can be Email, UserName, Confirmed PhoneNumber and user's Id</param>
        /// <param name="include">Include ICollection AppUser Table</param >
        /// <returns>return AppUser if exist, else retur null </returns>
        /// 
        public async Task<AppUser> FindUserByAllAsync(string value, string include = null)
        {
            var user = await FindByEmailAsync(value, include);
            user = user ?? await FindByNameAsync(value, include);
            user = user ?? await FindByPhoneNumberAsync(value, include);
            user = user ?? await FindByIdAsync(value, include);
            return user;
        }

        /// <summary>
        /// Get Current User' Address
        /// </summary>
        /// <param name="userId">User's Id</param>
        /// <returns>return UserAddress if exist, else return null</returns>
        public async Task<UserAddress> FindAddressAsync(string value)
        {
            var user = await FindUserByAllAsync(value, "UserAdress");
            return user.UserAddress;
        }


        /// <summary>
        /// find the id of the SuperAdmin
        /// </summary>
        /// <returns>string, return SuperUser Id if Exist, else return null</returns>
        public async Task<string> FindSuperAdminIdAsync()
        {
            // app has just one SuperAdmin, we don't want retrun it with any query
            List<AppUser> superAdmin = new List<AppUser>(await GetUsersInRoleAsync("SuperAdmin"));
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
            var user = await FindUserByAllAsync(value);
            if (user == null)
                return null;

            string role = (await GetRolesAsync(user)).FirstOrDefault();
            return role;
        }

        public async Task<Pagination<UserCardDto>> GetUsersCardsListAsync(UserPaginationParam param)
        {
            try
            {
                // query all blogs with pagination tools
                IReadOnlyList<AppUser> users = await _unitOfWork.Repository<AppUser>().ListAsync(new Speci_UserPagination(param: param, emptyCtor: false));
                // to get the count, it's the same criteria if spec
                int usersCount = await _unitOfWork.Repository<AppUser>().CountAsync(new Speci_UserPagination(param: param, emptyCtor: true));

                // mapping from Blog -> BlogCardDto
                IReadOnlyList<UserCardDto> _users = _mapper.Map<IReadOnlyList<AppUser>, IReadOnlyList<UserCardDto>>(users);

                // return using pagination tools
                return new Pagination<UserCardDto>(param.PageIndex, param.PageSize, usersCount, _users);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> DeleteUsersNotConfirmedAsync()
        {
            var beforOneMonth = DateTime.Today.AddMonths(-1);
            // Delete all users who were registered one month ago and have not yet activated their accounts
            List<AppUser> userUnauthorizeList = _store.Users
                                                    .Where(u => u.EmailConfirmed == false && u.PhoneNumberConfirmed == false && u.CreatedDate < beforOneMonth)
                                                    .ToList();
            foreach (var user in userUnauthorizeList)
            {
                await DeleteAsync(user);
            }
            return $"Delete all {userUnauthorizeList.Count} users, that not Confirm email and Phone number";
        }
    }
}
