using System;
using System.Collections.Generic;

namespace Kondor.Data.DataModel
{
    public class Example : Entity
    {
        public Guid ExampleUniqueId { get; set; }
        public string Sentence { get; set; }
        public virtual ICollection<ExampleView> ExampleViews { get; set; }
    }
}
