using System;
using System.Collections.Generic;

namespace Kondor.Data.LeitnerDataModels
{
    public class RichSide : ISide
    {
        public RichSide()
        {
            Parts = new List<Part>();
        }

        public List<Part> Parts { get; set; }

        public string Display()
        {
            throw new NotImplementedException();
        }
    }
}