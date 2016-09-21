using Kondor.Data.Enums;

namespace Kondor.Data.DataModel
{
    public class Response : Entity
    {
        public int ChatId { get; set; }
        public string MessageId { get; set; }
        public ResponseStatus Status { get; set; }
    }
}
