using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace INNO_F20_CC.TokenAnalyzer
{
    class TokenAnalyzer
    {
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
            ":=",
            ".",
            ",",
            ":",
            "[",
            "]",
            "(",
            ")"
        };
        static readonly ITokenReader[] readers =
        {
            new IdentifierReader(),
            new NumericReader(),
            new OperandReader(),
            new CommentBlockReader(),
            new CommentLineReader()
        };

        static IEnumerable<string> SplitSource(string source)
        {
            var reader = readers[0];
            var buffer = "";
            var line = 0;

            if (source[source.Length - 1] != '\n')
                source += '\n';

            for (int i = 0; i < source.Length - 1; i++)
            {
                char c = source[i];
                if (c == '\n')
                    line++;

                if (!reader.Read(source, ref i, ref buffer))
                {
                    if (buffer != string.Empty)
                    {
                        yield return buffer;
                        buffer = string.Empty;
                    }

                    reader = null;
                    foreach (var r in readers)
                        if (r.IsTrigger(source, ref i))
                        {
                            reader = r;
                            break;
                        }

                    if (reader == null || !Char.IsControl(c) || c != ' ')
                        Exit(line, i);
                }
            }
        }

        static Token[] ClassifyWords(IEnumerable<string> words)
        {
            var tokens = new List<Token>();
            var line = 1;
            var comment = false;
            foreach (var word in words)
            {
                if (word == "\n")
                {
                    line++;
                    continue;
                }
                if (comment)
                {
                    if (word == "*/")
                        comment = false;
                    continue;
                }
                if (word == "/*")
                {
                    comment = true;
                    continue;
                }

                var token = new Token()
                {
                    Value = word,
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
                // else
                //     Exit(word, line);

                tokens.Add(token);
            }
            return tokens.ToArray(); ;
        }

        public static Token[] Analyze(string source, bool serialize = true)
        {
            foreach (var w in SplitSource(source))
                Console.Write(w + "|");

            Console.WriteLine();
            Exit(1, 1);

            var Tokens = ClassifyWords(SplitSource(source));
            if (serialize)
            {
                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                var result = JsonSerializer.Serialize(Tokens, serializeOptions);
                File.WriteAllText("tokens.json", result);
            }
            return Tokens;
        }

        static void Exit(int line, int column)
        {
            Console.WriteLine($"Invalid token at {line}:{column}.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}