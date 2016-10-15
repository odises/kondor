using Kondor.Data.LeitnerDataModels;
using Kondor.Domain.Enums;

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