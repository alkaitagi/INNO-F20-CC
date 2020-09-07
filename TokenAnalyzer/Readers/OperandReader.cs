namespace INNO_F20_CC.TokenAnalyzer
{
    class OperandReader : ITokenReader
    {
        static readonly char[] operands = { '.', ',', ':', '=', '[', ']', '(', ')' };

        static bool IsOperand(char c)
        {
            foreach (var o in operands)
                if (o == c)
                    return true;
            return false;
        }

        public bool IsTrigger(string source, ref int i) =>
            IsOperand(source[i]);

        public bool Read(string source, ref int i, ref string token)
        {
            var c = source[i];
            if (c == ':' && source[i + 1] == '=')
            {
                i += 2;
                token = ":=";
            }
            if (IsOperand(c))
                token += c;
            return false;
        }
    }
}
