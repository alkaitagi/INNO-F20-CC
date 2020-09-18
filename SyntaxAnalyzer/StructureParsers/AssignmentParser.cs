using System;
using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class AssignmentParser {
        public static AssignmentNode parse(int TokenNumber){
            AssignmentNode node = new AssignmentNode();
            if (!(Parser.NeededToken(TokenNumber, "type", "name", 0) && Parser.NeededToken(TokenNumber + 1, "value", ":=", 0))){
                node.BadParsed = true;
                node.ErrorLine = Parser.TokensArray[Math.Min(TokenNumber, Parser.TokensArray.Length - 1)].Line;
                return node;
            }
            node.SetName(Parser.TokensArray[TokenNumber].Value);
            TokenNumber += 2;
            ExpressionNode ExprNode = ExpressionParser.parse(TokenNumber);
            if (ExprNode.BadParsed){
                node.BadParsed = true;
                node.ErrorLine = ExprNode.ErrorLine;
                return node;
            }
            node.SetExpression(ExprNode);
            node.TokenNumber = ExprNode.TokenNumber;
            return node;
        }
    }
}