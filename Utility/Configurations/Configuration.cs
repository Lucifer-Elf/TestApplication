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

        private static string GetParameterValue(string key)
        {
            string result = Environment.GetEnvironmentVariable(key);
            if (result == null)
                result = Environment.GetEnvironmentVariable(key.Replace('.', '_'));
            return result;
        }

        public static T GetValue<T>(string key, T defaultValue = default)
        {
            string value = GetParameterValue(key);

            if (value != null)
            {
                return checkValue<T>(value, defaultValue);
            }
            return defaultValue;
        }

        private static T checkValue<T>(string input, T defaultValue)
        {
            if (isType<T>(input))
            {
                return (T)Convert.ChangeType(input, typeof(Task));
            }

            return defaultValue;
        }
        private static bool isType<T>(string input)
        {         
            try
            {
                var objectType = (T)Convert.ChangeType(input, typeof(T));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
