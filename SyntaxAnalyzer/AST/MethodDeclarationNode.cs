using System.Collections.Generic;
using System.Linq.Expressions;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class MethodDeclarationNode : Node {
        public string name;
        public string ReturnType;
        public List<string> ParameterNames;
        public List<string> ParameterTypes;
        public List<Node> BodyNodes;
        public bool isDestruction;
        public MethodDeclarationNode(){
            ParameterNames = new List<string>();
            ParameterTypes = new List<string>();
            BodyNodes = new List<Node>();
        }
        public MethodDeclarationNode(bool isDestruction)
        {
            this.isDestruction = isDestruction;
            ParameterNames = new List<string>();
            ParameterTypes = new List<string>();
            BodyNodes = new List<Node>();
            this.name = "Destruct";
        }
        public MethodDeclarationNode(string name, string returnType)
        {
            this.name = name;
            ParameterNames = new List<string>();
            ParameterTypes = new List<string>();
            BodyNodes = new List<Node>();
        }
        public MethodDeclarationNode(string name, List<string> parameterNames, List<string> parameterTypes)
        {
            this.name = name;
            ParameterNames = parameterNames;
            ParameterTypes = parameterTypes;
            BodyNodes = new List<Node>();
        }
        public MethodDeclarationNode(string name, string returnType, List<string> parameterNames, List<string> parameterTypes)
        {
            this.name = name;
            this.ReturnType = returnType;
            ParameterNames = parameterNames;
            ParameterTypes = parameterTypes;
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
