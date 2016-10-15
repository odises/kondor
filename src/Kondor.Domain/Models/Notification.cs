using System;

namespace Kondor.Domain.Models
{
    public class Notification : Entity
    {
        public int TelegramUserId { get; set; }
        public DateTime CreationDatetime { get; set; }
    }
}
