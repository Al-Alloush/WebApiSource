using ApiBackend.ApiErrorHandlers;
using AutoMapper;
using Core.Entities.Identity;
using Core.EntitiesDTOs.Identity;
using Core.Interfaces.AppService;
using Core.SpecificationsQueries.Identity;
using Infrastructure.Services.AppServices;
using Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiBackend.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppSettingsService _appSettingsService;
        private readonly AppUserManager _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly EmailService _emailService;
        private readonly IMapper _mapper;

        public AuthController(
            AppSettingsService appSettingsService,
            AppUserManager userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            EmailService emailService,
            IMapper mapper)
        {
            _appSettingsService = appSettingsService;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _mapper = mapper;
        }


        [HttpGet("emailExist/{email}")]
        public async Task<ActionResult<bool>> ExistEmailAsync(string email)
        {
            var emailExist = await _userManager.FindByEmailAsync(email);
            if (emailExist != null)
                return true; // email exist
            else
                return false; // not email exist
        }


        [HttpGet("UsernameExist/{username}")]
        public async Task<ActionResult<bool>> ExistUserNameAsync(string username)
        {
            var UsernameExist = await _userManager.FindByNameAsync(username);
            if (UsernameExist != null)
                return true;
            else
                return false;
        }

        [HttpGet("PhoneNumberExist/{PhoneNumber}")]
        public async Task<ActionResult<bool>> ExistPhoneNumberAsync(string phoneNumber)
        {
            var PhoneNumberExist = await _userManager.FindByPhoneNumberAsync(phoneNumber);
            if (PhoneNumberExist != null)
                return true;
            else
                return false;
        }


        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(UserRegisterDto register)
        {

            // get the Accept-Langugae from Request Header
            register.LanguageId = HttpContext.Request.GetTypedHeaders().AcceptLanguage.FirstOrDefault()?.Value.Value;
            if (string.IsNullOrEmpty(register.LanguageId))
                register.LanguageId = _appSettingsService.GetDefaultAppLanguage();


            // set the dafault user Role
            register.Role = _appSettingsService.DefaultUserRole();

            // check if user age is allowed to register
            if (_appSettingsService.CheckIfAgeEligibleToRegister(register.Birthday))
                return BadRequest(new ApiErrorResponse(400,"AgeNotEligibleToRegister"));

            AppUser newUser = await _userManager.RegisterNewUserAsync(register);
            if (newUser != null) 
            {
                // Send verification Email, if not secusess delete current user 
                if (!await SendConfirmEmailAsync(newUser))
                    await _userManager.DeleteAsync(newUser);

                return Ok("AddUserSuccessfullyPleaseVisitYorEmailToConfirmThisAccount");
            }

            return BadRequest(new ApiErrorResponse(500,"UserNameOrEmailExistingBefore"));
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserLoginSuccessDto>> Login(UserLoginDto login)
        {
            // login using UserName, Confirmed PhoneNumber or Email, insensitive case.
            var user = await _userManager.FindUserByAllAsync(login.UsernameEmailPhonnumber);
            // check if user exist
            if (user == null)
                return NotFound(new ApiErrorResponse(404,"UserNotFound"));

            // chack the password
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await _signInManager.CheckPasswordSignInAsync( user, 
                                                                        login.Password, 
                                                                        _appSettingsService.LockoutAccountAlloued());
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return BadRequest(new ApiErrorResponse(400, "LouckAccount"));
                }
                return BadRequest(new ApiErrorResponse(400,"LoginFailedWrongUsernameOrPassword"));
            }

            // check if the email is not confirmed
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                // if not Confirmed sending a new Confirmation email and return a message
                if (await SendConfirmEmailAsync(user))
                    return Ok("ReSentConfirmEmail");
            }

            // check if this Accoutn need to reset Password, for example after created this Account by Admin 
            if (user.ResetPassword)
            {
                await SendResetPasswordAsync(user);
                return BadRequest(new ApiErrorResponse(400,"SentRestPasswordToYourEmail"));
            }

            var _user = _mapper.Map<AppUser, UserLoginSuccessDto>(user);
            _user.Token = await _tokenService.CreateTokenAsync(user);

            return _user;
        }

        // Get: api/Account/ForgotPassword
        [AllowAnonymous]
        [HttpGet("ForgotPassword/{email}")]
        public async Task<ActionResult<string>> ForgotPassword(string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);
            if (user == null) 
                return NotFound(new ApiErrorResponse(404,"EmailNotExist"));

            if (await SendResetPasswordAsync(user))
                return Ok("SentRestPasswordToYourEmail");

            throw new Exception("SentRestPasswordToYourEmailFailed");
        }

        [HttpGet("ConfirmEmail")]
        public async Task<ActionResult<string>> ConfirmEmail(string userId, string token)
        {

            // if this any data is null or empty
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest(new ApiErrorResponse(400,"SomeParameterEmptyOrInvalid"));

            // get user 
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new ApiErrorResponse(404, "NotFound"));

            // Confrim the accoutn
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return Ok("EmailConfirmedSuccessfully");
            else
                return BadRequest(new ApiErrorResponse(400, "EmailConfirmedFailed"));
        }

        // this endpoint is called from resetPassword.html page to update user's Password
        [AllowAnonymous]
        [HttpPost("SetResetPasswordConfirmation")]
        public async Task<ActionResult<string>> SetResetPasswordConfirmation([FromForm] ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByIdAsync(resetPasswordDto.UserId);
            if (user == null)
                return NotFound(new ApiErrorResponse(404, "NotFound"));

            // Decoding tha Token
            var token = WebEncoders.Base64UrlDecode(resetPasswordDto.Token);
            var _token = Encoding.UTF8.GetString(token);

            var result = await _userManager.ResetPasswordAsync(user, _token, resetPasswordDto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(new ApiErrorResponse(400, "UpdatePasswordFailed"));

            // if user object created by Admin, user must reset his password
            if (user.ResetPassword) // if true
            {
                user.ResetPassword = false;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    throw new Exception("UpdatePasswordFailed");
            }
            return Ok("UpdatePasswordSuccess");
        }


        private async Task<bool> SendConfirmEmailAsync(AppUser user)
        {
            // Start send Confirm Email
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // Send verification Email
            string confirmationLink = this.Url.ActionLink(nameof(ConfirmEmail), "Auth", new { userId = user.Id, token = token }, "https", null, Request.Scheme);
            if (await _emailService.SendConfirmEmailAsync(user, confirmationLink))
                return true;

            return false;
        }

        private async Task<bool> SendResetPasswordAsync(AppUser user)
        {
            // get the baseurl(Domain) of website
            var url = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            // because when we generate a new token this token will contain + , that will replace in URL to space, for that Encoding this token and Decoding it when update new password
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            byte[] encodedToken = Encoding.UTF8.GetBytes(token);
            string validToken = WebEncoders.Base64UrlEncode(encodedToken);
            string resetPasswordLink = $"{url}/ResetPasswordPage?userId={user.Id}&token={validToken}";

            if (await _emailService.SendResetPasswordAsync(user, resetPasswordLink))
                return true;

            return false;
        }
    }
}
