namespace INNO_F20_CC.TokenAnalyzer
{
    public class Token
    {
        public string Type { get; set; }
        public string Context { get; set; }
        public string Value { get; set; }
        public int Depth { get; set; }
        public int Line { get; set; }
    }
}
