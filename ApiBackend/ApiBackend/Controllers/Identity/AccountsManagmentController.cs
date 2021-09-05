using ApiBackend.ApiErrorHandlers;
using AutoMapper;
using Core.Entities.Identity;
using Core.EntitiesDTOs.Identity;
using Core.Helpers;
using Core.Helpers.App;
using Core.Helpers.Identity;
using Core.Interfaces.AppService;
using Core.SpecificationsQueries.Identity;
using Infrastructure.DataApp;
using Infrastructure.Services.AppServices;
using Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiBackend.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class AccountsManagmentController : ControllerBase
    {
        private readonly AppUserManager _userManager;
        private readonly RoleManager<AppIdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly AppSettingsService _appSettingsService;
        private readonly ITokenService _tokenService;

        public AccountsManagmentController( 
            AppUserManager userManager,
            RoleManager<AppIdentityRole> roleManager,
            AppDbContext context,
            IMapper mapper,
             AppSettingsService appSettingsService,
            ITokenService tokenService)

        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _mapper = mapper;
            _appSettingsService = appSettingsService;
            _tokenService = tokenService;
        }

        [HttpPost("UsersRegister")]
        public async Task<ActionResult<string>> Register(UserRegisterDto register)
        {

            // get the Accept-Langugae from Request Header
            register.LanguageId = HttpContext.Request.GetTypedHeaders().AcceptLanguage.FirstOrDefault()?.Value.Value;
            if (string.IsNullOrEmpty(register.LanguageId))
                register.LanguageId = _appSettingsService.GetDefaultAppLanguage();

            // check if user age is allowed to register
            if (_appSettingsService.CheckIfAgeEligibleToRegister(register.Birthday))
                return BadRequest(new ApiErrorResponse(400, "AgeNotEligibleToRegister"));

           

            // Admin can add just Editor & User, SuperAdmin can Add Admin, Editor & User
            AppUser currentUser = await _userManager.FindByEmailAsync(_tokenService.GetCurrentUserEmail());
            var currentUserRole = await _userManager.FindUserRoleNameAsync(currentUser.Email);
            if (currentUserRole == "Admin" && (register.Role.ToUpper().Trim() == "ADMIN" || register.Role.ToUpper().Trim() == "SUPERADMIN"))
                return BadRequest(new ApiErrorResponse(400, "NoPermationToAddAdmin"));

            AppUser newUser = await _userManager.RegisterNewUserAsync(register);
            if (newUser != null)
                return Ok(string.Format("Add{0}Successfully", register.Role.Trim()));

            return BadRequest(new ApiErrorResponse(500, "UserNameOrEmailExistingBefore,OrRoleNotExist"));
        }



        // Get: api/AccountsManagment/users
        [HttpGet("users")]
        public async Task<ActionResult<Pagination<UserCardDto>>> GetUsersList([FromForm] UserPaginationParam param)
        {
            var checkPermationCurrentUser = await CheckPermationCurrentUserAsync();
            if (checkPermationCurrentUser != null)
                return checkPermationCurrentUser;

             var data = await _userManager.GetUsersCardsListAsync(param);
            return data;
        }


        // Get: api/AccountsManagment/users/value( can be: Id, UserName, Email & PhoneNumber)}
        [HttpGet("users/{value}")]
        public async Task<ActionResult<UserDto>> GetUsers(string value)
        {
            var checkPermationCurrentUser = await CheckPermationCurrentUserAsync();
            if (checkPermationCurrentUser != null)
                return checkPermationCurrentUser;

            AppUser user = await _userManager.FindUserByAllAsync(value, $"{nameof(UserAddress)}, {nameof(UserImage)}");
            UserDto _user = _mapper.Map<AppUser, UserDto>(user);
            return Ok(_user);
        }

        // Get: api/AccountsManagment/role/value( can be: Id, UserName, Email & PhoneNumber)}
        [HttpGet("role/{value}")]
        public async Task<ActionResult<string>> GetUserRole(string value)
        {
            var checkPermationCurrentUser = await CheckPermationCurrentUserAsync();
            if (checkPermationCurrentUser != null)
                return checkPermationCurrentUser;

            var user = await _userManager.FindUserByAllAsync(value);
            var role = await _userManager.FindUserRoleNameAsync(user.Id);
            return Ok(role);
        }

        // Post: api/AccountsManagment/changeRole
        [HttpPost("changeRole")]
        public async Task<ActionResult<string>> ChangeUserRole(string idEmailUsername, string newRole)
        {
            var checkPermationCurrentUser = await CheckPermationCurrentUserAsync();
            if (checkPermationCurrentUser != null)
                return checkPermationCurrentUser;


            var currentUserEmail = _tokenService.GetCurrentUserEmail();
            var currentUser = await _userManager.FindByEmailAsync(currentUserEmail); ;
            if (currentUser == null) 
                return Unauthorized(new ApiErrorResponse(401,"Unauthorized"));

            // get user targeted using UserName or Email to change his Roles
            AppUser targetUser = await _userManager.FindUserByAllAsync(idEmailUsername);
            if (targetUser == null) 
                return NotFound(new ApiErrorResponse(404,"NotFound"));

            var targetUserRole = await _userManager.FindUserRoleNameAsync(targetUser.Email);

            // check if this role exist
            var existRole = await _roleManager.FindByNameAsync(newRole.Trim());
            if (existRole == null || existRole.NormalizedName == "SUPERADMIN")
                return BadRequest(new ApiErrorResponse(400,"RoleNotExist"));
            
            // check if new role the same new role
            if (targetUserRole.ToUpper().Trim() == newRole.ToUpper().Trim())
                return BadRequest(new ApiErrorResponse(400, "ThisUserHasSameRole"));

            // ckeck if current user is admin and targetUser admin too, in the same level of role
            var currentUserRole = await _userManager.FindUserRoleNameAsync(currentUserEmail);
            if(currentUserRole.Equals(targetUserRole))
                return BadRequest(new ApiErrorResponse(400, "NoPermission"));

            try
            {
                // add new Role
                var result = await _userManager.AddToRoleAsync(targetUser, newRole.Trim());
                if (result.Succeeded)
                {
                    // there a possibility to the user has more than one Role,
                    // but in this app we need just one role for every user
                    // remove old role
                    await _userManager.RemoveFromRoleAsync(targetUser, targetUserRole);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            // test if the user role now is updated
            string userTargetedNewRole = await _userManager.FindUserRoleNameAsync(targetUser.Email);
            if (userTargetedNewRole == newRole.Trim())
                return Ok("RoleSuccessUpdate");
            else
                return BadRequest(new ApiErrorResponse(400));
        }

        // Get: api/AccountsManagment/DeleteNotConfirmed
        // Delete all users are not confirmed email and PhoneNumber
        [HttpDelete("DeleteNotConfirmed")]
        public async Task<ActionResult<string>> DeleteUsersNotConfirmed()
        {
            var checkPermationCurrentUser = await CheckPermationCurrentUserAsync();
            if (checkPermationCurrentUser != null)
                return checkPermationCurrentUser;

            return await _userManager.DeleteUsersNotConfirmedAsync();
        }


        /// <summary>
        /// check if current user is exist and has right permation for this controller
        /// </summary>
        /// <returns>Unauthorized if User not exist or has not right Role, else null</returns>
        private async Task<ActionResult> CheckPermationCurrentUserAsync()
        {
            var currentUser = await _userManager.FindByEmailAsync(_tokenService.GetCurrentUserEmail());
            if (currentUser == null)
                return Unauthorized(new ApiErrorResponse(401));

            var currentUserRole = await _userManager.FindUserRoleNameAsync(currentUser.Email);
            if (currentUserRole != "Admin" && currentUserRole != "SuperAdmin")
                return Unauthorized(new ApiErrorResponse(401));

            return null;
        }
    }
}
