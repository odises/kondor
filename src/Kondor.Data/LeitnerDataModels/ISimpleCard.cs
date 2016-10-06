namespace Kondor.Data.LeitnerDataModels
{
    public interface ICard
    {
        string GetLearnView();
        string GetFrontExamView();
        string GetBackExamView();
        ISide Front { get; set; }
        ISide Back { get; set; }
    }
}