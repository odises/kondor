using System;
using System.Linq;
using Kondor.Data;
using Kondor.Data.DataModel;
using Kondor.Data.Enums;
using Kondor.Service.Extensions;

namespace Kondor.Service.Managers
{
    public class UserApi : IUserApi
    {
        private readonly IDbContext _context;

        public UserApi(IDbContext context)
        {
            _context = context;
        }

        public bool IsRegisteredUser(int telegramUserId)
        {
            if (_context.Set<ApplicationUser>().Any(p => p.TelegramUserId == telegramUserId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public UserState GetUserState(int telegramUserId, int minutes)
        {
            var datetime = DateTime.Now.AddMinutes(minutes * -1);

            if (_context.Updates.Any(p => p.FromId == telegramUserId && p.CreationDatetime > datetime))
            {
                return UserState.Active;
            }
            else
            {
                return UserState.Idle;
            }
        }

        public string GetRegistrationLink(int telegramUserId, string telegramUsername, string baseUri, string cipherKey)
        {
            var encrypted = StringCipher.Encrypt($"{telegramUserId}:{telegramUsername}", cipherKey);
            var base64Encoded = encrypted.GetBase64Encode();
            return $"{baseUri}/{base64Encoded}";
        }
    }
}
