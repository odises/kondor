using System.Collections.Generic;

namespace Kondor.Service
{
    public class PartOfSpeech
    {
        public string Title { get; set; }
        public List<string> Definitions { get; set; } 
    }

    public class Vocabulary
    {
        public List<PartOfSpeech> Parts { get; set; } 
        public List<string> Persians { get; set; }
        public List<string> Examples { get; set; }  
    }
}
