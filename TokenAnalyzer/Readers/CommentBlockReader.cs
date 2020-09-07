namespace INNO_F20_CC.TokenAnalyzer
{
    class CommentBlockReader : IReader
    {
        public bool IsTrigger(string source, ref int i)
        {
            if (source[i] == '/' && source[i + 1] == '*')
            {
                i += 2;
                return true;
            };
            return false;
        }

        public bool Read(string source, ref int i, ref string token)
        {
            if (source[i++] == '*' && source[i++] == '/')
                return false;
            return true;
        }
    }
}
