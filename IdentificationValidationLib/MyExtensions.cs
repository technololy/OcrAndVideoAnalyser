using System;
using System.Linq;

namespace IdentificationValidationLib
{
    public static class MyExtensions
    {
        public static string ToReverseString(this string value)
        {
            return string.Join("", value.Reverse());
        }

        public static string SubstringReverse(this string value, int indexFromEnd, int length)
        {
            return value.Substring(Math.Max(0, length - indexFromEnd));
            return value.ToReverseString().Substring(indexFromEnd, length).ToReverseString();
        }
    }
}
