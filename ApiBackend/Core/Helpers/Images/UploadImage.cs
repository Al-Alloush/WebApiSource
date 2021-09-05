using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers.Images
{
    public class UploadImage : BaseImages
    {
        [Required(ErrorMessage = "propErrorMsgFieldReauired")]
        [Display(Name = "propPhotoFile")]
        public IFormFile File { get; set; }
    }
}
