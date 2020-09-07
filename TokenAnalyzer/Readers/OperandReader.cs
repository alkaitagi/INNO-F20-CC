namespace INNO_F20_CC.TokenAnalyzer
{
    class OperandReader : IReader
    {
        static readonly char[] operands = { '.', ',', ':', '=', '[', ']', '(', ')' };

        static bool IsOperand(char c)
        {
            foreach (var o in operands)
                if (o == c)
                    return true;
            return false;
        }

        public bool CanTransition(string source, ref int i) =>
            IsOperand(source[i]);

        public bool Read(string source, ref int i, ref string buffer)
        {
            var c = source[i];
            if (c == ':' && source[i + 1] == '=')
            {
                buffer = ":=";
                i += 2;
            }
            else if (IsOperand(c))
            {
                buffer += c;
                i++;
            }
            return false;
        }

        public string GetTokenType(string word) => "operand";
    }
}
