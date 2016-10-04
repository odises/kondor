using System.Collections.Generic;

namespace Kondor.Data.LeitnerDataModels
{
    public class PartOfSpeech
    {
        public PartOfSpeech()
        {
            Definitions = new List<Definition>();
        }

        public string Title { get; set; }
        public List<Definition> Definitions { get; set; }
    }
}