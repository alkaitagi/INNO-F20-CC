using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class ConstructorDeclarationNode : Node {
        public List<string> ParameterNames;
        public List<string> ParameterTypes;
        public List<Node> BodyNodes;
        public ConstructorDeclarationNode(){
            ParameterNames = new List<string>();
            ParameterTypes = new List<string>();
            BodyNodes = new List<Node>();
        }
        public void AddParameter(string ParameterName, string ParameterType){
            ParameterNames.Add(ParameterName);
            ParameterTypes.Add(ParameterType);
        }
        public void AddBodyNode(Node node){
            BodyNodes.Add(node);
        }
    }
}
