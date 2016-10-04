using System;
using System.Collections.Generic;

namespace Kondor.Data.LeitnerDataModels
{
    public class RichSide : ISide
    {
        public RichSide()
        {
            PartsOfSpeech = new List<PartOfSpeech>();
        }

        public List<PartOfSpeech> PartsOfSpeech { get; set; }

        public string Display()
        {
            throw new NotImplementedException();
        }
    }
}