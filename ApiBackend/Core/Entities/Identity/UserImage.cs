using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Identity
{
    public class UserImage
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "FieldIsRequired")]
        [Display(Name = "Url")]
        public string Url { get; set; }

        [Required(ErrorMessage = "FieldIsRequired")]
        [Display(Name = "CreatedDate")]
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;


    }
}
