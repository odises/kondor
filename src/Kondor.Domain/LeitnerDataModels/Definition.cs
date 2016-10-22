using System;
using System.Collections.Generic;

namespace Kondor.Domain.LeitnerDataModels
{
    public class Definition
    {
        public Definition()
        {
            Examples = new List<Example>();
            Synonyms = new List<string>();
        }

        public string Value;
        public List<Example> Examples { get; set; }
        public List<string> Synonyms { get; set; }
    }

    public class Example
    {
        public Example()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Value { get; set; }
    }
}