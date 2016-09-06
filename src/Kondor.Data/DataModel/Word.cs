using System.Collections.Generic;

namespace Kondor.Data.DataModel
{
    public class Word : Entity
    {
        public Word()
        {
            Cards = new HashSet<Card>();
            Examples = new HashSet<Example>();
            Media = new HashSet<Medium>();
        }
        public string UserId { get; set; }
        public string Vocabulary { get; set; }
        public string Definition { get; set; }
        public ICollection<Card> Cards { get; set; }
        public ICollection<Example> Examples { get; set; }
        public ICollection<Medium> Media { get; set; }
        public virtual ApplicationUser User { get; set; } 
    }
}
