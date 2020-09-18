using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class VariableDeclarationNode : Node {
        public string name;
        public ExpressionNode expression;
        public void SetName(string name){
            this.name = name;
        }
        public void SetExpression(ExpressionNode expression){
            this.expression = expression;
        }
    }
}
