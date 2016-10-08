using System;
using System.IO;
using System.Text.RegularExpressions;
using Kondor.Service.Extensions;
using Kondor.Service.Parsers;

namespace Kondor.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var streamReader = new StreamReader(@"c:\test\havij.txt"))
            {
                var text = streamReader.ReadToEnd();
                var result = Regex.IsMatch(text, Data.Constants.RegexPatterns.RichSideFirstRegex);
            }
            
        }
    }
}
