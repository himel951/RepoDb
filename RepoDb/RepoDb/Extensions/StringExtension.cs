﻿using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="String"/>.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Joins an array string with a given separator.
        /// </summary>
        /// <param name="strings">The enumerable list of strings.</param>
        /// <param name="separator">The separator to be used.</param>
        /// <returns>A joined string from a given array of strings separated by the defined separator.</returns>
        public static string Join(this IEnumerable<string> strings,
            string separator)
        {
            return string.Join(separator, strings);
        }

        /// <summary>
        /// Removes the non-alphanumeric characters.
        /// </summary>
        /// <param name="value">The string value where the non-alphanumeric characters will be removed.</param>
        /// <param name="trim">The boolean value that indicates whether to trim the string before removing the non-alphanumeric characters.</param>
        /// <returns>The alphanumeric string.</returns>
        public static string AsAlphaNumeric(this string value,
            bool trim)
        {
            if (trim)
            {
                value = value.Trim();
            }
            return Regex.Replace(value, @"[^a-zA-Z0-9]", "_");
        }

        /// <summary>
        /// Unquotes a string.
        /// </summary>
        /// <param name="value">The string value to be unqouted.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>The unquoted string.</returns>
        public static string AsUnquoted(this string value,
            IDbSetting dbSetting)
        {
            if (dbSetting == null)
            {
                return value;
            }
            return value
                .Replace(dbSetting.OpeningQuote, string.Empty)
                .Replace(dbSetting.ClosingQuote, string.Empty);
        }

        /// <summary>
        /// Unquotes a string.
        /// </summary>
        /// <param name="value">The string value to be unqouted.</param>
        /// <param name="trim">The boolean value that indicates whether to trim the string before unquoting.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>The unquoted string.</returns>
        public static string AsUnquoted(this string value,
            bool trim,
            IDbSetting dbSetting)
        {
            if (trim)
            {
                value = value.Trim();
            }
            if (dbSetting == null)
            {
                return value;
            }
            if (string.IsNullOrEmpty(dbSetting?.SchemaSeparator) || value.IndexOf(dbSetting?.SchemaSeparator) < 0)
            {
                return value.AsUnquoted(dbSetting);
            }
            else
            {
                var splitted = value.Split(dbSetting?.SchemaSeparator.ToCharArray());
                return splitted.Select(s => s.AsUnquoted(dbSetting)).Join(dbSetting?.SchemaSeparator);
            }
        }

        /// <summary>
        /// Quotes a string.
        /// </summary>
        /// <param name="value">The string value to be quoted.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>The quoted string.</returns>
        public static string AsQuoted(this string value,
            IDbSetting dbSetting)
        {
            if (dbSetting == null)
            {
                return value;
            }
            if (!value.StartsWith(dbSetting.OpeningQuote))
            {
                value = string.Concat(dbSetting.OpeningQuote, value);
            }
            if (!value.EndsWith(dbSetting.ClosingQuote))
            {
                value = string.Concat(value, dbSetting.ClosingQuote);
            }
            return value;
        }

        /// <summary>
        /// Quotes a string.
        /// </summary>
        /// <param name="value">The string value to be quoted.</param>
        /// <param name="trim">The boolean value that indicates whether to trim the string before quoting.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>The quoted string.</returns>
        public static string AsQuoted(this string value,
            bool trim,
            IDbSetting dbSetting)
        {
            return AsQuoted(value, trim, false, dbSetting);
        }

        /// <summary>
        /// Quotes a string.
        /// </summary>
        /// <param name="value">The string value to be quoted.</param>
        /// <param name="trim">The boolean value that indicates whether to trim the string before quoting.</param>
        /// <param name="ignoreSchema">The boolean value that indicates whether to ignore the schema.</param>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>The quoted string.</returns>
        public static string AsQuoted(this string value,
            bool trim,
            bool ignoreSchema,
            IDbSetting dbSetting)
        {
            if (trim)
            {
                value = value.Trim();
            }
            if (dbSetting == null)
            {
                return value;
            }
            if (ignoreSchema || value.IndexOf(dbSetting.SchemaSeparator) < 0)
            {
                return value.AsQuoted(dbSetting);
            }
            else
            {
                var splitted = value.Split(dbSetting?.SchemaSeparator.ToCharArray());
                return splitted.Select(s => s.AsQuoted(dbSetting)).Join(dbSetting.SchemaSeparator);
            }
        }

        // AsEnumerable
        internal static IEnumerable<string> AsEnumerable(this string value)
        {
            yield return value;
        }

        // AsJoinQualifier
        internal static string AsJoinQualifier(this string value,
            string leftAlias,
            string rightAlias,
            IDbSetting dbSetting)
        {
            return string.Concat(leftAlias, dbSetting.SchemaSeparator, value.AsQuoted(dbSetting), " = ", rightAlias, dbSetting.SchemaSeparator, value.AsQuoted(dbSetting));
        }

        // AsField
        internal static string AsField(this string value,
            IDbSetting dbSetting)
        {
            return value.AsQuoted(dbSetting);
        }

        // AsParameter
        internal static string AsParameter(this string value,
            IDbSetting dbSetting)
        {
            return AsParameter(value, 0, dbSetting);
        }

        // AsParameter
        internal static string AsParameter(this string value,
            int index,
            IDbSetting dbSetting)
        {
            return index > 0 ? string.Concat(dbSetting.ParameterPrefix, value.AsUnquoted(dbSetting), "_", index) :
                string.Concat(dbSetting.ParameterPrefix, value.AsUnquoted(dbSetting));
        }

        // AsAliasField
        internal static string AsAliasField(this string value,
            string alias,
            IDbSetting dbSetting)
        {
            return string.Concat(alias, dbSetting.SchemaSeparator, value.AsQuoted(dbSetting));
        }

        // AsParameterAsField
        internal static string AsParameterAsField(this string value,
            int index,
            IDbSetting dbSetting)
        {
            return string.Concat(AsParameter(value, index, dbSetting), " AS ", AsField(value, dbSetting));
        }

        // AsFieldAndParameter
        internal static string AsFieldAndParameter(this string value,
            int index,
            IDbSetting dbSetting)
        {
            return string.Concat(AsField(value, dbSetting), " = ", AsParameter(value, index, dbSetting));
        }

        // AsFieldAndAliasField
        internal static string AsFieldAndAliasField(this string value,
            string alias,
            IDbSetting dbSetting)
        {
            return string.Concat(AsField(value, dbSetting), " = ", alias, dbSetting.SchemaSeparator, AsField(value, dbSetting));
        }

        /* IEnumerable<string> */

        // AsFields
        internal static IEnumerable<string> AsFields(this IEnumerable<string> values,
            IDbSetting dbSetting)
        {
            return values?.Select(value => value.AsField(dbSetting));
        }

        // AsParameters
        internal static IEnumerable<string> AsParameters(this IEnumerable<string> values,
            int index,
            IDbSetting dbSetting)
        {
            return values?.Select(value => value.AsParameter(index, dbSetting));
        }

        // AsAliasFields
        internal static IEnumerable<string> AsAliasFields(this IEnumerable<string> values,
            string alias,
            IDbSetting dbSetting)
        {
            return values?.Select(value => value.AsAliasField(alias, dbSetting));
        }

        // AsParametersAsFields
        internal static IEnumerable<string> AsParametersAsFields(this IEnumerable<string> values,
            int index,
            IDbSetting dbSetting)
        {
            return values?.Select(value => value.AsParameterAsField(index, dbSetting));
        }

        // AsFieldsAndParameters
        internal static IEnumerable<string> AsFieldsAndParameters(this IEnumerable<string> values,
            int index,
            IDbSetting dbSetting)
        {
            return values?.Select(value => value.AsFieldAndParameter(index, dbSetting));
        }

        // AsFieldsAndAliasFields
        internal static IEnumerable<string> AsFieldsAndAliasFields(this IEnumerable<string> values,
            string alias,
            IDbSetting dbSetting)
        {
            return values?.Select(value => value.AsFieldAndAliasField(alias, dbSetting));
        }
    }
}
