namespace Kondor.Data.LeitnerDataModels
{
    public class RichCard : ICard
    {
        public RichCard()
        {
            Front = new SimpleSide();
            Back = new RichSide();
        }
        public ISide Front { get; set; }
        public ISide Back { get; set; }
    }
}