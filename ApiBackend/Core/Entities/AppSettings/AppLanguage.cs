using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.AppSettings
{
    public class AppLanguage
    {
        // language Code like: de, en-us, en, ar, ...
        [Required(ErrorMessage = "FieldIsRequired")]
        [StringLength(6, ErrorMessage = "StringLengthInvalid", MinimumLength = 2)]
        [Display(Name = "Id")]
        public string Id { get; set; }

        // name in the same language, like : Deutsch, Englisch, العربي
        [Required(ErrorMessage = "FieldIsRequired")]
        [StringLength(50, ErrorMessage = "StringLengthInvalid", MinimumLength = 2)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        // the Language Direction: ltr or rtl
        [Required(ErrorMessage = "FieldIsRequired")]
        [StringLength(3, ErrorMessage = "StringLengthInvalid", MinimumLength = 3)]
        [Display(Name = "LanguageDirection")]
        public string LanguageDirection { get; set; }

        // to add or delete a language from project
        [Required(ErrorMessage = "StringLengthInvalid")]
        [Display(Name = "Added")]
        public bool Added { get; set; }
    }
}
