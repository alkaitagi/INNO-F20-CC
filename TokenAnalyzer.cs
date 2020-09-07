using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace INNO_F20_CC
{
    class TokenAnalyzer
    {
        struct Token
        {
            public string Type { get; set; }
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
        };
        static readonly HashSet<string> delimiters = new HashSet<string>()
        {
            ".",
            ",",
            ":",
            ":=",
            "[",
            "]",
            "(",
            ")"
        };

        static string[] SplitSource(string source){
            int state = 0;
            bool tokenReady = false;
            List<String> tokenList = new List<string>();
            string currentToken = "";
            for (int i = 0; i < source.Length;){
                if (state == 0){
                    if (tokenReady == true){
                        tokenList.Add(currentToken);
                        tokenReady = false;
                        currentToken = "";
                    }
                    if (source[i] == '\n'){
                        tokenList.Add("\n");
                    }
                    else if ((int)source[i] == 13 && source[i + 1] == '\n'){
                        tokenList.Add("\n");
                        i++;
                    }
                    else if (Char.IsDigit(source[i])){
                        currentToken += source[i];
                        state = 4;
                    }
                    else if (Char.IsLetter(source[i]) || source[i] == '_'){
                        currentToken += source[i];
                        state = 1;
                    }
                    else if (source[i] == '(' || source[i] == ')' || source[i] == '[' || source[i] == ']'){
                        currentToken += source[i];
                        tokenReady = true;
                    }
                    else if (source[i] != ' ' && (int)source[i] != 9) {
                        currentToken += source[i];
                        state = 3;
                    }
                    i++;
                }
                else if (state == 1){
                    if (Char.IsLetter(source[i]) || Char.IsDigit(source[i]) || source[i] == '_'){
                        currentToken += source[i];
                        i++;
                    }
                    else {
                        state = 0;
                        tokenReady = true;
                    }
                }
                /*else if (state == 2){
                    if (source[i] == '(' || source[i] == ')' || source[i] == '[' || source[i] == ']'){
                        state = 0;
                        currentToken += source[i];
                        i++;
                    }
                    else {
                        state = 0;
                        tokenReady = true;
                    }
                }*/
                else if (state == 3){
                    if (source[i] != '(' && source[i] != ')' && source[i] != '[' && source[i] != ']' &&
                        source[i] != '\n' && source[i] != ' ' && !Char.IsLetter(source[i]) && !Char.IsDigit(source[i]) && source[i] != '_'){
                        currentToken += source[i];
                        i++;
                    }
                    else {
                        state = 0;
                        tokenReady = true;
                    }
                }
                else {
                    if (Char.IsDigit(source[i]) || Char.IsLetter(source[i]) || source[i] == '_' || source[i] == '.'){
                        currentToken += source[i];
                        i++;
                    }
                    else {
                        state = 0;
                        tokenReady = true;
                    }
                }
            }
            return tokenList.ToArray();
        }
        static Token[] ClassifyWords(string[] words)
        {
            var tokens = new List<Token>();
            var depth = 0;
            var line = 1;
            
            foreach (var word in words)
            {
                /*for (int i = 0; i < word.Length; ++i){
                    Console.WriteLine((int)word[i]);
                } */     
                if (word == "\n")
                {
                    line++;
                    continue;
                }
                var token = new Token()
                {
                    Value = word,
                    Depth = depth,
                    Line = line
                };

                //Console.WriteLine(word);        
                if (keywords.Contains(word))
                    token.Type = "keyword";
                else if (delimiters.Contains(word))
                    token.Type = "delimiter";
                else if (bool.TryParse(word, out _))
                    token.Type = "bool";
                else if (int.TryParse(word, out _))
                    token.Type = "int";
                else {    
                    bool correctToken = true;
                    bool dot = false;
                    for (int j = 0; j < word.Length; ++j){
                        if (dot == false){
                            if ((j == 0 || j == word.Length - 1) && word[j] == '.'){
                                correctToken = false;
                            }
                            else if (word[j] != '.' && !Char.IsDigit(word[j])){
                                correctToken = false;
                            }
                            else if (word[j] == '.'){
                                dot = true;
                            }
                        }
                        else {
                            if (!Char.IsDigit(word[j])){
                                correctToken = false;
                            }
                        }
                    }
                    if (correctToken){
                        token.Type = "real";
                    }
                    else if (Regex.IsMatch(word, @"^[A-Za-z_]+\w*$")){
                        token.Type = "identifier";
                    }
                    else {
                        Exit(word, line);
                    }
                }

                if (word == "loop" || word == "then" || word == "is")
                    depth++;
                else if (word == "end")
                    depth--;

                tokens.Add(token);
            }
            return tokens.ToArray();
        }

        public static void Analyze(string filename)
        {
            var source = File.ReadAllText(filename);
            var Tokens = ClassifyWords(SplitSource(source + "\r\n"));
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
