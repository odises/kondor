using System.Collections.Generic;
using System.Linq;
using Kondor.Domain;
using Kondor.Domain.Enums;
using Kondor.Domain.Models;

namespace Kondor.Data.EF
{
    public class EFUpdateRepository : EFRepository<Update>, IUpdateRepository
    {
        public EFUpdateRepository(IDbContext context) : base(context)
        {
        }
        public Update GetLastUpdate()
        {
            return DbSet.OrderByDescending(p => p.Id).FirstOrDefault();
        }

        public Update GetUpdateByUpdateId(int updateId)
        {
            return DbSet.FirstOrDefault(p => p.UpdateId == updateId);
        }

        public IEnumerable<Update> GetUpdatesByStatus(UpdateStatus status)
        {
            return DbSet.Where(p => p.Status == status);
        }
    }
}
