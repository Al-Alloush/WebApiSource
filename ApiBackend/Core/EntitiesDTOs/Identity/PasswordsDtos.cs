using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntitiesDTOs.Identity
{
    public class BasePasswordEntityDto
    {
        [Required(ErrorMessage = "FieldIsRequired")]
        [RegularExpression("(?=^.{6,100}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$", ErrorMessage = "PasswordInvalid")]
        [StringLength(maximumLength: 16, ErrorMessage = "StringLengthInvalid", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "FieldIsRequired")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "PasswordsNotMatch")]
        [Display(Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordDto : BasePasswordEntityDto
    {
        [Required(ErrorMessage = "FieldIsRequired")]
        [Display(Name = "UserId")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "FieldIsRequired")]
        [Display(Name = "Token")]
        public string Token { get; set; }
    }
}
