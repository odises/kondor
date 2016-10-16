using System.Collections.Generic;
using System.Linq;
using Kondor.Domain;
using Kondor.Domain.Models;

namespace Kondor.Data.EF
{
    public class EFExampleRepository : EFRepository<Example>, IExampleRepository
    {
        public EFExampleRepository(IDbContext context) : base(context)
        {
        }
        public IEnumerable<Example> GetExamplesByCardId(int id)
        {
            return DbSet.Where(p => p.CardId == id);
        }
    }
}
