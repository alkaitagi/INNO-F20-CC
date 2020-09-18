using System.Collections.Generic;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class CallNode : Node {
        public string CallerName; //if exists, this or ClassName structure
        public List<string> CalleeNames; //if exists
        public List<List<ExpressionNode> > arguments;
        public CallNode(){
            arguments = new List<List<ExpressionNode> >();
            CalleeNames = new List<string>();
        }
        public void SetCallerName(string CallerName){
            this.CallerName = CallerName;
        }
        public void AddCallee(string CalleeName, List<ExpressionNode> arguments){
            CalleeNames.Add(CalleeName);
            this.arguments.Add(arguments);
        }
    }
}
