using System;
using System.Collections.Generic;
using Kondor.Data.Enums;
using Kondor.Data.LeitnerDataModels;
using Newtonsoft.Json;

namespace Kondor.Data.DataModel
{
    public class Card : Entity
    {
        public Card()
        {
            CardStates = new HashSet<CardState>();
        }
        public string UserId { get; set; }
        public CardType CardType { get; set; }
        public CardStatus CardStatus { get; set; }
        public string CardData { get; set; }
        public virtual ICollection<CardState> CardStates { get; set; }
        public virtual ApplicationUser User { get; set; }

        public ICard DeserializeCardData()
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
