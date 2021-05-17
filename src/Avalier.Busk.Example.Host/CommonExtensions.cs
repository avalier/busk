using System;

namespace Avalier.Busk.Example.Host
{
    public static class CommonExtensions
    {
        public static T ThrowIfNull<T>(this T value, string paramName)
        {
            if (null == value)
            {
                throw new ArgumentNullException(paramName);
            }

            return value;
        }
    }
}
