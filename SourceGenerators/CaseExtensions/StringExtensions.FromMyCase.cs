using System;

namespace GodotUtilities.CaseExtensions
{
    public static partial class StringExtensions
    {
        /// <summary>
        /// Remove "My" from the variable to revert it back to normal.
        /// the 'My' naming convention is something I use for properties in Nodes.
        /// for instance, MyPlayerController, MyProperty
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string FromMyCase(this string source)
        {
            if (source != null)
            {
                if (source.StartsWith("My", StringComparison.OrdinalIgnoreCase)
                && source.Length > 2
                && char.IsUpper(source[2]))
                {
                    return ToCamelCase(source[2..]);
                }
                else
                {
                    return ToCamelCase(source);
                }
            }

            throw new ArgumentNullException(nameof(source));

        }
    }
}
