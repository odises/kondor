using System;
using System.Collections.Generic;

namespace Kondor.Data.LeitnerDataModels
{
    public class RichSide : ISide
    {
        public RichSide()
        {
            PartsOfSpeech = new List<PartOfSpeech>();
        }

        public List<PartOfSpeech> PartsOfSpeech { get; set; }

        public string Display()
        {
            var result = "";
            foreach (var partOfSpeech in PartsOfSpeech)
            {
                result = result + $"`{partOfSpeech.Title}`{Environment.NewLine}{Environment.NewLine}";

                var defCount = 0;
                foreach (var definition in partOfSpeech.Definitions)
                {
                    result = result + $"{defCount + 1}. {definition.Value}{Environment.NewLine}";

                    foreach (var example in definition.Examples)
                    {
                        result = result + $"- _{example.Value}_{Environment.NewLine}";
                    }

                    defCount++;
                }
            }

            return result;
        }
    }
}