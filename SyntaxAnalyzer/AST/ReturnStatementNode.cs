using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class ReturnStatementNode : Node {
        public bool IsVoid;
        public ExpressionNode expression;
        public void SetExpression(ExpressionNode expression){
            this.expression = expression;
        }
    }
}
