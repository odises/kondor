using System.Collections.Generic;
using System.Linq;
using Kondor.Domain;
using Kondor.Domain.Enums;
using Kondor.Domain.Models;

namespace Kondor.Data.EF
{
    public class EFResponseRepository : EFRepository<Response>, IResponseRepository
    {
        public EFResponseRepository(IDbContext context) : base(context)
        {
        }
        public IEnumerable<IGrouping<int, Response>> GetResponsesGroupedByTelegramUserId()
        {
            return DbSet.Where(p => p.Status == ResponseStatus.New).GroupBy(p => p.TelegramUserId);
        }
    }
}
