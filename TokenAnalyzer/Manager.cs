using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace INNO_F20_CC.TokenAnalyzer
{
    public static class Manager
    {
        static readonly IReader[] readers =
        {
            new IdentifierReader(),
            new NumericReader(),
            new OperandReader(),
            new CommentBlockReader(),
            new CommentLineReader()
        };

        static List<Token> GetTokens(string source)
        {
            var tokens = new List<Token>();
            var reader = readers[0];
            var buffer = "";
            var line = 0;

            if (source[source.Length - 1] != '\n')
                source += '\n';

            for (int i = 0; i < source.Length - 1;)
            {
                if (source[i] == '\n')
                    line++;

                if (reader == null || !reader.Read(source, ref i, ref buffer))
                {
                    if (buffer != string.Empty)
                    {
                        if (reader.GetTokenType(buffer) is String type)
                            tokens.Add(new Token(buffer, type, line, i));
                        buffer = string.Empty;
                    }
                    reader = null;

                    if (source[i] == ' ' || Char.IsControl(source[i]))
                        i++;
                    else
                    {
                        foreach (var r in readers)
                            if (r.CanTransition(source, ref i))
                            {
                                reader = r;
                                break;
                            }
                        if (reader == null)
                            Error(line, i);
                    }
                }
            }
            return tokens;
        }

        public static List<Token> Analyze(string source, bool serialize = true)
        {
            var tokens = GetTokens(source);
            if (serialize)
            {
                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                var result = JsonSerializer.Serialize(tokens, serializeOptions);
                File.WriteAllText("tokens.json", result);
            }
            return tokens;
        }

        static void Error(int line, int column)
        {
            Console.WriteLine($"Invalid token at {line}:{column}.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
