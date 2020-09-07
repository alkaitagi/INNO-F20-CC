namespace INNO_F20_CC.TokenAnalyzer
{
    class IdentifierReader : IReader
    {
        static bool IsLatinLetter(char c) =>
            c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z';

        public bool IsTrigger(string source, ref int i)
        {
            var c = source[i];
            return c == '_' || IsLatinLetter(c);
        }

        public bool Read(string source, ref int i, ref string token)
        {
            var c = source[i];
            if (c == '_' || char.IsDigit(c) || IsLatinLetter(c))
            {
                token += c;
                i++;
                return true;
            }
            return false;
        }
    }
}
