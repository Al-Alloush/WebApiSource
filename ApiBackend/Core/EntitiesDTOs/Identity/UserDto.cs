using Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntitiesDTOs.Identity
{
    public class UserDto
    {
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "EmailConfirmed")]
        public string EmailConfirmed { get; set; }

        [Display(Name = "UserName")]
        public string UserName { get; set; }


        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Display(Name = "PhoneNumberConfirmed")]
        public string PhoneNumberConfirmed { get; set; }


        [Display(Name = "TwoFactorEnabled ")]
        public bool TwoFactorEnabled { get; set; }


        [Display(Name = "ProfileImageUrl ")]
        public string ProfileImageUrl { get; set; }


        [Display(Name = "CoverImageUrl ")]
        public string CoverImageUrl { get; set; }

        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Display(Name = "Birthday")]
        public DateTime Birthday { get; set; }

        [Display(Name = "CreatedDate")]
        public DateTimeOffset CreatedDate { get; set; }

        [Display(Name = "LanguageId")]
        public string LanguageId { get; set; }

        [Display(Name = "Addresses")]
        public UserAddressDto UserAddress { get; set; }

        [Display(Name = "UserImages")]
        public List<UserImage> UserImages { get; set; }
    }
}
