using System;
using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {  
    public class VariableDeclarationParser {
        public static VariableDeclarationNode parse(int TokenNumber){
            VariableDeclarationNode node = new VariableDeclarationNode();
            if (!(Parser.NeededToken(TokenNumber, "value", "var", 0) && 
            Parser.NeededToken(TokenNumber + 1, "type", "name", 0) &&
            Parser.NeededToken(TokenNumber + 2, "value", ":", 0))){
                node.BadParsed = true;
                node.ErrorLine = Parser.TokensArray[Math.Min(TokenNumber, Parser.TokensArray.Length - 1)].Line;
                return node;
            }
            node.SetName(Parser.TokensArray[TokenNumber + 1].Value);
            TokenNumber += 3; 
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