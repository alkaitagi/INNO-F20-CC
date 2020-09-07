using System.IO;

namespace INNO_F20_CC
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = File.ReadAllText(args[0]);
            var tokens = TokenAnalyzer.Analyze(source);
        }
    }
}
