using System.Linq;
using Kondor.Domain;
using Kondor.Domain.Models;

namespace Kondor.Data.EF
{
    public class EFSettingRepository : EFRepository<Setting>, ISettingRepository
    {
        public EFSettingRepository(IDbContext context) : base(context)
        {
        }
        public Setting GetSettingByType(string type)
        {
            return DbSet.FirstOrDefault(p => p.SettingType == type);
        }
    }
}