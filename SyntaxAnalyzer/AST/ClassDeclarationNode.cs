using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class ClassDeclarationNode : Node {
        public string name;
        public string SuperClass;
        public List<Node> VariableDeclarations, MethodDeclarations, ConstructorDeclarations;
        public ClassDeclarationNode(){
            VariableDeclarations = new List<Node>();
            MethodDeclarations = new List<Node>();
            ConstructorDeclarations = new List<Node>();
        }
        public void SetName (string name){
            this.name = name;
        }
        public void SetSuperClass (string SuperClass){
            this.SuperClass = SuperClass;
        }
        public void AddVariableDeclaration(VariableDeclarationNode node){
            VariableDeclarations.Add(node);
        }
        public void AddMethodDeclaration(MethodDeclarationNode node){
            MethodDeclarations.Add(node);
        }
        public void AddConstructorDeclaraton(ConstructorDeclarationNode node){
            ConstructorDeclarations.Add(node);
        }
    }
}
