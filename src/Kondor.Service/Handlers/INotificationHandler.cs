using System.Collections.Generic;
using Kondor.Data.DataModel;

namespace Kondor.Service.Handlers
{
    public interface INotificationHandler
    {
        void SendNotification();
        bool UserShouldBeNotified(int telegramUserId);
        List<ApplicationUser> UsersThatShouldBeNotified();
    }
}