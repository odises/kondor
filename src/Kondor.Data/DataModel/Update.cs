using System;
using Kondor.Data.Enums;

namespace Kondor.Data.DataModel
{
    public class Update : Entity
    {
        public int UpdateId { get; set; }
        public UpdateType UpdateType { get; set; }
        public string SerializedUpdate { get; set; }
        public DateTime CreationDatetime { get; set; }
        public int Status { get; set; }
        public DateTime ModifiedDatetime { get; set; }
    }
}
