using System;
using System.Collections.Generic;
using Kondor.Domain.Enums;
using Kondor.Domain.LeitnerDataModels;
using Newtonsoft.Json;

namespace Kondor.Domain.Models
{
    public class Card : Entity
    {
        public Card()
        {
            CardStates = new HashSet<CardState>();
            Examples = new HashSet<Example>();
        }
        public string UserId { get; set; }
        public CardType CardType { get; set; }
        public CardStatus CardStatus { get; set; }
        public string CardData { get; set; }
        public virtual ICollection<CardState> CardStates { get; set; }
        public virtual ICollection<Example> Examples { get; set; }
        public virtual ApplicationUser User { get; set; }

        public ISimpleCard DeserializeCardData()
        {
            if (CardType == CardType.SimpleCard)
            {
                return JsonConvert.DeserializeObject<SimpleCard>(CardData);
            }
            else if (CardType == CardType.RichCard)
            {
                return JsonConvert.DeserializeObject<RichCard>(CardData);
            }

            throw new InvalidCastException();
        }
    }
}
