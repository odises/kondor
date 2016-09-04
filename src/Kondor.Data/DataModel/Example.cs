namespace Kondor.Data.DataModel
{
    public class Example : Entity
    {
        public string Sentence { get; set; }
        public int WordId { get; set; }
        public virtual Word Word { get; set; }
    }
}
