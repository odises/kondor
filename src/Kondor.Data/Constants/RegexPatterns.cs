namespace Kondor.Data.Constants
{
    public static class RegexPatterns
    {
        public const string RichSideFirstRegex = @"(##.+\s)((--.+\s*)(%%.+\s*)*)+";
        public const string RichSideSecondRegex = @"(--.+\s*)(%%.+\s*)*";
        public const string PronunciationRegex = @"(@@[A-Z]{2}\(.+\)\s*)*";
        public const string InsidePronunciationRegex = @"(@@)([A-Z]{2})\((.+)\)";
    }
}
