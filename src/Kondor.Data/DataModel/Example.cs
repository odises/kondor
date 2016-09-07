namespace Kondor.Data.DataModel
{
    public class Example : Entity
    {
        public string Sentence { get; set; }
        public int MemId { get; set; }
        public virtual Mem Mem { get; set; }
    }
}
