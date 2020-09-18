using System;
using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class ClassDeclarationParser {
        public static ClassDeclarationNode parse(int TokenNumber){
            ClassDeclarationNode node = new ClassDeclarationNode();
            if (!(Parser.NeededToken(TokenNumber, "value", "class", 0) && Parser.NeededToken(TokenNumber + 1, "type", "name", 0))){
                node.BadParsed = true;
                node.ErrorLine = Parser.TokensArray[Math.Min(TokenNumber, Parser.TokensArray.Length - 1)].Line;
                return node;
            }
            node.SetName(Parser.TokensArray[TokenNumber + 1].Value);
            TokenNumber += 2;
            if (Parser.NeededToken(TokenNumber, "value", "extends", 0) && Parser.NeededToken(TokenNumber + 1, "type", "name", 0)){
                node.SetSuperClass(Parser.TokensArray[TokenNumber + 1].Value);
                TokenNumber += 2;
            }
            if (!Parser.NeededToken(TokenNumber, "value", "is", 0)){
                node.BadParsed = true;
                node.ErrorLine = Parser.TokensArray[Math.Min(TokenNumber, Parser.TokensArray.Length - 1)].Line;
                return node;
            }
            TokenNumber++;
            while (true){
                if (Parser.NeededToken(TokenNumber, "value", "end", 0)){
                    TokenNumber++;
                    node.TokenNumber = TokenNumber;
                    return node;
                }
                VariableDeclarationNode VariableNode = VariableDeclarationParser.parse(TokenNumber);
                if (!VariableNode.BadParsed){
                    TokenNumber = VariableNode.TokenNumber;
                    node.AddVariableDeclaration(VariableNode);
                    continue;
                }
                MethodDeclarationNode MethodNode = MethodDeclarationParser.parse(TokenNumber);
                if (!MethodNode.BadParsed){
                    TokenNumber = MethodNode.TokenNumber;
                    node.AddMethodDeclaration(MethodNode);
                    continue;
                }
                ConstructorDeclarationNode ConstructorNode = ConstructorDeclarationParser.parse(TokenNumber);
                if (ConstructorNode.BadParsed){
                    node.BadParsed = true;
                    node.ErrorLine = Math.Max(VariableNode.ErrorLine, Math.Max(MethodNode.ErrorLine, ConstructorNode.ErrorLine));
                    return node;
                }
                TokenNumber = ConstructorNode.TokenNumber;
                node.AddConstructorDeclaraton(ConstructorNode);
                if (TokenNumber >= Parser.TokensArray.Length){
                    node.BadParsed = true;
                    node.ErrorLine = Parser.TokensArray[Parser.TokensArray.Length - 1].Line;
                    return node;
                }
            }
        }
    }
}