using Core.Entities.AppSettings;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {

        [Display(Name = "ProfileImageUrl ")]
        public string ProfileImageUrl { get; set; }

        [Display(Name = "CoverImageUrl ")]
        public string CoverImageUrl { get; set; }

        [StringLength(100, ErrorMessage = "StringLengthInvalid", MinimumLength = 0)]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [StringLength(100, ErrorMessage = "StringLengthInvalid", MinimumLength = 0)]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "FieldIsRequired")]
        [Display(Name = "Birthday")]
        public DateTime Birthday { get; set; }

        [Required(ErrorMessage = "FieldIsRequired")]
        [Display(Name = "CreatedDate")]
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        [Display(Name = "RequiredResetPassword")]
        public bool ResetPassword { get; set; }

        [Required(ErrorMessage = "FieldIsRequired")]
        [StringLength(6, ErrorMessage = "StringLengthInvalid", MinimumLength = 2)]
        [Display(Name = "LanguageId")]
        public string LanguageId { get; set; }
        public AppLanguage Language { get; set; }


        [Display(Name = "Addresses")]
        public UserAddress UserAddress { get; set; }

        [Display(Name = "UserImages")]
        public ICollection<UserImage> UserImages { get; set; }
    }
}
