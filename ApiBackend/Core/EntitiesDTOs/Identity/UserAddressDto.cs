using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntitiesDTOs.Identity
{
    public class UserAddressDto
    {

        [Required(ErrorMessage = "FieldIsRequired")]
        [StringLength(100, ErrorMessage = "StringLengthInvalid", MinimumLength = 3)]
        [Display(Name = "Street")]
        public string Street { get; set; }

        [Required(ErrorMessage = "FieldIsRequired")]
        [StringLength(25, ErrorMessage = "StringLengthInvalid", MinimumLength = 1)]
        [Display(Name = "BuildingNum")]
        public string BuildingNum { get; set; }

        [StringLength(25, ErrorMessage = "StringLengthInvalid", MinimumLength = 1)]
        [Display(Name = "Flore")]
        public string Flore { get; set; }

        [Required(ErrorMessage = "FieldIsRequired")]
        [StringLength(100, ErrorMessage = "StringLengthInvalid", MinimumLength = 3)]
        [Display(Name = "City")]
        public string City { get; set; }

        [StringLength(100, ErrorMessage = "StringLengthInvalid", MinimumLength = 3)]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required(ErrorMessage = "FieldIsRequired")]
        [StringLength(100, ErrorMessage = "StringLengthInvalid", MinimumLength = 2)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required(ErrorMessage = "FieldIsRequired")]
        [StringLength(25, ErrorMessage = "StringLengthInvalid", MinimumLength = 3)]
        [Display(Name = "PostCode")]
        public string PostCode { get; set; }
    }
}
