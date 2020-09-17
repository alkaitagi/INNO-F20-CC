using System;
using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class CallParser {
        public static CallNode parse(int TokenNumber){
            CallNode node = new CallNode();
            if ((Parser.NeededToken(TokenNumber, "type", "name", 0) || Parser.NeededToken(TokenNumber, "value", "this", 0)) &&
            Parser.NeededToken(TokenNumber + 1, "value", ".", 0)){
                node.SetCallerName(Parser.TokensArray[TokenNumber].Value);
                TokenNumber += 2;
            }
            while (true){
                string CurrentCallee = "";
                List<ExpressionNode> arguments = new List<ExpressionNode>();
                if (!Parser.NeededToken(TokenNumber, "type", "name", 0)){
                    node.BadParsed = true;
                    node.ErrorLine = Parser.TokensArray[Math.Min(Parser.TokensArray.Length - 1, TokenNumber)].Line;
                    return node;
                }
                CurrentCallee = Parser.TokensArray[TokenNumber].Value;
                TokenNumber++;
                if (Parser.NeededToken(TokenNumber, "value", "(", 0)){
                    TokenNumber++;
                    while(true){
                        ExpressionNode enode = ExpressionParser.parse(TokenNumber);
                        if (enode.BadParsed){
                            node.BadParsed = true;
                            node.ErrorLine = enode.ErrorLine;
                            return node;
                        }
                        TokenNumber = enode.TokenNumber;
                        arguments.Add(enode);
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
                    node.AddCallee(CurrentCallee, arguments);
                }
                if (Parser.NeededToken(TokenNumber, "value", ".", 0)){
                    TokenNumber++;
                    continue;
                }
                break;
            }
            node.TokenNumber = TokenNumber;
            return node;
        }
    }
}