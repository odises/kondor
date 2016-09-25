using System.Collections.Generic;

namespace Kondor.Data.DataModel
{
    public class Mem : Entity
    {
        public Mem()
        {
            Cards = new HashSet<Card>();
            Examples = new HashSet<Example>();
            Media = new HashSet<Medium>();
        }
        public string UserId { get; set; }
        public string MemBody { get; set; }
        public string Definition { get; set; }
        public virtual ICollection<Card> Cards { get; set; }
        public virtual ICollection<Example> Examples { get; set; }
        public virtual ICollection<Medium> Media { get; set; }
        public virtual ApplicationUser User { get; set; }

        public string GenerateCardView()
        {
            var result = $"{this.MemBody}\n\n{this.Definition}";
            return result;
        }
    }
}
