namespace Kondor.Data.LeitnerDataModels
{
    public class SimpleCard : ICard
    {
        public SimpleCard()
        {
            Front = new SimpleSide();
            Back = new SimpleSide();
        }

        public string GetLearnView()
        {
            throw new System.NotImplementedException();
        }

        public string GetFrontExamView()
        {
            throw new System.NotImplementedException();
        }

        public string GetBackExamView()
        {
            throw new System.NotImplementedException();
        }

        public ISide Front { get; set; }
        public ISide Back { get; set; }
    }
}
