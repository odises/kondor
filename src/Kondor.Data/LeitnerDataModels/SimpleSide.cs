namespace Kondor.Data.LeitnerDataModels
{
    public class SimpleSide : ISide
    {
        public string Value { get; set; }

        public string Display()
        {
            return Value;
        }
    }
}