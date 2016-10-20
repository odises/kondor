using System;
using System.Collections.Generic;
using Kondor.Data.LeitnerDataModels;

namespace Kondor.Domain.LeitnerDataModels
{
    public class RichSide : ISide
    {
        public RichSide()
        {
            PartsOfSpeech = new List<PartOfSpeech>();
            Pronunciations = new List<Pronunciation>();
        }

        public List<PartOfSpeech> PartsOfSpeech { get; set; }
        public List<Pronunciation> Pronunciations { get; set; } 

        public string Raw()
        {
            var result = "";
            foreach (var pronunciation in Pronunciations)
            {
                result = result + $"@@{pronunciation.Region}({pronunciation.Value}){Environment.NewLine}";
            }
            foreach (var partOfSpeech in PartsOfSpeech)
            {
                result = result + $"##{partOfSpeech.Title}{Environment.NewLine}";
                foreach (var definition in partOfSpeech.Definitions)
                {
                    result = result + $"--{definition.Value}{Environment.NewLine}";

                    foreach (var example in definition.Examples)
                    {
                        result = result + $"%%{example.Value}{Environment.NewLine}";
                    }
                }
            }
            return result.TrimEnd(Environment.NewLine.ToCharArray());
        }

        public string Display()
        {
            var result = "";

            foreach (var partOfSpeech in PartsOfSpeech)
            {
                result = result + $"`{partOfSpeech.Title}`{Environment.NewLine}";
                if (Pronunciations.Count > 0)
                {
                    foreach (var pronunciation in Pronunciations)
                    {
                        result = result + $"`{pronunciation.Region} /{pronunciation.Value}/`{Environment.NewLine}";
                    }
                }

                result = result + Environment.NewLine;

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

                result = result + Environment.NewLine + Environment.NewLine;
            }

            return result;
        }
    }
}