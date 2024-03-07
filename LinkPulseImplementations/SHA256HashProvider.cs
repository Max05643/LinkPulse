using LinkPulseDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace LinkPulseImplementations
{
    /// <summary>
    /// Provides a way to compute shortened version of sha256 hash. It should not be used for security purposes
    /// </summary>
    public class SHA256HashProvider : IHashProvider
    {
        string IHashProvider.Hash(string input)
        {
            return Convert.ToBase64String(SHA256.HashData(Encoding.Unicode.GetBytes(input)))[..15];
        }
    }
}
