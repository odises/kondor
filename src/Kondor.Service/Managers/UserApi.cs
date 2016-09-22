using System.Linq;
using Kondor.Data;
using Kondor.Data.DataModel;
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

        public string GetRegistrationLink(int telegramUserId, string telegramUsername, string baseUri, string cipherKey)
        {
            var encrypted = StringCipher.Encrypt($"{telegramUserId}:{telegramUsername}", cipherKey);
            var base64Encoded = encrypted.GetBase64Encode();
            return $"{baseUri}/{base64Encoded}";
        }
    }
}
