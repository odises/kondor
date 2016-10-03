namespace Kondor.Data.LeitnerDataModels
{
    public class SimpleCard : ICard
    {
        public SimpleCard()
        {
            Front = new SimpleSide();
            Back = new SimpleSide();
        }

        public ISide Front { get; set; }
        public ISide Back { get; set; }
    }
}
