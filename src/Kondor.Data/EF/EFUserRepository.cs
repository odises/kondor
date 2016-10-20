using System;
using System.Collections.Generic;
using System.Linq;
using Kondor.Domain;
using Kondor.Domain.Enums;
using Kondor.Domain.Models;

namespace Kondor.Data.EF
{
    public class EFUserRepository : EFRepository<ApplicationUser>, IUserRepository
    {
        public EFUserRepository(IDbContext context) : base(context)
        {
        }

        public ApplicationUser GetUserByTelegramId(int telegramUserId)
        {
            return DbSet.FirstOrDefault(p => p.TelegramUserId == telegramUserId);
        }

        public IEnumerable<Update> GetUserUpdates(string id)
        {
            var updates = GetAllUserUpdates(id);
            return updates;
        }

        public IEnumerable<Update> GetUserUpdates(string id, TimeSpan timeSpan)
        {
            var originDateTime = DateTime.Now - timeSpan;

            var allUpdates = GetAllUserUpdates(id);

            var result = allUpdates.Where(p => p.CreationDatetime > originDateTime);

            return result;
        }

        public IEnumerable<ApplicationUser> GetUsersThatShouldBeNotified(int maximumAlertsNumber, TimeSpan interval)
        {
            // todo maximum alert number
            var originDateTime = DateTime.Now - interval;

            var result =
                from user in
                    DbContext.CardStates.Where(
                        p =>
                            p.Status == InboxCardsStatus.NewInPosition && p.CardPosition != Position.Finished &&
                            p.ExaminationDateTime <= DateTime.Now)
                        .GroupBy(p => p.UserId).Select(s => s.FirstOrDefault().User)
                where
                    !DbContext.Notifications.Any(
                        p => p.TelegramUserId == user.TelegramUserId && p.CreationDatetime > originDateTime)
                //&& DbContext.Notifications.Count(p => p.TelegramUserId == user.TelegramUserId) < maximumAlertsNumber
                select user;

            return result;
        }

        protected IQueryable<Update> GetAllUserUpdates(string id)
        {
            var user = this.GetById(id);
            var updates = DbContext.Updates.Where(p => p.FromId == user.TelegramUserId);
            return updates;
        }
    }
}
