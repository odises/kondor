using System.Collections.Generic;

namespace Kondor.Service.DataModel
{
    public class User
    {
        public int Id { get; set; }
        public string TelegramUsername { get; set; }
        public int TelegramUserId { get; set; }
        public ICollection<Card> Cards { get; set; } 
    }
}
