using System;
using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class Parser {
        public static TokenAnalyzer.Token[] TokensArray;
        public static void ExitWithError(int line){
            Console.WriteLine($"Syntax Error at line {line}.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
        public static bool NeededToken(int TokenNumber, string AttributeType, string ValueOrType, int LineOrColumn){
            if (TokenNumber >= TokensArray.Length){
                return false;
            }
            else {
                if (AttributeType.Equals("value")){
                    return TokensArray[TokenNumber].Value.Equals(ValueOrType);
                }
                else if (AttributeType.Equals("type")){
                    return TokensArray[TokenNumber].Type.Equals(ValueOrType);
                }
                else if (AttributeType.Equals("line")){
                    return (TokensArray[TokenNumber].Line == LineOrColumn);
                }
                else if (AttributeType.Equals("column")){
                    return (TokensArray[TokenNumber].Column == LineOrColumn);
                }
                return false;
            }
        }
        public static RootNode ParseProgram(List<TokenAnalyzer.Token> TokensList){
            TokensArray = TokensList.ToArray();
            int TokenNumber = 0;
            RootNode node = new RootNode();
            while(true){
                ClassDeclarationNode ClassDeclNode = ClassDeclarationParser.parse(TokenNumber);
                if (ClassDeclNode.BadParsed){
                    ExitWithError(ClassDeclNode.ErrorLine);
                }
                else {
                    node.AddClassDeclaration(ClassDeclNode);
                    TokenNumber = ClassDeclNode.TokenNumber;
                }
                if (TokenNumber >= TokensArray.Length){
                    break;
                }
            }
            return node;
        }
    }
}