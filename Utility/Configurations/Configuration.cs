using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servize.Utility.Configurations
{
    public class Configuration
    {

        public static void AddParameter(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) return;
            Environment.SetEnvironmentVariable(key, value);
        }

        public static string GetParameterValue(string key)
        {
            string result = Environment.GetEnvironmentVariable(key);
            if (result == null)
                result = Environment.GetEnvironmentVariable(key.Replace('.', '_'));
            return result;
        }
    }
}
