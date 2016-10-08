namespace Kondor.Data.Constants
{
    public static class RegexPatterns
    {
        public const string RichSideFirstRegex = @"(##.+\s)((--.+\s*)(%%.+\s*)*)+";
        public const string RichSideSecondRegex = @"(--.+\s*)(%%.+\s*)*";
    }
}
