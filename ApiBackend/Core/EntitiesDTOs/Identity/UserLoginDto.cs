using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntitiesDTOs.Identity
{
    public class UserLoginDto
    {

        [Required(ErrorMessage = "FieldIsRequired")]
        [Display(Name = "UsernameEmailPhonnumber")]
        public string UsernameEmailPhonnumber { get; set; }


        [Required(ErrorMessage = "FieldIsRequired")]
        [StringLength(50, ErrorMessage = "StringLengthInvalid", MinimumLength = 6)]
        [DataType(DataType.Password, ErrorMessage = "PasswordInvalid")]
        [RegularExpression("(?=^.{6,100}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$", ErrorMessage = "PasswordInvalid")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
