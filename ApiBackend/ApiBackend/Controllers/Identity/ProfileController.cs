using ApiBackend.ApiErrorHandlers;
using AutoMapper;
using Core.Entities.Identity;
using Core.EntitiesDTOs.Identity;
using Core.Helpers.Images;
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
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly AppUserManager _userManager;
        private readonly UserImageService _userImageService;
        private readonly UserAddressService _userAddressService;
        private readonly SmsService _smsService;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public ProfileController(
            AppUserManager userManager,
            UserImageService userImageService,
            UserAddressService userAddressService,
            SmsService smsService,
            IMapper mapper,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _userImageService = userImageService;
            _userAddressService = userAddressService;
            _smsService = smsService;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        #region User Data Endpoints

        // Get: api/Profile/user
        [HttpGet("user")]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            var email = _tokenService.GetCurrentUserEmail();
            var user = await _userManager.FindByEmailAsync(email, $"{nameof(UserImage)}, {nameof(UserAddress)}");
            if (user == null)
                return NotFound(new ApiErrorResponse(404,"NotFound"));

            UserDto _user = _mapper.Map<AppUser, UserDto>(user);
            return Ok(_user);
        }
        #endregion User Data Endpoints

        #region User Address Endpoints

        // Get: api/profile/address
        [HttpGet("address")]
        public async Task<ActionResult<UserAddressDto>> GetUserAddress()
        {
            var email = _tokenService.GetCurrentUserEmail();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) 
                return Unauthorized(new ApiErrorResponse(401,"Unauthorized!"));

            UserAddress address = await _userAddressService.FindAddressAsync(user.Id);
            UserAddressDto _address = _mapper.Map<UserAddress, UserAddressDto>(address);
            return _address;
        }

        // Put: api/profile/address
        [HttpPut("address")]
        public async Task<ActionResult<UserAddressDto>> CreateUpdateUserAddress(UserAddressDto addressDto)
        {
            var email = _tokenService.GetCurrentUserEmail();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) 
                return Unauthorized(new ApiErrorResponse(401,"unauthorized!"));

            UserAddress address = await _userAddressService.FindAddressAsync(user.Id);

            if (address != null)
            {
                // update address object with addressDto data
                _mapper.Map(addressDto, address);

                UserAddress updated = await _userAddressService.UpdateUserAddress(address);
                if (updated != null)
                    return addressDto;

                return BadRequest(new ApiErrorResponse(400, "FailedUpdate"));
            }
            else
            {
                UserAddress added = await _userAddressService.CreateUserAddress(addressDto, user.Id);
                if (added != null)
                    return addressDto;

                return BadRequest(new ApiErrorResponse(400, "FailedCreate"));
            }
        }
        #endregion User Address Endpoints

        #region Phone Endpoints

        // Get: api/profile/phone
        [HttpGet("phone")]
        public async Task<ActionResult<string>> GetPhoneNumber()
        {
            var email = _tokenService.GetCurrentUserEmail();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) 
                return Unauthorized(new ApiErrorResponse(401,"Unauthorized!"));

            if (user.PhoneNumber == null)
                return NotFound(new ApiErrorResponse(404));

            return Ok(user.PhoneNumber);
        }

        // Put: api/profile/phone
        [HttpPut("phone/{number}")]
        public async Task<ActionResult<string>> CreatePhoneNumber(string number)
        {
            var email = _tokenService.GetCurrentUserEmail();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) 
                return Unauthorized(new ApiErrorResponse(401,"Unauthorized!"));

            if (user.PhoneNumber == number && await _userManager.IsPhoneNumberConfirmedAsync(user))
                return Ok("PhoneSuccessConfirmed");// to avoid re Confirm the phone number already confirmed before

            // check if this phone number is confirmed befor for other user
            var confirmedPhone = await _userManager.FindByPhoneNumberAsync(number);
            if (confirmedPhone != null )
            {
                return BadRequest(new ApiErrorResponse(400, "PhoneNumberInvalid"));
            }
            // check Valid Number
            if(number.Substring(0, 2) != "00")
            {
                return BadRequest(new ApiErrorResponse(400, "PhoneNumberInvalid"));
            }

            user.PhoneNumber = number;
            // if user add new phone number after confirm old number, set PhoneNumberConfirmed to false.
            user.PhoneNumberConfirmed = false;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                if (!await SendConfirmSmsAsync(user, number))
                {
                    user.PhoneNumber = null;
                    user.PhoneNumberConfirmed = false;
                    var deletePhoneResult = await _userManager.UpdateAsync(user);
                    if (deletePhoneResult.Succeeded)
                        return BadRequest(new ApiErrorResponse(400, "PhoneFaildSendConfirmSMS"));

                    throw new Exception("SMS to confirmed Phone Number not sent");
                }
                return Ok("PhoneSmsSendSuccessfully");
            }

            return BadRequest(new ApiErrorResponse(400, "PhoneFaildSendConfirmSMS"));
        }

        [AllowAnonymous]
        [HttpPost("phone/Confirm")]
        public async Task<ActionResult<string>> ConfirmPhoneNumber(string userEmail, string phoneNumber, string token)
        {
            if (userEmail == null || phoneNumber == null || token == null)
                return BadRequest(new ApiErrorResponse(400));

            // check if this phone number used from other user
            var userPhone = await _userManager.FindByPhoneNumberAsync(phoneNumber);
            if (userPhone != null)
                return BadRequest(new ApiErrorResponse(400, "PhoneNumberInvalid"));

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user != null)
            {
                var confirmed = await _userManager.IsPhoneNumberConfirmedAsync(user);
                if (user.PhoneNumber == phoneNumber && confirmed)
                    return Ok("PhoneSuccessConfirmed");// to avoid re Confirm the phone number already confirmed before


                var result = await _userManager.ChangePhoneNumberAsync(user, phoneNumber, token);
                if (result.Succeeded)
                    return Ok("phoneConfirmedSuccessfully");
            }
            return BadRequest(new ApiErrorResponse(400, "PhoneFailedConfirmed"));
        }

        // Delete: api/profile/phone
        [HttpDelete("phone")]
        public async Task<ActionResult<string>> DeletePhoneNumber()
        {
            var email = _tokenService.GetCurrentUserEmail();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) 
                return Unauthorized(new ApiErrorResponse(401, "Unauthorized!"));

            var result = await _userManager.SetPhoneNumberAsync(user, null);
            if (result.Succeeded)
            {
                user.PhoneNumberConfirmed = false;
                var updateResult = await _userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                    return Ok("PhoneSuccessDelete"); // SuccessfullyDelete Phone number 
            }
            throw new Exception("PhoneFailedDelete");
        }
        #endregion Phone Contoller

        #region Image Endpoints

        [HttpGet("image")]
        public async Task<ActionResult<IReadOnlyList<UserImage>>> GetUserImagesAsync()
        {
            IReadOnlyList<UserImage> images = await _userImageService.FindUserImagesAsync();
            return Ok(images);
        }

        [HttpPost("image")]
        public async Task<ActionResult<AppImageUploadResult>> UploadImage([FromForm] UploadImage UploadImage)
        {
            ImageTransformation transform = _mapper.Map<UploadImage, ImageTransformation>(UploadImage);

            AppImageUploadResult appImageUploadResult = await _userImageService.UploadImageAsync(UploadImage.File, transform);
            if (appImageUploadResult == null)
                return BadRequest(new ApiErrorResponse(400, "failedUpload"));

            return Ok(appImageUploadResult);

        }

        [HttpDelete("image/{id}")]
        public async Task<ActionResult<AppImageUploadResult>> DeleteImage(string id)
        {

            var deleted = await _userImageService.DeleteImageAsync(id);
            if (deleted)
                return Ok("DeleteSuccessfully");

            return BadRequest(new ApiErrorResponse(400, "failedUpload"));
        }

        [HttpPut("image/{imageId}/{profile_cover}")]
        public async Task<ActionResult<string>> SetImageProfilCovereAsync(string imageId, string profile_cover)
        {
            var email = _tokenService.GetCurrentUserEmail();
            var user = await _userManager.FindByEmailAsync(email, nameof(UserImage));
            if (user == null)
                return NotFound(new ApiErrorResponse(404,"UserNotFound!"));

            var image = await _userImageService.CreateImageProfilCovereAsync(user, imageId, profile_cover);
            if (image != null)
                return $"Create {profile_cover} successfully";

            return BadRequest(new ApiErrorResponse(400));
        }

        [HttpGet("Image/Profile")]
        public async Task<ActionResult<UserImage>> GetUserProfileImageAsync()
        {
            var email = _tokenService.GetCurrentUserEmail();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401, "Unauthorized!"));

            return Ok(user.ProfileImageUrl);

        }

        [HttpGet("Image/Cover")]
        public async Task<ActionResult<UserImage>> GetUserCoverImageAsync()
        {
            var email = _tokenService.GetCurrentUserEmail();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Unauthorized(new ApiErrorResponse(401, "Unauthorized!"));

            return Ok(user.CoverImageUrl);
        }

        [HttpDelete("image/{imageId}/{profile_cover}")]
        public async Task<ActionResult<string>> DeleteImageProfilCovereAsync(string imageId, string profile_cover)
        {
            var email = _tokenService.GetCurrentUserEmail();
            var user = await _userManager.FindByEmailAsync(email, nameof(UserImage));
            if (user == null)
                return NotFound(new ApiErrorResponse(404, "UserNotFound!"));

            var image = await _userImageService.DeleteImageProfilCovereAsync(user, imageId, profile_cover);
            if (image != null)
                return $"Delete {profile_cover} successfully";

            return BadRequest(new ApiErrorResponse(400));
        }
        #endregion Image Endpoints

        private async Task<bool> SendConfirmSmsAsync(AppUser user, string phoneNumber)
        {
            // Generate the token and send it
            var _token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            string confirmationLink = this.Url.ActionLink("ConfirmPhoneNumber", "Profile", new { userEmail = user.Email, phoneNumber = phoneNumber, token = _token }, "https", null, Request.Scheme);
            
            if (await _smsService.SendConfirmSmsAsync(phoneNumber, _token, confirmationLink))
                return true;

            return false;
        }
    }
}
