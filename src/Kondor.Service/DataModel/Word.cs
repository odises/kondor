using System.Collections.Generic;

namespace Kondor.Service.DataModel
{
    public class Word 
    {
        public int Id { get; set; }

        public string Vocabulary { get; set; }

        public string Definition { get; set; }

        public ICollection<Card> Cards { get; set; } 
    }
}
