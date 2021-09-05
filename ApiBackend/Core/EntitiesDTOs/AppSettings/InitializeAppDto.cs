using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntitiesDTOs.AppSettings
{
    public class InitializeAppDto
    {
        [Required(ErrorMessage = "FieldIsRequired")]
        [StringLength(150, ErrorMessage = "StringLengthInvalid", MinimumLength = 8)]
        [EmailAddress(ErrorMessage = "EmailInvalid")]
        //[RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-‌​]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", ErrorMessage = "EmailInvalid")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "FieldIsRequired")]
        [StringLength(100, ErrorMessage = "StringLengthInvalid", MinimumLength = 3)]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "FieldIsRequired")]
        [StringLength(50, ErrorMessage = "StringLengthInvalid", MinimumLength = 6)]
        [DataType(DataType.Password, ErrorMessage = "PasswordInvalid")]
        [RegularExpression("(?=^.{6,100}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$", ErrorMessage = "PasswordInvalid")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "FieldIsRequired")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "PasswordsNotMatch")]
        [Display(Name = "ConfirmPassword ")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "FieldIsRequired")]
        [StringLength(100, ErrorMessage = "StringLengthInvalid", MinimumLength = 3)]
        [Display(Name = "AppName")]
        public string AppName { get; set; }
    }
}
