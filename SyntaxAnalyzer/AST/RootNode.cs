using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class RootNode : Node {
        public List<Node> ClassDeclarations;
        public RootNode(){
            ClassDeclarations = new List<Node>();
        }
        public void AddClassDeclaration(ClassDeclarationNode node){
            ClassDeclarations.Add(node);
        }
    }
}
