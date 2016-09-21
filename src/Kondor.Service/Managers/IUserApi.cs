namespace Kondor.Service.Managers
{
    public interface IUserApi
    {
        string GetRegistrationLink(int telegramUserId, string telegramUsername, string baseUri, string cipherKey);
        bool IsRegisteredUser(int telegramUserId);
    }
}