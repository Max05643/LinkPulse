using LinkPulseDefinitions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkPulseImplementations
{
    public class ShortenerController : IShortenerController
    {
        const int maxTries = 20;

        readonly IHashProvider hashProvider;
        readonly IStorage storage;
        readonly TimeSpan? expirationTime;
        readonly ILogger<ShortenerController> logger;
        readonly bool shouldUpdateExpirationTimeOnRead;

        public ShortenerController(IHashProvider hashProvider, IStorage storage, IConfiguration configuration, ILogger<ShortenerController> logger)
        {
            this.hashProvider = hashProvider;
            this.storage = storage;
            this.logger = logger;

            var expirationTimeConfig = configuration["Shortener:ExpirationTimeSeconds"];

            if (expirationTimeConfig == null)
            {
                expirationTime = null;
                logger.LogWarning("'Shortener:ExpirationTimeSeconds' key was not found in configuration for ShortenerController. Expiration time for urls won't be used");
            }
            else
            {
                expirationTime = TimeSpan.FromSeconds(int.Parse(expirationTimeConfig));
            }


            var expirationUpdateConfig = configuration["Shortener:ExpandExpirationTimeOnEveryUse"];

            if (expirationUpdateConfig == null)
            {
                shouldUpdateExpirationTimeOnRead = false;
                logger.LogWarning("'Shortener:ExpandExpirationTimeOnEveryUse' key was not found in configuration for ShortenerController. Expiration time for urls won't be updated on every usage");
            }
            else
            {
                shouldUpdateExpirationTimeOnRead = bool.Parse(expirationUpdateConfig);
            }
        }

        bool IShortenerController.TryAddNewURL(string url, out string? shortenedVersion)
        {
            for (int i = 0; i < maxTries; i++)
            {
                var shortened = hashProvider.Hash($"{url}{i}");
                var result = storage.TryAddKeyValuePair(shortened, url, expirationTime);

                if (result)
                {
                    shortenedVersion = shortened;
                    return true;
                }
            }

            shortenedVersion = null;
            return false;
        }

        bool IShortenerController.TryGetURLByShortenedVersion(string shortenedVersion, out string? fullUrl)
        {
            return storage.TryGetValue(shortenedVersion, out fullUrl, shouldUpdateExpirationTimeOnRead ? expirationTime : null);
        }
    }
}
