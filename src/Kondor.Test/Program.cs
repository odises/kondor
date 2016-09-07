using System;
using Kondor.Service;
using Kondor.Service.Extensions;

namespace Kondor.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = StringCipher.Encrypt("salam", "testkey");
            var base64Encoded = result.GetBase64Encode();
            var base64Decoded = base64Encoded.GetBase64Decode();
            var original = StringCipher.Decrypt(base64Decoded, "testkey");
            Console.WriteLine(original);
        }
    }
}
