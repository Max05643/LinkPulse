using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkPulseDefinitions
{
    /// <summary>
    /// Provides a way to convert full urls to their shortened versions and to retrieve full url by the shortened version 
    /// </summary>
    public interface IShortenerController
    {
        /// <summary>
        /// Tries to save provided url and assigns a shortenedVersion version of it. Returns true on success
        /// </summary>
        bool TryAddNewURL(string url, out string? shortenedVersion);

        /// <summary>
        /// Tries to get a full URL by the shortened version. Will assign fullUrl and return true on success
        /// </summary>
        bool TryGetURLByShortenedVersion(string shortenedVersion, out string? fullUrl);
    }
}
