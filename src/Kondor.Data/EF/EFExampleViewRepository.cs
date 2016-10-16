using System.Collections.Generic;
using System.Linq;
using Kondor.Domain;
using Kondor.Domain.Models;

namespace Kondor.Data.EF
{
    public class EFExampleViewRepository : EFRepository<ExampleView>, IExampleViewRepository
    {
        public EFExampleViewRepository(IDbContext context) : base(context)
        {
        }
        public IEnumerable<ExampleView> GetAllRelatedExampleViews(IEnumerable<int> exampleIds)
        {
            return DbSet.Where(p => exampleIds.Any(c => c == p.ExampleId));
        }

        public ExampleView GetExampleViewByExampleAndUserId(int exampleId, string userId)
        {
            return DbSet.FirstOrDefault(p => p.ExampleId == exampleId && p.UserId == userId);
        }
    }
}
