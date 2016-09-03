using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kondor.WebApplication.Models
{
    public class CardViewModel
    {
        [Display(Name = "Front Side")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required field")]
        public string FrontSide { get; set; }

        [Display(Name = "Back Side")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required field")]
        public string BackSide { get; set; }

        [Display(Name = "Examples")]
        public string Examples { get; set; }

        [Display(Name = "Image Url")]
        public string ImageUrl { get; set; }
    }
}