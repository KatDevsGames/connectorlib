/*
 * Copyright 2018 Equilateral IT
 *
 * ConnectorLib is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * ConnectorLib is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with ConnectorLib.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Text;
using JetBrains.Annotations;

namespace ConnectorLibDemo
{
    /// <summary>
    /// Extension methods for String objects.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string.
        /// </summary>
        /// <param name="str">The source string.</param>
        /// <param name="oldValue">The string to be replaced.</param>
        /// <param name="newValue">The string to replace all occurrences of oldValue.</param>
        /// <param name="comparison">The StringComparison used to determine matches of oldValue.</param>
        /// <returns>A string that is equivalent to the current string except that all instances of oldValue are replaced with newValue. If oldValue is not found in the current instance, the method returns the current instance unchanged.</returns>
        [NotNull]
        public static string Replace([CanBeNull] this string str, [CanBeNull] string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            if (str == null) { return string.Empty; }
            if (oldValue == null) { return str; }
            if (newValue == null) { newValue = string.Empty; }

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }

        /// <summary>
        /// Performs case-insensitive comparison using the invariant culture.
        /// </summary>
        /// <param name="s1">The first string to compare.</param>
        /// <param name="s2">The second string to compare.</param>
        /// <returns>True if the strings are equal using a case-insensitive comparison in the invariant culture, false otherwise.</returns>
        public static bool EqualsInvariantIgnoreCase([NotNull] this string s1, [CanBeNull] string s2)
            => s1.Equals(s2, StringComparison.InvariantCultureIgnoreCase);
    }
}