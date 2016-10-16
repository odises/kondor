using System;
using System.Collections.Generic;
using Kondor.Domain.Models;

namespace Kondor.Domain
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        ApplicationUser GetUserByTelegramId(int telegramUserId);
        IEnumerable<Update> GetUserUpdates(string id);
        IEnumerable<Update> GetUserUpdates(string id, TimeSpan timeSpan);
        IEnumerable<ApplicationUser> GetUsersThatShouldBeNotified(int maximumAlertsNumber, TimeSpan interval);
    }
}
