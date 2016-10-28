using System;

namespace Kondor.Domain.Models
{
    public class SubDeck : Entity
    {
        public string Title { get; set; }
        public DateTime CreationDateTime { get; set; }
        public int DeckId { get; set; }
        public virtual Deck Deck { get; set; }
    }
}
