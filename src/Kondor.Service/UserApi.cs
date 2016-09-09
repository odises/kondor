using System.Linq;
using Kondor.Data;
using Kondor.Service.Extensions;

namespace Kondor.Service
{
    public class UserApi
    {
        public bool IsRegisteredUser(int telegramUserId)
        {
            var entityContext = new EntityContext();
            if (entityContext.Users.Any(p => p.TelegramUserId == telegramUserId))
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
