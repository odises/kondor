using System;
using System.Collections.Generic;
using Kondor.Domain.Models;

namespace Kondor.Domain
{
    public interface IUserRepository : IDisposable
    {
        ApplicationUser GetUserByTelegramId(int telegramUserid);
        IEnumerable<Update> GetUserUpdates(string id);
        IEnumerable<Update> GetUserUpdates(string id, TimeSpan timeSpan);
        void Save();
    }
}
