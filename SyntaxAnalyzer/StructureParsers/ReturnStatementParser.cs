using System;
using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class ReturnStatementParser {
        public static ReturnStatementNode parse(int TokenNumber){
            ReturnStatementNode node = new ReturnStatementNode();
            if (!Parser.NeededToken(TokenNumber, "value", "return", 0)){
                node.BadParsed = true;
                node.ErrorLine = Parser.TokensArray[Math.Min(TokenNumber, Parser.TokensArray.Length - 1)].Line;
                return node;
            }
            TokenNumber++;
            ExpressionNode ExprNode = ExpressionParser.parse(TokenNumber);
            if (ExprNode.BadParsed){
                node.IsVoid = true;
            }
            else {
                node.SetExpression(ExprNode);
                TokenNumber = ExprNode.TokenNumber;
            }
            node.TokenNumber = TokenNumber;
            return node;
        }
    }
}