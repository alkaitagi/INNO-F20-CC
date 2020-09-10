using System.IO;
using System;

namespace INNO_F20_CC
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = File.ReadAllText(args[0]);
            var tokens = TokenAnalyzer.Manager.Analyze(source);
            var AST = SyntaxAnalyzer.Parser.ParseProgram(tokens);
        }
    }
}
