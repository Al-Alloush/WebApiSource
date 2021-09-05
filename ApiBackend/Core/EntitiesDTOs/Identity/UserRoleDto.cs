﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntitiesDTOs.Identity
{
    public class UserRoleDto
    {
        [Required(ErrorMessage = "FieldIsRequired")]
        [StringLength(500, ErrorMessage = "StringLengthInvalid", MinimumLength = 3)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "FieldIsRequired")]
        [Display(Name = "PermissionLevel")]
        public int PermissionLevel { get; set; }

        [StringLength(500, ErrorMessage = "StringLengthInvalid", MinimumLength = 0)]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
