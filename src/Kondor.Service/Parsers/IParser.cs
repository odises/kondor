using Kondor.Data.LeitnerDataModels;

namespace Kondor.Service.Parsers
{
    public interface IParser
    {
        ISide ParseSimpleSide(string input);
        ISide ParseRichSide(string input);
    }
}