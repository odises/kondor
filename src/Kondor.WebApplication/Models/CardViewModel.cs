using Kondor.Data.Enums;
using Kondor.Data.LeitnerDataModels;

namespace Kondor.WebApplication.Models
{
    public class CardViewModel
    {
        public int Id { get; set; }
        public CardType CardType { get; set; }
        public ISide Front { get; set; }
        public ISide Back { get; set; }
    }
}