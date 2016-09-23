using System;

namespace Kondor.Data.DataModel
{
    public class Notification : Entity
    {
        public int TelegramUserId { get; set; }
        public DateTime CreationDatetime { get; set; }
    }
}
