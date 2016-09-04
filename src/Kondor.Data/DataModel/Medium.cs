using Kondor.Data.Enums;

namespace Kondor.Data.DataModel
{
    public class Medium : Entity
    {
        public int WordId { get; set; }
        public byte[] MediumContent { get; set; }
        public string ContentType { get; set; }
        public virtual Word Word { get; set; }
    }
}
