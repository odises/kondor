namespace Kondor.Data.LeitnerDataModels
{
    public interface ICard
    {
        ISide Front { get; set; }
        ISide Back { get; set; }
    }
}