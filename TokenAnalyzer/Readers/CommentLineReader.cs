namespace INNO_F20_CC.TokenAnalyzer
{
    class CommentLineReader : ITokenReader
    {
        public bool IsTrigger(string source, ref int i) =>
            source[i] == '/' && source[i + 1] == '/';

        public bool Read(string source, ref int i, ref string token) =>
            source[i] != '\n';
    }
}
