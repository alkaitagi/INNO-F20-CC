namespace INNO_F20_CC.TokenAnalyzer
{
    class NumericReader : IReader
    {
        public bool CanTransition(string source, ref int i) =>
            char.IsDigit(source[i]);

        public bool Read(string source, ref int i, ref string buffer)
        {
            var c = source[i];
            if (char.IsDigit(c))
            {
                buffer += c;
                i++;
                return true;
            }
            return false;
        }

        public string GetTokenType(string word) => "numeric";
    }
}
