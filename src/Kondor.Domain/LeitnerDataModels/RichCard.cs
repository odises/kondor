using System;

namespace Kondor.Domain.LeitnerDataModels
{
    public class RichCard : IRichCard
    {
        public RichCard()
        {
            Front = new SimpleSide();
            Back = new RichSide();
        }

        public string GetLearnView()
        {
            return $"*{Front.Display()}*\n\n{Back.Display()}";
        }

        public string GetFrontExamView()
        {
            return $"*{Front.Display()}*";
        }

        public string GetBackExamView()
        {
            var back = Back as RichSide;
            if (back == null)
            {
                throw new InvalidCastException();
            }
            return $"*{Front.Display()}*\n\n{back.DisplayWithoutExamples()}";
        }

        public ISide Front { get; set; }
        public IRichSide Back { get; set; }
    }
}