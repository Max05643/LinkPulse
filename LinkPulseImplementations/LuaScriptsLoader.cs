using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LinkPulseImplementations
{

    /// <summary>
    /// Contains helper methods for loading Lua scripts for Redis
    /// </summary>
    internal static class LuaScriptsLoader
    {
        /// <summary>
        /// Prepares Redis script from the embedded resource
        /// </summary>
        public static LuaScript Load(string scriptLocation, string scriptName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream($"{scriptLocation}.{scriptName}") ?? throw new IOException("Resource was not found");
            using var reader = new StreamReader(stream);
            return LuaScript.Prepare(reader.ReadToEnd());
        }
    }
}
