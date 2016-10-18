using System.Collections.Generic;
using Kondor.Domain.Models;

namespace Kondor.Domain
{
    public interface ICardRepository : IRepository<Card>
    {
        IEnumerable<Card> GetCardsByUserId(string id);
        Card GetRandomlyCardToLearn(string userId);
    }
}