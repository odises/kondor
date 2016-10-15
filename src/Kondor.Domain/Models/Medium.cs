namespace Kondor.Domain.Models
{
    public class Medium : Entity
    {
        public int MemId { get; set; }
        public byte[] MediumContent { get; set; }
        public string ContentType { get; set; }
    }
}
