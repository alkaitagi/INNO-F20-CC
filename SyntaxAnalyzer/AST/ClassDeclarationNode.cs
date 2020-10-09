using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class ClassDeclarationNode : Node {
        public string name;
        public string SuperClass;
        public string GenericClass;
        public List<Node> VariableDeclarations, MethodDeclarations, ConstructorDeclarations;
        public ClassDeclarationNode(){
            VariableDeclarations = new List<Node>();
            MethodDeclarations = new List<Node>();
            ConstructorDeclarations = new List<Node>();
        }
        public ClassDeclarationNode(string name,string superClass, List<Node> methodDeclarationNodes, List<Node> constructorDeclarationNodes, List<Node> variableDeclarations){
            this.name = name;
            this.SuperClass = superClass;
            VariableDeclarations = variableDeclarations;
            MethodDeclarations = methodDeclarationNodes;
            ConstructorDeclarations = constructorDeclarationNodes;
        }
        public ClassDeclarationNode(string name,string superClass, string genericClass, List<Node> methodDeclarationNodes, List<Node> constructorDeclarationNodes, List<Node> variableDeclarations){
            this.name = name;
            this.SuperClass = superClass;
            this.GenericClass = genericClass;
            VariableDeclarations = variableDeclarations;
            MethodDeclarations = methodDeclarationNodes;
            ConstructorDeclarations = constructorDeclarationNodes;
        }
        public ClassDeclarationNode(string name,string superClass, string genericClass){
            this.name = name;
            this.SuperClass = superClass;
            this.GenericClass = genericClass;
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

        public void SetGenericClass(string genericClass)
        {
            this.GenericClass = genericClass;
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
