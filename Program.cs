using System.IO;

namespace INNO_F20_CC
{
    class Program
    {
        static void Main(string[] args)
        {
            TokenAnalyzer.Init(File.ReadAllText("source.txt"));
        }
    }
}
