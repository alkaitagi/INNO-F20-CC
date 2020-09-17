using System;
using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class WhileLoopParser {
        public static WhileLoopNode parse(int TokenNumber){
            WhileLoopNode node = new WhileLoopNode();
            if (!Parser.NeededToken(TokenNumber, "value", "while", 0)){
                node.BadParsed = true;
                node.ErrorLine = Parser.TokensArray[Math.Min(TokenNumber, Parser.TokensArray.Length - 1)].Line;
                return node;
            }
            TokenNumber++;
            ExpressionNode ExprNode = ExpressionParser.parse(TokenNumber);
            if (ExprNode.BadParsed){
                node.BadParsed = true;
                node.ErrorLine = ExprNode.ErrorLine;
                return node;
            }
            node.SetExpression(ExprNode);
            TokenNumber = ExprNode.TokenNumber;
            if (!Parser.NeededToken(TokenNumber, "value", "loop", 0)){
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
