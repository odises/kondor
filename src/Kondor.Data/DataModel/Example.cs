using System.Collections.Generic;

namespace Kondor.Data.DataModel
{
    public class Example : Entity
    {
        public string Sentence { get; set; }
        public int MemId { get; set; }
        public virtual Card Mem { get; set; }
        public virtual ICollection<ExampleView> ExampleViews { get; set; }

        public string ToBoldedString()
        {
            return this.Sentence;
        }
    }
}
