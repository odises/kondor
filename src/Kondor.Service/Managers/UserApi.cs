using System;
using System.Linq;
using Kondor.Domain;
using Kondor.Domain.Enums;
using Kondor.Service.Extensions;

namespace Kondor.Service.Managers
{
    public class UserApi : IUserApi
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserApi(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool IsRegisteredUser(int telegramUserId)
        {
            var user = _unitOfWork.UserRepository.GetUserByTelegramId(telegramUserId);
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
            var user = _unitOfWork.UserRepository.GetUserByTelegramId(telegramUserId);
            var updates = _unitOfWork.UserRepository.GetUserUpdates(user.Id, new TimeSpan(0, minutes, 0));
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
