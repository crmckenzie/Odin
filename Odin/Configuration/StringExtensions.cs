using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Odin.Configuration
{
    /// <summary>
    /// Extension methods for <see cref="string"/>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns the hyphen-cased form of a string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>
        /// E.g., "FredBob" becomes "fred-bob" 
        /// </returns>
        public static string KebabCase(this string input)
        {
            var result = Regex.Replace(input, ".[A-Z]", m => m.Value[0] + "-" + m.Value[1]).ToLower();
            return result;
        }

        /// <summary>
        /// Extension method wrapper around string.Join
        /// </summary>
        /// <param name="input"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> input, string separator)
        {
            return string.Join(separator, input);
        }

        /// <summary>
        /// Paginates text over multiple lines for a given columnWidth.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="columnWidth"></param>
        /// <returns></returns>
        public static IEnumerable<string> Paginate(this string source, int columnWidth)
        {
            if (string.IsNullOrWhiteSpace(source))
                yield break;

            var paragraphs = source.Split(new[] { "\r\n", "\r", "\n"}, StringSplitOptions.None);

            foreach (var paragraph in paragraphs)
            {
                var words = paragraph.Split(' ');

                for (var j = 0; j < words.Length; j++)
                {
                    var word = words[j];
                    var line = word;

                    var phraseLength = word.Length;
                    for (var k = j + 1; k < words.Length; k++)
                    {
                        var nextWord = words[k];
                        phraseLength++;
                        phraseLength += nextWord.Length;
                        if (phraseLength > columnWidth) break;

                        line += " " + nextWord;
                        j++;
                    }

                    yield return line;
                }
            }
        }
    }
}