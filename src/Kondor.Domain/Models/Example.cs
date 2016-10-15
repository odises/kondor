using System;
using System.Collections.Generic;

namespace Kondor.Domain.Models
{
    public class Example : Entity
    {
        public int CardId { get; set; }
        public Guid ExampleUniqueId { get; set; }
        public string Sentence { get; set; }
        public virtual Card Card { get; set; }
        public virtual ICollection<ExampleView> ExampleViews { get; set; }
    }
}
