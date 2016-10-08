namespace Kondor.Data.Constants
{
    public static class RegexPatterns
    {
        public const string RichSideFirstRegex = @"(\#[\w\s]+)((\-[\w\s,';.?!\(\)]+)(\$[\w\s,';.?!\(\)]+)*)+";
        public const string RichSideSecondRegex = @"(\-[\w\s,';.?!\(\)]+)(\$[\w\s,';.?!\(\)]+)+";
    }
}
