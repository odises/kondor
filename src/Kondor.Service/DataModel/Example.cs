using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kondor.Service.DataModel
{
    public class Example
    {
        public int Id { get; set; }
        public string Sentence { get; set; }
        public int WordId { get; set; }
        public virtual Word Word { get; set; }
    }
}
