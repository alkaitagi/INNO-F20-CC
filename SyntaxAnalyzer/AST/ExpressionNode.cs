using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class ExpressionNode : Node {
        public string ExpressionType; //Literal or Call
        public string IntegerLiteral; //if expression is integer literal, "1", "2", "3", etc.
        public string BooleanLiteral; //if expression is boolean literal, "true" or "false"
        public string FloatLiteral; //if expression is float literal, "0.125", "1.521213", etc.
        public CallNode call;
        public ExpressionNode(){
            IntegerLiteral = "";
            BooleanLiteral = "";
            FloatLiteral = "";
        }
        public void SetExpressionType(string ExpressionType){
            this.ExpressionType = ExpressionType;
        }
        public void SetCall(CallNode call){
            this.call = call;
        }
    }
}
