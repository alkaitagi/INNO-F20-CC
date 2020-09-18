using System;
using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class BodyParser {
        public static List<Node> parse(int TokenNumber){
            List<Node> BodyNodes = new List<Node>();
            while (true){
                if (!(Parser.NeededToken(TokenNumber, "type", "name", 0) ||
                Parser.NeededToken(TokenNumber, "value", "while", 0) ||
                Parser.NeededToken(TokenNumber, "value", "if", 0) ||
                Parser.NeededToken(TokenNumber, "value", "return", 0) ||
                Parser.NeededToken(TokenNumber, "value", "var", 0) ||
                Parser.NeededToken(TokenNumber, "value", "this", 0))){
                    return BodyNodes;
                }
                VariableDeclarationNode VariableNode = VariableDeclarationParser.parse(TokenNumber);
                if (!VariableNode.BadParsed){
                    BodyNodes.Add(VariableNode);
                    TokenNumber = VariableNode.TokenNumber;
                    continue;
                }
                AssignmentNode anode = AssignmentParser.parse(TokenNumber);
                if (!anode.BadParsed){
                    BodyNodes.Add(anode);
                    TokenNumber = anode.TokenNumber;
                    continue;
                }
                WhileLoopNode LoopNode = WhileLoopParser.parse(TokenNumber);
                if (!LoopNode.BadParsed){
                    BodyNodes.Add(LoopNode);
                    TokenNumber = LoopNode.TokenNumber;
                    continue;
                }
                IfStatementNode IfNode = IfStatementParser.parse(TokenNumber);
                if (!IfNode.BadParsed){
                    BodyNodes.Add(IfNode);
                    TokenNumber = IfNode.TokenNumber;
                    continue;
                }
                ReturnStatementNode ReturnNode = ReturnStatementParser.parse(TokenNumber);
                if (!ReturnNode.BadParsed){
                    BodyNodes.Add(ReturnNode);
                    TokenNumber = ReturnNode.TokenNumber;
                    continue;
                }
                CallNode cnode = CallParser.parse(TokenNumber);
                if (cnode.BadParsed){
                    Node node = new Node();
                    node.BadParsed = true;
                    node.ErrorLine = Math.Max(Math.Max(VariableNode.ErrorLine, anode.ErrorLine), 
                        Math.Max(LoopNode.ErrorLine, IfNode.ErrorLine));
                    node.ErrorLine = Math.Max(Math.Max(ReturnNode.ErrorLine, cnode.ErrorLine), node.ErrorLine); //deepest parsing error
                    BodyNodes.Add(node);
                    return BodyNodes;
                }
                BodyNodes.Add(cnode);
                TokenNumber = cnode.TokenNumber;
                if (TokenNumber >= Parser.TokensArray.Length){ //if tokens ended
                    Node node = new Node();
                    node.BadParsed = true;
                    node.ErrorLine = Parser.TokensArray[Parser.TokensArray.Length - 1].Line;
                    return BodyNodes;
                }
            }
        }
    }
}