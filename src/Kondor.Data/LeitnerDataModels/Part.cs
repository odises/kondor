using System.Collections.Generic;

namespace Kondor.Data.LeitnerDataModels
{
    public class Part
    {
        public Part()
        {
            Definitions = new List<Definition>();
        }

        public string Value { get; set; }
        public List<Definition> Definitions { get; set; }
    }
}