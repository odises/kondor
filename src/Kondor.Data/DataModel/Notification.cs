using System;

namespace Kondor.Data.DataModel
{
    public class Notification : Entity
    {
        public int ChatId { get; set; }
        public DateTime CreationDatetime { get; set; }
    }
}
