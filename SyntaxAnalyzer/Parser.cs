using System;
using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class Parser {
        
        private static int ExitWithError(int line){
            Console.WriteLine($"Syntax Error at {line}.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
            return 0;
        }
        public static TokenAnalyzer.Token[] TokensArray;
        public static int /* AST */ ParseProgram(List<TokenAnalyzer.Token> TokensList){
            TokensArray = TokensList.ToArray();
            int TokensCount = TokensArray.Length;
            int TokensParsed = 0;
            while(TokensParsed < TokensCount){
                TokensParsed = ParseClassDeclaration(TokensParsed, TokensCount);
            }
            return 0;
        }

        private static int ParseClassDeclaration(int TokensParsed, int TokensCount){
            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("class")){
                TokensParsed++;
                TokensParsed = ParseClassName(TokensParsed, TokensCount);
                if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("is")){
                    TokensParsed++;
                    if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("end")){
                        TokensParsed++;
                        return TokensParsed;
                    }
                    else {
                        while (true){
                            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("end")){
                                break;
                            }
                            TokensParsed = ParseMemberDeclaration(TokensParsed, TokensCount);
                        }
                        if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("end")){
                            TokensParsed++;
                            return TokensParsed;
                        }
                        else {
                            //Console.WriteLine(TokensParsed + " " + TokensArray[TokensParsed - 1].Value);
                            Console.WriteLine("ParseClassDeclaration 0");
                            return ExitWithError(TokensArray[TokensParsed - 1].Line);        
                        }
                    }
                }
                else if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("extends")){
                    TokensParsed++;
                    TokensParsed = ParseClassName(TokensParsed, TokensCount);
                    if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("is")){
                        TokensParsed++;
                        if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("end")){
                            TokensParsed++;
                            return TokensParsed;
                        }
                        else {
                            while (true){
                                if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("end")){
                                    break;
                                }
                                TokensParsed = ParseMemberDeclaration(TokensParsed, TokensCount);
                            }
                            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("end")){
                                TokensParsed++;
                                return TokensParsed;
                            }
                            else {
                                Console.WriteLine("ParseClassDeclaration 1");
                                return ExitWithError(TokensArray[TokensParsed - 1].Line);        
                            }
                        }
                    }
                    else {
                        Console.WriteLine("ParseClassDeclaration 2");
                        return ExitWithError(TokensArray[TokensParsed - 1].Line);
                    }    
                }
                else {
                    Console.WriteLine("ParseClassDeclaration 3");
                    return ExitWithError(TokensArray[TokensParsed - 1].Line);
                }
            }
            else {
                Console.WriteLine("ParseClassDeclaration 4");
                return ExitWithError(TokensArray[TokensParsed - 1].Line);
            }
        }

        private static int ParseClassName(int TokensParsed, int TokensCount){
            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Type.Equals("name")){
                TokensParsed++;
                if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("[")){
                    TokensParsed++;
                    TokensParsed = ParseClassName(TokensParsed, TokensCount);
                    if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("]")){
                        TokensParsed++;
                        return TokensParsed;
                    }
                    else {
                        Console.WriteLine("ParseClassName 0");
                        return ExitWithError(TokensArray[TokensParsed - 1].Line);
                    }
                }
                else {
                    return TokensParsed;
                }
            }
            else {
                Console.WriteLine("ParseClassName 1");
                return ExitWithError(TokensArray[TokensParsed - 1].Line);
            }
        }

        private static int ParseMemberDeclaration(int TokensParsed, int TokensCount){
            if (TokensArray[TokensParsed].Value.Equals("var")){
                TokensParsed = ParseVariableDeclaration(TokensParsed, TokensCount);
            }
            else if (TokensArray[TokensParsed].Value.Equals("method")){
                TokensParsed = ParseMethodDeclaration(TokensParsed, TokensCount);
            }
            else if (TokensArray[TokensParsed].Value.Equals("this")) {
                TokensParsed = ParseConstructorDeclaration(TokensParsed, TokensCount);
            }
            else {
                Console.WriteLine("ParseMemberDeclaration 0");
                return ExitWithError(TokensArray[TokensParsed - 1].Line);
            }
            return TokensParsed;
        }
    
        private static int ParseVariableDeclaration(int TokensParsed, int TokensCount){
            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("var")){
                TokensParsed++;
                if (TokensParsed < TokensCount && TokensArray[TokensParsed].Type.Equals("name")){
                    TokensParsed++;
                    if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals(":")){
                        TokensParsed++;
                        TokensParsed = ParseExpression(TokensParsed, TokensCount);
                        return TokensParsed;
                    }
                    else {
                        Console.WriteLine("ParseVariableDeclaration 0");
                        return ExitWithError(TokensArray[TokensParsed - 1].Line);
                    }
                }
                else {
                    Console.WriteLine("ParseVariableDeclaration 1");
                    return ExitWithError(TokensArray[TokensParsed - 1].Line);
                }
            }
            else {
                Console.WriteLine("ParseVariableDeclaration 2");
                return ExitWithError(TokensArray[TokensParsed - 1].Line);
            }
        }

        private static int ParseExpression(int TokensParsed, int TokensCount){
            TokensParsed = ParsePrimary(TokensParsed, TokensCount);
            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("(")){
                TokensParsed = ParseArguments(TokensParsed, TokensCount);
            }
            while (true){
                if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals(".")){
                    TokensParsed++;
                    if (TokensParsed < TokensCount && TokensArray[TokensParsed].Type.Equals("name")){
                        TokensParsed++;
                        if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("(")){
                            TokensParsed = ParseArguments(TokensParsed, TokensCount);
                            continue;
                        }
                        else if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals(".")){
                            continue;
                        }
                        else {
                            return ExitWithError(TokensArray[TokensParsed - 1].Line);
                        }
                    }
                    else {
                        return ExitWithError(TokensArray[TokensParsed - 1].Line);
                    }
                }
                else {
                    break;
                }
            }
            return TokensParsed;
        }

        private static int ParsePrimary(int TokensParsed, int TokensCount){
            if(TokensParsed < TokensCount &&
            (TokensArray[TokensParsed].Type.Equals("int") ||
            TokensArray[TokensParsed].Type.Equals("float") ||
            TokensArray[TokensParsed].Type.Equals("bool") ||
            TokensArray[TokensParsed].Value.Equals("this"))){
                TokensParsed++;
                return TokensParsed;
            }
            else {
                TokensParsed = ParseClassName(TokensParsed, TokensCount);
                return TokensParsed;
            }
        }

        private static int ParseArguments(int TokensParsed, int TokensCount){
            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("(")){
                TokensParsed++;
                TokensParsed = ParseExpression(TokensParsed, TokensCount);
                while(true){
                    if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals(")")){
                        TokensParsed++;
                        return TokensParsed;
                    }
                    if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals(",")){
                        TokensParsed++;
                        TokensParsed = ParseExpression(TokensParsed, TokensCount);
                    }
                } 
            }
            else {
                return ExitWithError(TokensArray[TokensParsed - 1].Line);
            }
        }

        private static int ParseMethodDeclaration(int TokensParsed, int TokensCount){
            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("method")){
                TokensParsed++;
                if (TokensParsed < TokensCount && TokensArray[TokensParsed].Type.Equals("name")){
                    TokensParsed++;
                    if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("is")){
                        TokensParsed++;
                    }
                    else if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals(":")){
                        TokensParsed++;
                        if (TokensParsed < TokensCount && TokensArray[TokensParsed].Type.Equals("name")){
                            TokensParsed++;
                            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("is")){
                                TokensParsed++;
                            }
                            else {
                                Console.WriteLine("ParseMethodDeclaration -2");
                                return ExitWithError(TokensArray[TokensParsed - 1].Line);
                            }
                        }
                        else {
                            Console.WriteLine("ParseMethodDeclaration -1");
                            return ExitWithError(TokensArray[TokensParsed - 1].Line);
                        }
                    }
                    else {
                        TokensParsed = ParseParameters(TokensParsed, TokensCount);
                        if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("is")){
                            TokensParsed++;
                        }
                        else {
                            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals(":")){
                                TokensParsed++;
                                if (TokensParsed < TokensCount && TokensArray[TokensParsed].Type.Equals("name")){
                                    TokensParsed++;
                                    if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("is")){
                                        TokensParsed++;
                                    }
                                    else {
                                        Console.WriteLine("ParseMethodDeclaration 0");
                                        return ExitWithError(TokensArray[TokensParsed - 1].Line);
                                    }
                                }
                                else {
                                    Console.WriteLine("ParseMethodDeclaration 1");
                                    return ExitWithError(TokensArray[TokensParsed - 1].Line);
                                }
                            }
                            else {
                                Console.WriteLine("ParseMethodDeclaration 2");
                                return ExitWithError(TokensArray[TokensParsed - 1].Line);
                            }
                        }
                    }
                    TokensParsed = ParseBody(TokensParsed, TokensCount);
                    if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("end")){
                        TokensParsed++;
                        return TokensParsed;
                    }
                    else {
                        Console.WriteLine("ParseMethodDeclaration 3");
                        return ExitWithError(TokensArray[TokensParsed - 1].Line);
                    }
                }
                else {
                    Console.WriteLine("ParseMethodDeclaration 4");
                    return ExitWithError(TokensArray[TokensParsed - 1].Line);
                }
            }
            else {
                Console.WriteLine("ParseMethodDeclaration 5");
                return ExitWithError(TokensArray[TokensParsed - 1].Line);
            }
        }

        private static int ParseParameters(int TokensParsed, int TokensCount){
            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("(")){
                TokensParsed++;
                TokensParsed = ParseParameterDeclaration(TokensParsed, TokensCount);
                while (true){
                    if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals(")")){
                        TokensParsed++;
                        break;
                    }
                    else {
                        if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals(",")){
                            TokensParsed++;
                            TokensParsed = ParseParameterDeclaration(TokensParsed, TokensCount);
                        }
                        else {
                            return ExitWithError(TokensArray[TokensParsed - 1].Line);
                        }
                    }
                }
                return TokensParsed;
            }
            else {
                return ExitWithError(TokensArray[TokensParsed - 1].Line);
            }
        }

        private static int ParseParameterDeclaration(int TokensParsed, int TokensCount){
            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Type.Equals("name")){
                TokensParsed++;
                if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals(":")){
                    TokensParsed++;
                    TokensParsed = ParseClassName(TokensParsed, TokensCount);
                    return TokensParsed;
                }
                else {
                    return ExitWithError(TokensArray[TokensParsed - 1].Line);
                }
            }
            else {
                return ExitWithError(TokensArray[TokensParsed - 1].Line);
            }
        }

        private static int ParseBody(int TokensParsed, int TokensCount){
            while (true){
                if (TokensParsed < TokensCount && 
                (TokensArray[TokensParsed].Value.Equals("else") || TokensArray[TokensParsed].Value.Equals("end"))){
                    return TokensParsed;
                }
                else if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("var")){
                    TokensParsed = ParseVariableDeclaration(TokensParsed, TokensCount);
                }
                else {
                    TokensParsed = ParseStatement(TokensParsed, TokensCount);
                }
            }
        }

        private static int ParseStatement(int TokensParsed, int TokensCount){
            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("if")){
                TokensParsed = ParseIfStatement(TokensParsed, TokensCount);
            }
            else if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("while")){
                TokensParsed = ParseWhileLoop(TokensParsed, TokensCount);
            }
            else if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("return")){
                TokensParsed = ParseReturnStatement(TokensParsed, TokensCount);
            }
            else {
                TokensParsed = ParseAssignment(TokensParsed, TokensCount);
            }
            return TokensParsed;
        }

        private static int ParseIfStatement(int TokensParsed, int TokensCount){
            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("if")){
                TokensParsed++;
                TokensParsed = ParseExpression(TokensParsed, TokensCount);
                if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("then")){
                    TokensParsed++;
                    TokensParsed = ParseBody(TokensParsed, TokensCount);
                    if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("end")){
                        TokensParsed++;
                        return TokensParsed;
                    }
                    else if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("else")) {
                        TokensParsed++;
                        TokensParsed = ParseBody(TokensParsed, TokensCount);
                        if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("end")){
                            TokensParsed++;
                            return TokensParsed;
                        }
                        else {
                            return ExitWithError(TokensArray[TokensParsed - 1].Line);
                        }
                    }
                    else {
                        return ExitWithError(TokensArray[TokensParsed - 1].Line);
                    }
                }
                else {
                    return ExitWithError(TokensArray[TokensParsed - 1].Line);
                }
            }
            else {
                return ExitWithError(TokensArray[TokensParsed - 1].Line);
            }
        }

        private static int ParseWhileLoop(int TokensParsed, int TokensCount){
            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("while")){
                TokensParsed++;
                TokensParsed = ParseExpression(TokensParsed, TokensCount);
                if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("loop")){
                    TokensParsed++;
                    TokensParsed = ParseBody(TokensParsed, TokensCount);
                    if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("end")){
                        TokensParsed++;
                        return TokensParsed;
                    }
                    else {
                        return ExitWithError(TokensArray[TokensParsed - 1].Line);
                    }
                }
                else {
                    return ExitWithError(TokensArray[TokensParsed - 1].Line);
                }
            }
            else {
                return ExitWithError(TokensArray[TokensParsed - 1].Line);
            }
        }

        private static int ParseReturnStatement(int TokensParsed, int TokensCount){
            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("return")){
                TokensParsed++;
                if (TokensParsed < TokensCount && TokensArray[TokensParsed].Line == TokensArray[TokensParsed - 1].Line){
                    TokensParsed = ParseExpression(TokensParsed, TokensCount);
                }
                return TokensParsed;
            }
            else {
                return ExitWithError(TokensArray[TokensParsed - 1].Line);
            }
        }

        private static int ParseAssignment(int TokensParsed, int TokensCount){
            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Type.Equals("name")){
                TokensParsed++;
                if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals(":=")){
                    TokensParsed++;
                    TokensParsed = ParseExpression(TokensParsed, TokensCount);
                    return TokensParsed;
                }
                else {
                    return ExitWithError(TokensArray[TokensParsed - 1].Line);
                }
            }
            else {
                return ExitWithError(TokensArray[TokensParsed - 1].Line);
            }
        }

        private static int ParseConstructorDeclaration(int TokensParsed, int TokensCount){
            if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("this")){
                TokensParsed++;
                if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("is")){
                    TokensParsed++;
                    TokensParsed = ParseBody(TokensParsed, TokensCount);
                    if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("end")){
                        TokensParsed++;
                        return TokensParsed;
                    }
                    else {
                        return ExitWithError(TokensArray[TokensParsed - 1].Line);
                    }
                }
                else {
                    TokensParsed = ParseParameters(TokensParsed, TokensCount);
                    if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("is")){
                        TokensParsed++;
                        TokensParsed = ParseBody(TokensParsed, TokensCount);
                        if (TokensParsed < TokensCount && TokensArray[TokensParsed].Value.Equals("end")){
                            TokensParsed++;
                            return TokensParsed;
                        }
                        else {
                            return ExitWithError(TokensArray[TokensParsed - 1].Line);
                        }
                    }
                    else {
                        return ExitWithError(TokensArray[TokensParsed - 1].Line);
                    }   
                }
            }
            else {
                return ExitWithError(TokensArray[TokensParsed - 1].Line);
            }
        }
    }
}