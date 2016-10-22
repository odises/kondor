using Kondor.Domain.LeitnerDataModels;

namespace Kondor.Service.Parsers
{
    public interface IParser
    {
        ISide ParseSimpleSide(string input);
        IRichSide ParseRichSide(string input);
    }
}