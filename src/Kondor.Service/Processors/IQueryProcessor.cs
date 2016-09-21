using Kondor.Data.TelegramTypes;

namespace Kondor.Service.Processors
{
    public interface IQueryProcessor
    {
        void Process(CallbackQuery callbackQuery);
    }
}