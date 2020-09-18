using System;
using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class ConstructorDeclarationParser {
        public static ConstructorDeclarationNode parse(int TokenNumber){
            ConstructorDeclarationNode node = new ConstructorDeclarationNode();
            if (!Parser.NeededToken(TokenNumber, "value", "this", 0)){
                node.BadParsed = true;
                node.ErrorLine = Parser.TokensArray[Math.Min(TokenNumber, Parser.TokensArray.Length - 1)].Line;
                return node;
            }
            TokenNumber++;
            if (Parser.NeededToken(TokenNumber, "value", "(", 0)){
                TokenNumber++;
                while (true){
                    if (!(Parser.NeededToken(TokenNumber, "type", "name", 0) && 
                    Parser.NeededToken(TokenNumber + 1, "value", ":", 0) && 
                    Parser.NeededToken(TokenNumber + 2, "type", "name", 0))){
                        node.BadParsed = true;
                        node.ErrorLine = Parser.TokensArray[Math.Min(TokenNumber, Parser.TokensArray.Length - 1)].Line;
                        return node;
                    }
                    node.AddParameter(Parser.TokensArray[TokenNumber].Value, Parser.TokensArray[TokenNumber + 2].Value);
                    TokenNumber += 3;
                     if (Parser.NeededToken(TokenNumber, "value", ",", 0)){
                        TokenNumber++;
                        continue;
                    }
                    if (Parser.NeededToken(TokenNumber, "value", ")", 0)){
                        TokenNumber++;
                        break;
                    }
                    node.BadParsed = true;
                    node.ErrorLine = Parser.TokensArray[Math.Min(Parser.TokensArray.Length - 1, TokenNumber)].Line;
                    return node;
                }
            }
            if (!Parser.NeededToken(TokenNumber, "value", "is", 0)){
                node.BadParsed = true;
                node.ErrorLine = Parser.TokensArray[Math.Min(TokenNumber, Parser.TokensArray.Length - 1)].Line;
                return node;
            }
            TokenNumber++;
            List<Node> BodyNodes = BodyParser.parse(TokenNumber);
            if (BodyNodes.Count == 0){
                node.BadParsed = true;
                node.ErrorLine = Parser.TokensArray[Math.Min(TokenNumber, Parser.TokensArray.Length - 1)].Line;
                return node;
            }
            Node LastNode = BodyNodes[BodyNodes.Count - 1];
            if (LastNode.BadParsed){
                node.BadParsed = true;
                node.ErrorLine = LastNode.ErrorLine;
                return node;
            }
            for (int i = 0; i < BodyNodes.Count; ++i){
                node.AddBodyNode(BodyNodes[i]);
            }
            TokenNumber = LastNode.TokenNumber;
            
            if (!Parser.NeededToken(TokenNumber, "value", "end", 0)){
                node.BadParsed = true;
                node.ErrorLine = Parser.TokensArray[Math.Min(TokenNumber, Parser.TokensArray.Length - 1)].Line;
                return node;
            }
            TokenNumber++;
            node.TokenNumber = TokenNumber;
            return node;
        }
    }
}