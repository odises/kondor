using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;

namespace Kondor.WebApplication.Models
{
    public class RawCardViewModel
    {
        [Display(Name = "Front Side")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required field")]
        public string FrontSide { get; set; }

        [Display(Name = "Back Side")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required field")]
        public string BackSide { get; set; }
    }
}