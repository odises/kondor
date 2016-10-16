using System;
using System.Collections.Generic;
using Kondor.Domain.Models;

namespace Kondor.Domain
{
    public interface ISettingRepository : IRepository<Setting>
    {
        Setting GetSettingByType(string type);
    }
}
