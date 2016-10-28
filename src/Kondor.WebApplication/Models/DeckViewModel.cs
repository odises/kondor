using System.ComponentModel.DataAnnotations;

namespace Kondor.WebApplication.Models
{
    public class DeckViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
    }
}