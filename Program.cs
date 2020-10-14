using System.IO;

using System;
using INNO_F20_CC.SemanticsAnalyzer;

namespace INNO_F20_CC
{
    class Program
    {
        static void Main(string[] args)
        {
            //var source = File.ReadAllText(args[0]);
            var source = @"class Test13 is
    var catalog: Song
    method Listen(id: Integer) is
      catalog.setStreams(catalog.streams.Plus(1))
    end
    
    method Add(id: Integer, duration: Integer) is
      catalog := Song(id, duration)
    end
end    
    
class Song is
  var id: Integer(0)
  var duration: Integer(0)
  var streams: Integer(0)
  this(arg1: Integer, arg2: Integer) is
    id := arg1
    duration := arg2
  end
  method setStreams(n : Integer) is
    streams := n
  end
end
";

            //var source = File.ReadAllText("Tests/13");
            var tokens = TokenAnalyzer.Manager.Analyze(source);

            var AST = SyntaxAnalyzer.Parser.ParseProgram(tokens);
            
            SemanticAnalyzer.SemanticAnalysis(ref AST, ref tokens);
            Console.WriteLine("всё!");
            
            Console.Read();

        }
    }
}
