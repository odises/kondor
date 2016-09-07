namespace Kondor.Data.DataModel
{
    public class Medium : Entity
    {
        public int MemId { get; set; }
        public byte[] MediumContent { get; set; }
        public string ContentType { get; set; }
        public virtual Mem Mem { get; set; }
    }
}
