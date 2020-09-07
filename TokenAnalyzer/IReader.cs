namespace INNO_F20_CC.TokenAnalyzer
{
    interface IReader
    {
        public bool IsTrigger(string source, ref int i);
        public bool Read(string source, ref int i, ref string token);
    }
}
