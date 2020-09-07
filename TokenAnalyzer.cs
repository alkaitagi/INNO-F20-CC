using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace INNO_F20_CC
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

        // Regex
        //        .Replace(source, @"(\n)|\s|//.*|(\d+\.\d+|:=|[\.,:\(\)\[\]])", @" $1 ")
        //        .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        static IEnumerable<string> SplitSource(string source)
        {
            static void SaveBuffer(ref string buffer, List<string> words)
            {
                if (buffer != "")
                    words.Add(buffer);
                buffer = "";
            }

            Func<char, bool> isBreak = c => Char.IsControl(c) || c == ' ';

            var words = new List<string>();
            var line = 0;
            var buffer = "";
            var state = "start";

            if (source[source.Length - 1] != '\n')
                source += '\n';
            var sl = source.Length - 1;

            for (int i = 0; i < sl; i++)
            {
                var c = source[i];
                var cn = source[i + 1];
                if (c == '\n')
                    line++;

                if (state == "start")
                {
                    if (Char.IsControl(c) || c == ' ')
                        continue;
                    if (Char.IsNumber(c))
                    {
                        buffer += c;
                        state = "int";
                    }
                    else if (c == ':' && cn == '=')
                    {
                        words.Add(":=");
                        i++;
                    }
                    else if (keywords.Contains(c.ToString()))
                        words.Add(c.ToString());
                    else if (c == '/' && cn == '*')
                    {
                        state = "commentBlock";
                        i++;
                    }
                    else if (c == '/' && cn == '/')
                    {
                        state = "comment";
                        i++;
                    }
                    else if (c == '_' || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z')
                    {
                        buffer += c;
                        state = "name";
                    }
                }
                else if (state == "name")
                {
                    if (c == '_' || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || Char.IsDigit(c))
                        buffer += c;
                    else
                    {
                        SaveBuffer(ref buffer, words);
                        i--;
                        state = "start";
                    }

                }
                else if (state == "int" || state == "float")
                {
                    if (Char.IsNumber(c))
                        buffer += c;
                    else if (c == '.')
                    {
                        if (state == "int")
                        {
                            buffer += c;
                            state = "float";
                        }
                        else
                            Exit(line, i, "a digit");
                    }
                    else
                    {
                        SaveBuffer(ref buffer, words);
                        i--;
                        state = "start";
                    }
                }
                else if (state == "comment")
                {
                    if (c == '\n')
                        state = "start";
                }
                else if (state == "commentBlock")
                {
                    if (c == '*' && cn == '/')
                    {
                        state = "start";
                        i++;
                    }
                }
            }

            return words;
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
            Exit(1, 1, "");
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

        static void Exit(int line, int column, string expected)
        {
            Console.WriteLine($"Error at {line}:{column}. Expected {expected}.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
