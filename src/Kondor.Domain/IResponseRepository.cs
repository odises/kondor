using System;
using System.Collections.Generic;
using System.Linq;
using Kondor.Domain.Models;

namespace Kondor.Domain
{
    public interface IResponseRepository : IRepository<Response>
    {
        IEnumerable<IGrouping<int, Response>> GetResponsesGroupedByTelegramUserId();
    }
}
