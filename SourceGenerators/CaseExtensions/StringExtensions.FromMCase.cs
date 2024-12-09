using System;

namespace GodotUtilities.CaseExtensions
{
    public static partial class StringExtensions
    {
        /// <summary>
        /// Remove "m" from the variable to revert it back to normal.
        /// the 'm' naming convention is something I use for private fields in Nodes.
        /// for instance, mPlayerController, mTimer
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string FromMCase(this string source)
        {
            if (source != null)
            {
                if (source.StartsWith("m", StringComparison.OrdinalIgnoreCase)
                && source.Length > 1
                && char.IsUpper(source[1]))
                {
                    return source[1..];
                }
                else
                {
                    return source;
                }
            }

            throw new ArgumentNullException(nameof(source));
        }
    }
}
