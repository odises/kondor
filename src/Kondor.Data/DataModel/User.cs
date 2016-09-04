using System.Collections.Generic;

namespace Kondor.Data.DataModel
{
    public class User : Entity
    {
        public User()
        {
            Cards = new HashSet<Card>();
        }

        public string TelegramUsername { get; set; }
        public int TelegramUserId { get; set; }
        public ICollection<Card> Cards { get; set; } 
    }
}
