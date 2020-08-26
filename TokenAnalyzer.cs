using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace INNO_F20_CC
{
    class TokenAnalyzer
    {
        struct Token
        {
            string id;
            string category;
            string value;
        }

        private static readonly string[] tokenNames =
        {
            "class",
            "extends",
            "var",
            "methods",
            "while",
            "loop",
            "if",
            "then",
            "else",
            "is",
            "end",
            "true",
            "false",
            "this",
            "return"
        };

        public static string[] Split(string source) =>
            Regex
                .Replace(source, @"\s|(:=|[.,:\(\)\[\]])", @" $1 ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        public static void Init(string source)
        {
            foreach (var s in Split(source)) Console.Write(s + " ");
        }
    }
}
