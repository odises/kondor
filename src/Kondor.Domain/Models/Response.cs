using Kondor.Domain.Enums;

namespace Kondor.Domain.Models
{
    public class Response : Entity
    {
        public int TelegramUserId { get; set; }
        public int ChatId { get; set; }
        public string MessageId { get; set; }
        public ResponseStatus Status { get; set; }
    }
}
