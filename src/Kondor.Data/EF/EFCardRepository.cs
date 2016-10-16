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
    }
}
