using System;
using System.Collections.Generic;
using System.Linq;
using Kondor.Domain;
using Kondor.Domain.Models;

namespace Kondor.Data.EF
{
    public class EFCardRepository : EFRepository<Card>, ICardRepository
    {
        public EFCardRepository(IDbContext context) : base(context)
        {
        }

        public IEnumerable<Card> GetCardsByUserId(string id)
        {
            return DbSet.Where(p => p.UserId == id);
        }

        public Card GetRandomlyCardToLearn(string userId)
        {
            var availableCards = DbSet.Where(m => !DbContext.CardStates.Any(p => p.CardId == m.Id) && m.UserId == userId);
            return availableCards.OrderBy(p => Guid.NewGuid()).FirstOrDefault();
        }
    }
}
