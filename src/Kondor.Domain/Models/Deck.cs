using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kondor.Domain.Models
{
    public class Deck : Entity
    {
        public string Title { get; set; }
        public DateTime CreationDateTime { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public ICollection<Card> Cards { get; set; }
        public ICollection<SubDeck> SubDecks { get; set; }
    }
}
