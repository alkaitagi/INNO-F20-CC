using System;

namespace INNO_F20_CC.SyntaxAnalyzer {
    public class Node {
        public int TokenNumber;
        public int ErrorLine;
        public bool BadParsed;
        public Node(){
            BadParsed = false;
            ErrorLine = -1;
        }
        public void SetTokenNumber(int TokenNumber){
            this.TokenNumber = TokenNumber;
        }
        public void SetBadParsed(bool BadParsed){
            this.BadParsed = BadParsed;
        }
    }
}