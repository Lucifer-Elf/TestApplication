﻿using System;

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
                return CheckValue<T>(value, defaultValue);
            }
            return defaultValue;
        }

        private static T CheckValue<T>(string input, T defaultValue)
        {
            if (IsType<T>(input))
            {
                return (T)Convert.ChangeType(input, typeof(T));
            }

            return defaultValue;
        }
        private static bool IsType<T>(string input)
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
