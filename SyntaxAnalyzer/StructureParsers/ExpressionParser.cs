using System;
using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class ExpressionParser {
        public static ExpressionNode parse(int TokenNumber){
            ExpressionNode node = new ExpressionNode();
            if (Parser.NeededToken(TokenNumber, "type", "int", 0)){
                node.ExpressionType = "literal";
                node.IntegerLiteral = Parser.TokensArray[TokenNumber].Value;
                TokenNumber++;
                node.TokenNumber = TokenNumber;
                return node;
            }
            if (Parser.NeededToken(TokenNumber, "type", "bool", 0)){
                node.ExpressionType = "literal";
                node.BooleanLiteral = Parser.TokensArray[TokenNumber].Value;
                TokenNumber++;
                node.TokenNumber = TokenNumber;
                return node;
            }
            if (Parser.NeededToken(TokenNumber, "type", "float", 0)){
                node.ExpressionType = "literal";
                node.FloatLiteral = Parser.TokensArray[TokenNumber].Value;
                TokenNumber++;
                node.TokenNumber = TokenNumber;
                return node;
            }
            CallNode cnode = CallParser.parse(TokenNumber);
            if (cnode.BadParsed){
                node.BadParsed = true;
                node.ErrorLine = cnode.ErrorLine;
                return node;
            }
            node.TokenNumber = cnode.TokenNumber;
            node.SetCall(cnode);
            return node;
        }
    }
}