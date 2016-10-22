namespace Kondor.Domain.LeitnerDataModels
{
    public interface ISimpleCard
    {
        string GetLearnView();
        string GetFrontExamView();
        string GetBackExamView();
    }
}