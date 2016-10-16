using System.Collections.Generic;
using Kondor.Domain.Models;

namespace Kondor.Domain
{
    public interface IExampleRepository : IRepository<Example>
    {
        IEnumerable<Example> GetExamplesByCardId(int id);
    }
}
