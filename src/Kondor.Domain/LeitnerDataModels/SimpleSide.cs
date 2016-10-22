namespace Kondor.Domain.LeitnerDataModels
{
    public class SimpleSide : ISide
    {
        public string Value { get; set; }

        public string Raw()
        {
            return Value;
        }

        public string Display()
        {
            return Value;
        }
    }
}