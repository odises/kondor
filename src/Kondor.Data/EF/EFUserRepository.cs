using System;
using System.Collections.Generic;
using System.Linq;
using Kondor.Domain;
using Kondor.Domain.Models;

namespace Kondor.Data.EF
{
    public class EFUserRepository : IUserRepository
    {
        private readonly IDbContext _context;
        private bool _disposed; // by default value is 'false'

        public EFUserRepository(IDbContext context)
        {
            this._context = context;
        }

        public ApplicationUser GetUserByTelegramId(int telegramUserId)
        {
            return _context.Set<ApplicationUser>().FirstOrDefault(p => p.TelegramUserId == telegramUserId);
        }

        protected IQueryable<Update> GetAllUserUpdates(string id)
        {
            var user = _context.Set<ApplicationUser>().Find(id);
            var updates = _context.Updates.Where(p => p.FromId == user.TelegramUserId);
            return updates;
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

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }
    }
}
