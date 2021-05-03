using System;

namespace Avalier.Busk
{
    internal static class LocalExtensions
    {
        public static T ThrowIfNull<T>(this T value, string paramName)
            where T : class
        {
            if (null == value)
            {
                throw new ArgumentNullException(paramName);
            }
            return value;
        }
        
        public static string ThrowIfNullOrEmpty(this string value, string paramName)
        {
            if (null == value)
            {
                throw new ArgumentNullException(paramName);
            }
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Argument was an empty string", paramName);
            }
            return value;
        }
    }
}