using System;
using System.Linq;
using System.Text.RegularExpressions;
using Kondor.Data.LeitnerDataModels;

namespace Kondor.Service.Parsers
{
    public class RichSideParser : IParser
    {
        public ISide ParseSimpleSide(string input)
        {
            var simpleSide = new SimpleSide { Value = input };
            return simpleSide;
        }

        public ISide ParseRichSide(string input)
        {
            var richSide = new RichSide();

            var regex = new Regex(Data.Constants.RegexPatterns.RichSideFirstRegex);
            var text = input;
            var temp = regex.Matches(text);

            var match = temp.Cast<Match>();

            foreach (var part in match)
            {
                var partOfSpeech = new PartOfSpeech();

                var partOfSpeechTitle = part.Groups[1].Value.Replace(Environment.NewLine, "").Replace("#", "");

                partOfSpeech.Title = partOfSpeechTitle;

                var translations = part.Groups[2].Captures.Cast<Capture>();

                foreach (var item in translations)
                {
                    var definition = new Definition();

                    var reg = new Regex(Data.Constants.RegexPatterns.RichSideSecondRegex);
                    var t = reg.Matches(item.Value);
                    var m = t.Cast<Match>();

                    var firstMatch = m.FirstOrDefault();
                    if (firstMatch == null) // in this case we don't have any example for this definition
                    {
                        definition.Value = item.Value.Replace(Environment.NewLine, "").Replace("- ", "");
                    }
                    else
                    {
                        definition.Value = firstMatch.Groups[1].Value.Replace(Environment.NewLine, "").Replace("- ", "");

                        definition.Examples = firstMatch.Groups[2]
                            .Captures
                            .Cast<Capture>()
                            .Select(p => new Example { Value = p.Value.Replace(Environment.NewLine, "").Replace("$ ", "") })
                            .ToList();
                    }

                    partOfSpeech.Definitions.Add(definition);
                }

                richSide.PartsOfSpeech.Add(partOfSpeech);
            }

            return richSide;
        }
    }
}
