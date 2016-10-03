using System.Collections.Generic;

namespace Kondor.Data.LeitnerDataModels
{
    public class Definition
    {
        public Definition()
        {
            Examples = new List<string>();
            Synonyms = new List<string>();
        }

        public string Value;
        public List<string> Examples { get; set; }
        public List<string> Synonyms { get; set; }
    }
}