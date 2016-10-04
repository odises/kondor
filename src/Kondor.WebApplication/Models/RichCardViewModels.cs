using System.ComponentModel.DataAnnotations;

namespace Kondor.WebApplication.Models
{
    public class RichCardViewModel
    {
        [Display(Name = "Front Side")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required field")]
        public string FrontSide { get; set; }

        [Display(Name = "Back Side")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required field")]
        public string BackSide { get; set; }
    }
}