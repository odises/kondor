using Kondor.Data.Enums;

namespace Kondor.Data.SettingModels
{
    public class GeneralSettings : ISettings
    {
        public string TelegramApiKey { get; set; }
        public string SendMessageEndPoint { get; set; }
        public string AnswerCallbackQueryEndPoint { get; set; }
        public string EditMessageTextEndPoint { get; set; }
        public string GetUpdatesEndPoint { get; set; }
        public int MaximumNumberOfAlert { get; set; }
        public int AlertsInterval { get; set; }
        public int UserStateTolerance { get; set; }
        public string RemovedMessagesText { get; set; }
        public TimeUnit LeitnerTimeUnit { get; set; }
        public int FirstBoxCapacity { get; set; }
        public int LeitnerOverstoppingTolerance { get; set; }
        public int TelegramTaskInterval { get; set; }
        public int CleanUpTaskInterval { get; set; }
        public int NotificationTaskInterval { get; set; }
    }
}
