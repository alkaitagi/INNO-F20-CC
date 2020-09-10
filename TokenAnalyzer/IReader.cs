namespace INNO_F20_CC.TokenAnalyzer
{
    interface IReader
    {
        public bool HasError() => false;
        public void Reset() { }
        public bool CanTransition(string source, ref int i);
        public bool Read(string source, ref int i, ref string buffer);
        public string GetTokenType(string word) => null;
    }
}
