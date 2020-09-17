using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class WhileLoopNode : Node {
        public ExpressionNode expression;
        public List<Node> BodyNodes;
        public WhileLoopNode(){
            BodyNodes = new List<Node>();
        }
        public void SetExpression(ExpressionNode expression){
            this.expression = expression;
        }
        public void AddBodyNode(Node node){
            BodyNodes.Add(node);
        }
    }
}
