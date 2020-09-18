using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class IfStatementNode : Node {
        public ExpressionNode expression;
        public List<Node> IfBodyNodes;
        public List<Node> ElseBodyNodes;
        public IfStatementNode(){
            IfBodyNodes = new List<Node>();
            ElseBodyNodes = new List<Node>();
        }
        public void SetExpression(ExpressionNode expression){
            this.expression = expression;
        }
        public void AddIfBodyNode(Node node){
            IfBodyNodes.Add(node);
        }
        public void AddElseBodyNode(Node node){
            ElseBodyNodes.Add(node);
        }
    }
}
