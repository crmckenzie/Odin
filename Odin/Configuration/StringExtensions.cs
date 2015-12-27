using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Odin.Configuration
{
    public static class StringExtensions
    {
        public static string HyphenCase(this string input)
        {
            var result = Regex.Replace(input, ".[A-Z]", m => m.Value[0] + "-" + m.Value[1]).ToLower();
            return result;
        }

        public static string Join(this IEnumerable<string> input, string separator)
        {
            return string.Join(separator, input);
        }

        public static IEnumerable<string> Paginate(this string source, int columnWidth)
        {
            if (string.IsNullOrWhiteSpace(source))
                yield break;

            var paragraphs = source.Split(new[] { "\r\n", "\r", "\n"}, StringSplitOptions.RemoveEmptyEntries);

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