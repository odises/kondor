using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kondor.Service.Extensions
{
    public static class StringExtensions
    {
        public static string GetBase64Encode(this string source)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(source);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string GetBase64Decode(this string source)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(source);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
