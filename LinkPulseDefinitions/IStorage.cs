using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkPulseDefinitions
{
    /// <summary>
    /// Provides a thread-safe way to store key-value string pairs with an optional expiration time
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Tries to add a new key-value pair to the storage. If timeToExpire is not null the key will expire. Can successfully add a key if such a key does not already exist. Returns the result of adding a pair  
        /// </summary>
        bool TryAddKeyValuePair(string key, string value, TimeSpan? timeToExpire = null);

        /// <summary>
        /// Tries to get a value by the key. Will assign value and return true on success
        /// </summary>
        bool TryGetValue(string key, out string? value);
    }
}
