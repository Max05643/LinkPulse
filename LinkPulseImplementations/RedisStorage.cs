using LinkPulseDefinitions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LinkPulseImplementations
{
    /// <summary>
    /// Provides a way to store keys in Redis
    /// </summary>
    public class RedisStorage : IStorage
    {
        readonly ConnectionMultiplexer connectionMultiplexer;
        readonly ILogger<RedisStorage> logger;
        readonly Dictionary<string, LuaScript> scriptsPrepared = new();

        public RedisStorage(IConfiguration config, ILogger<RedisStorage> logger)
        {

            var connectionString = config.GetConnectionString("redis") ?? throw new ArgumentException("Can not read 'redis' connection string");

            connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
            this.logger = logger;

            try
            {
                scriptsPrepared.Add("AddKey", LuaScriptsLoader.Load("LinkPulseImplementations.LuaScripts", "AddKey"));
                scriptsPrepared.Add("GetKey", LuaScriptsLoader.Load("LinkPulseImplementations.LuaScripts", "GetKey"));
                scriptsPrepared.Add("GetKeyUpdateExp", LuaScriptsLoader.Load("LinkPulseImplementations.LuaScripts", "GetKeyUpdateExp"));
                scriptsPrepared.Add("AddKeyExp", LuaScriptsLoader.Load("LinkPulseImplementations.LuaScripts", "AddKeyExp"));
            }
            catch (Exception ex)
            {
                logger.LogError("Error while preparing scripts for RedisStorage: {ex}", ex);
            }
        }

        bool IStorage.TryAddKeyValuePair(string key, string value, TimeSpan? timeToExpire)
        {
            try
            {
                var redisClient = connectionMultiplexer.GetDatabase();

                if (timeToExpire == null)
                {
                    var args = new { key = (RedisKey)key, val = (RedisValue)value };
                    var result = (bool)redisClient.ScriptEvaluate(scriptsPrepared["AddKey"], args);
                    return result;
                }
                else
                {
                    var args = new { key = (RedisKey)key, val = (RedisValue)value, exp = (RedisValue)((int)timeToExpire.Value.TotalMilliseconds) };
                    var result = (bool)redisClient.ScriptEvaluate(scriptsPrepared["AddKeyExp"], args);
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error while adding a new key in RedisStorage: {ex}", ex);
                return false;
            }
        }

        bool IStorage.TryGetValue(string key, out string? value, TimeSpan? newTimeToExpire)
        {
            try
            {
                var redisClient = connectionMultiplexer.GetDatabase();


                RedisResult? result = null;

                if (!newTimeToExpire.HasValue)
                {
                    var args = new { key = (RedisKey)key };
                    result = redisClient.ScriptEvaluate(scriptsPrepared["GetKey"], args);
                }
                else
                {
                    var args = new { key = (RedisKey)key, exp = (RedisValue)((int)newTimeToExpire.Value.TotalMilliseconds) };
                    result = redisClient.ScriptEvaluate(scriptsPrepared["GetKeyUpdateExp"], args);
                }


                if (result == null || result.IsNull)
                {
                    value = null;
                    return false;
                }
                else
                {
                    value = (string)result!;
                    return true;
                }

            }
            catch (Exception ex)
            {
                logger.LogError("Error while trying to get a value from RedisStorage: {ex}", ex);
                value = null;
                return false;
            }
        }
    }
}
