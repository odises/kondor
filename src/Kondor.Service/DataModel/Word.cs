using System.Collections.Generic;
using System.Security.Policy;

namespace Kondor.Service.DataModel
{
    public class Word
    {
        public int Id { get; set; }

        public string Vocabulary { get; set; }

        public string Definition { get; set; }

        public ICollection<Card> Cards { get; set; }
        public ICollection<Example> Examples { get; set; }
        public ICollection<Word> Media { get; set; }
    }
}
