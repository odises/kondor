using System.Collections.Generic;
using Kondor.Domain.Enums;
using Kondor.Domain.Models;

namespace Kondor.Domain
{
    public interface IUpdateRepository : IRepository<Update>
    {
        Update GetLastUpdate();
        Update GetUpdateByUpdateId(int updateId);
        IEnumerable<Update> GetUpdatesByStatus(UpdateStatus status);
    }
}
