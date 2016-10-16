using System.Collections.Generic;
using Kondor.Domain.Models;

namespace Kondor.Domain
{
    public interface IExampleViewRepository : IRepository<ExampleView>
    {
        IEnumerable<ExampleView> GetAllRelatedExampleViews(IEnumerable<int> exampleIds);
        ExampleView GetExampleViewByExampleAndUserId(int exampleId, string userId);
    }
}
