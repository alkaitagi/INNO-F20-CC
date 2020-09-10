namespace INNO_F20_CC.TokenAnalyzer
{
    class NumericReader : IReader
    {
        int points;
        public bool HasError() => points > 1;
        public void Reset() => points = 0;

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
            else if (c == '.')
            {
                buffer += c;
                points++;

                if (HasError())
                    return false;

                i++;
                return true;
            }
            return false;
        }

        public string GetTokenType(string word) =>
            points == 0 ? "int" : "float";
    }
}
