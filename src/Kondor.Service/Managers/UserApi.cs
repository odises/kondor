using System;
using System.Linq;
using Kondor.Domain;
using Kondor.Domain.Enums;
using Kondor.Service.Extensions;

namespace Kondor.Service.Managers
{
    public class UserApi : IUserApi
    {
        private readonly IUserRepository _userRepository;

        public UserApi(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool IsRegisteredUser(int telegramUserId)
        {
            var user = _userRepository.GetUserByTelegramId(telegramUserId);
            if (user != null)
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
            var user = _userRepository.GetUserByTelegramId(telegramUserId);
            var updates = _userRepository.GetUserUpdates(user.Id, new TimeSpan(0, minutes, 0));
            if (updates.Any())
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
