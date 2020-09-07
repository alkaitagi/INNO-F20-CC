using System.Collections.Generic;

namespace INNO_F20_CC.TokenAnalyzer
{
    class IdentifierReader : IReader
    {
        static readonly HashSet<string> keywords = new HashSet<string>()
        {
            "class",
            "extends",
            "is",
            "end",
            "var",
            "method",
            "this",
            "while",
            "loop",
            "if",
            "else",
            "then",
            "return"
        };

        static bool IsLatinLetter(char c) =>
            c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z';

        public bool CanTransition(string source, ref int i)
        {
            var c = source[i];
            return c == '_' || IsLatinLetter(c);
        }

        public bool Read(string source, ref int i, ref string buffer)
        {
            var c = source[i];
            if (c == '_' || char.IsDigit(c) || IsLatinLetter(c))
            {
                buffer += c;
                i++;
                return true;
            }
            return false;
        }

        public string GetTokenType(string value) =>
            keywords.Contains(value) ? "keyword" : "name";
    }
}
