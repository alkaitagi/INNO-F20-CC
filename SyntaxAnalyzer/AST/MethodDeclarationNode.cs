using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class MethodDeclarationNode : Node {
        public string name;
        public string ReturnType;
        public List<string> ParameterNames;
        public List<string> ParameterTypes;
        public List<Node> BodyNodes;
        public MethodDeclarationNode(){
            ParameterNames = new List<string>();
            ParameterTypes = new List<string>();
            BodyNodes = new List<Node>();
        }
        public void SetName(string name){
            this.name = name;
        }
        public void SetReturnType(string ReturnType){
            this.ReturnType = ReturnType;
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
