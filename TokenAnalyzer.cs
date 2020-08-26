using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace INNO_F20_CC
{
    class TokenAnalyzer
    {
        class Token
        {
            public string Type { get; set; }
            public string Context { get; set; }
            public string Value { get; set; }
            public int Depth { get; set; }
            public int Line { get; set; }
        }

        static readonly HashSet<string> keywords = new HashSet<string>()
        {
            "class",
            "extends",
            "is",
            "end",
            "var",
            "method",
            "this",
            "while",
            "loop",
            "if",
            "else",
            "then",
            "return",
            ".",
            ",",
            ":",
            ":=",
            "[",
            "]",
            "(",
            ")"
        };

        static string[] SplitSource(string source) =>
           Regex
               .Replace(source, @"(\n)|\s|//.*|(\d+\.\d+|:=|[\.,:\(\)\[\]])", @" $1 ")
               .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        static Token[] ClassifyWords(string[] words)
        {
            var tokens = new List<Token>();
            var depth = 0;
            var line = 1;
            var inComment = false;
            foreach (var word in words)
            {
                if (word == "\n")
                {
                    line++;
                    continue;
                }
                if (inComment)
                {
                    if (word == "*/")
                        inComment = false;
                    continue;
                }
                if (word == "/*")
                {
                    inComment = true;
                    continue;
                }

                var token = new Token()
                {
                    Value = word,
                    Depth = depth,
                    Line = line
                };

                if (keywords.Contains(word))
                    token.Type = "keyword";
                else if (bool.TryParse(word, out _))
                    token.Type = "bool";
                else if (int.TryParse(word, out _))
                    token.Type = "int";
                else if (float.TryParse(word, out _))
                    token.Type = "real";
                else if (Regex.IsMatch(word, @"^[A-Za-z_]+\w*$"))
                    token.Type = "name";
                else
                    Exit(word, line);

                if (word == "loop" || word == "then" || word == "is")
                    depth++;
                else if (word == "end")
                    depth--;

                tokens.Add(token);
            }
            return tokens.ToArray(); ;
        }

        public static void Analyze(string filename)
        {
            var source = File.ReadAllText(filename);
            var Tokens = ClassifyWords(SplitSource(source));
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var result = JsonSerializer.Serialize(Tokens, serializeOptions);
            File.WriteAllText("tokens.json", result);
        }

        static void Exit(string word, int line)
        {
            Console.WriteLine($"Error: unknown token {word} at line {line}.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
