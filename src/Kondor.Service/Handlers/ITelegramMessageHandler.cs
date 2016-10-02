using Kondor.Data.TelegramTypes;

namespace Kondor.Service.Handlers
{
    public interface ITelegramMessageHandler
    {
        void MessageProcessor(Message message);
        int ProcessMessages();
        void SaveUpdate(Update update);
        void SaveUpdates();
    }
}