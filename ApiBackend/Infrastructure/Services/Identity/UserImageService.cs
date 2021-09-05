using Core.Entities.Identity;
using Core.Helpers.Images;
using Core.Interfaces.AppService;
using Core.Interfaces.AppService.RepositoriesAndPatterns;
using Infrastructure.DataApp;
using Infrastructure.Services.ThirdPartyServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Identity
{
    public class UserImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppUserManager _userManager;
        private readonly ImageCloudService _imageCloudService;
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _context;

        public UserImageService(
            IUnitOfWork unitOfWork,
            AppUserManager appUserManager,
            ImageCloudService imageCloudService,
            ITokenService tokenService,
            AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _userManager = appUserManager;
            _imageCloudService = imageCloudService;
            _tokenService = tokenService;
            _context = context;

        }

        #region Images
        public async Task<IReadOnlyList<UserImage>> FindUserImagesAsync()
        {
            var email = _tokenService.GetCurrentUserEmail();
            var user = await _userManager.FindByEmailAsync(email,"UserImage");
            IReadOnlyList<UserImage> userImages = user.UserImages.ToList();
            return userImages;
        }

        public async Task<AppImageUploadResult> UploadImageAsync(IFormFile file, ImageTransformation transform)
        {
            if (file.Length > 0)
            {
                AppImageUploadResult imageUploadResult = await _imageCloudService.UploadPhotoAsync(file, transform);

                if (imageUploadResult == null)
                    return null;

                var userImage = new UserImage
                {
                    Id = imageUploadResult.PublicId,
                    Url = imageUploadResult.Url
                };

                var email = _tokenService.GetCurrentUserEmail();
                var user = await _userManager.FindByEmailAsync(email, $"{nameof(UserImage)}");
                user.UserImages.Add(userImage);
                await _context.SaveChangesAsync();

                return imageUploadResult;
            }
            return null;
        }

        public async Task<bool> DeleteImageAsync(string id)
        {
            var email = _tokenService.GetCurrentUserEmail();
            var user = await _userManager.FindByEmailAsync(email, $"{nameof(UserImage)}");
            if (user == null)
                return false;

            var image = user.UserImages.Where(x => x.Id == id).FirstOrDefault();
            if (image == null)
                return false;

            user.UserImages.Remove(image);

            if (user.ProfileImageUrl == image.Url)
                user.ProfileImageUrl = null;

            if (user.CoverImageUrl == image.Url)
                user.CoverImageUrl = null;

            await _context.SaveChangesAsync();

            var deleted = await _imageCloudService.DeleteImageAsync(id);
            if (deleted)
                return true;

            return false;

        }

        public async Task<UserImage> CreateImageProfilCovereAsync(AppUser user, string imageId, string profile_cover)
        {
            var image = user.UserImages.Where(x => x.Id == imageId).FirstOrDefault();
            if (image == null)
                return null;

            if (profile_cover.ToLower().Trim() == "profile")
                user.ProfileImageUrl = image.Url;
            else if (profile_cover.ToLower().Trim() == "cover")
                user.CoverImageUrl = image.Url;
            else
                return null;

            if (await _unitOfWork.Repository<AppUser>().SaveChangesAsync())
                return image;

            return null;
        }

        public async Task<UserImage> DeleteImageProfilCovereAsync(AppUser user,string imageId, string profile_cover)
        {
            var image = user.UserImages.Where(x => x.Id == imageId).FirstOrDefault();
            if (image == null)
                return null;

            if (profile_cover.ToLower().Trim() == "profile")
                user.ProfileImageUrl = null;
            else if (profile_cover.ToLower().Trim() == "cover")
                user.CoverImageUrl = null;
            else
                return null;

            if (await _unitOfWork.Repository<AppUser>().SaveChangesAsync())
                return image;

            return null;
        }

        #endregion Images
    }
}
