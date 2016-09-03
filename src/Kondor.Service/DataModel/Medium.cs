using Kondor.Service.Enums;

namespace Kondor.Service.DataModel
{
    public class Medium
    {
        public int Id { get; set; }
        public int WordId { get; set; }
        public byte[] Content { get; set; }
        public MediumType ContentType { get; set; }
        public virtual Word Word { get; set; }
    }
}
