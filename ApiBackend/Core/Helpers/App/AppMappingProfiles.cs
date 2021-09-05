using AutoMapper;
using Core.Entities.Identity;
using Core.EntitiesDTOs.Identity;
using Core.Helpers.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers.App
{
    public class AppMappingProfiles : Profile
    {
        public AppMappingProfiles()
        {
            //
            CreateMap<AppUser, UserCardDto>();
            CreateMap<AppUser, UserDto>();
            CreateMap<AppUser, UserLoginSuccessDto>();
            CreateMap<UserRegisterDto, AppUser>();

            //
            CreateMap<UserAddress, UserAddressDto>();
            CreateMap<UserAddressDto, UserAddress>();

            //
            CreateMap<UploadImage, ImageTransformation>();

            //
            CreateMap<UserRoleDto, AppIdentityRole>();
        }
    }
}
