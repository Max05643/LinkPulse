using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkPulseDefinitions
{
    /// <summary>
    /// Provides a way to get a short hash from an arbitrary string input
    /// </summary>
    public interface IHashProvider
    {
        /// <summary>
        /// Provides a short hash from input
        /// </summary>
        string Hash(string input);
    }
}
