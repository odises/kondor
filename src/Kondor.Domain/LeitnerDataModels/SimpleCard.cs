namespace Kondor.Domain.LeitnerDataModels
{
    public class SimpleCard : ISimpleCard
    {
        public SimpleCard()
        {
            Front = new SimpleSide();
            Back = new SimpleSide();
        }

        public string GetLearnView()
        {
            return $"*{Front.Display()}*\n\n{Back.Display()}";
        }

        public string GetFrontExamView()
        {
            return $"*{Front.Display()}*";
        }

        public string GetBackExamView()
        {
            return GetLearnView();
        }

        public ISide Front { get; set; }
        public ISide Back { get; set; }
    }
}
