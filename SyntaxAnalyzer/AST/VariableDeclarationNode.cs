using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class VariableDeclarationNode : Node {
        public string name;
        public ExpressionNode expression;

        public VariableDeclarationNode(string name, string type)
        {
            this.name = name;
            this.expression = new ExpressionNode()
            {
                ExpressionType = "Call",
                call = new CallNode()
                {
                    CallerName = null,
                    CalleeNames = new List<string>() {type}
                }
            };
        }

        public VariableDeclarationNode(){}
        public void SetName(string name)
        {
            this.name = name;
        }

        public void SetExpression(ExpressionNode expression){
            this.expression = expression;
        }
    }
}
