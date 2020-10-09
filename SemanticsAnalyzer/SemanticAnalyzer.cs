using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using INNO_F20_CC.SyntaxAnalyzer;
using INNO_F20_CC.TokenAnalyzer;

namespace INNO_F20_CC.SemanticsAnalyzer
{
    public static class SemanticAnalyzer
    {
        private static RootNode _rootNode;
        private static List<Token> _tokens;
        public static void SemanticAnalysis(ref RootNode rootNode, ref List<Token> tokens)
        {
            _rootNode = rootNode;
            _tokens = tokens;
            AddBasicClasses();
            AddDefaultConstructors();

            for (int i = 0; i < _rootNode.ClassDeclarations.Count - 7; i++)
            {
                CheckClass((ClassDeclarationNode)_rootNode.ClassDeclarations[i]);
            }

            AddDestructors();
        }

        static ClassDeclarationNode findClass(string name)
        {
            foreach (ClassDeclarationNode declaration in _rootNode.ClassDeclarations)
            {
                if (name == declaration.name)
                {
                    return declaration;
                }
            }

            return null;
        }

        static void CheckClass(ClassDeclarationNode classDeclarationNode)
        {
            var variables = GetVariables(classDeclarationNode);
            var methods = GetMethods(classDeclarationNode);
            List<string> validVariables = variables[0];
            List<string> validMethods = methods[0];
            List<string> variableTypes = variables[1];
            List<string> methodTypes = methods[1];
            foreach (MethodDeclarationNode method in classDeclarationNode.MethodDeclarations)
            {
                CheckBody(method.BodyNodes, validVariables.Concat(method.ParameterNames).ToList(), validMethods, variableTypes.Concat(method.ParameterTypes).ToList(), classDeclarationNode, method.ReturnType);
            }
        }

        static bool CheckBody(List<Node> body, List<string> validVariables, List<string> validMethods, List<string> variableTypes, ClassDeclarationNode currentClass, string targetReturnType)
        {
            List<string> newVariables = new List<string>();
            List<string> newTypes = new List<string>();
            foreach (Node node in body)
            {
                string type = node.GetType().Name;
                if (type == "VariableDeclarationNode")
                {
                    newVariables.Add(((VariableDeclarationNode) node).name);
                    newTypes.Add(((VariableDeclarationNode) node).expression.call.CalleeNames[0]);
                }else
                if (type == "CallNode")
                {
                    return CheckCall((CallNode) node, validVariables.Concat(newVariables).ToList(), validMethods,
                        variableTypes.Concat(newTypes).ToList(), currentClass, "-1");
                }else
                if (type == "ReturnStatementNode")
                {
                    if (!CheckReturnStatement((ReturnStatementNode)node, validVariables.Concat(newVariables).ToList(), validMethods, variableTypes.Concat(newTypes).ToList(), currentClass, targetReturnType))
                    {
                        Error(node);
                        return false;
                    }
                }else
                if (!CheckNode(node, validVariables.Concat(newVariables).ToList(), validMethods, variableTypes.Concat(newTypes).ToList(), currentClass, targetReturnType))
                {
                    Error(node);
                    return false;
                }
            }
            return true;
        }

        static bool CheckNode(Node node, List<string> validVariables, List<string> validMethods, List<string> variableTypes, ClassDeclarationNode currentClass, string targetReturnType)  //Checks AST Node
        {
            string type = node.GetType().Name;

            if (type == "VariableDeclarationNode")  //if Variable Declaration
            {
                if (((VariableDeclarationNode) node).expression.ExpressionType == "call")  //Check Call
                {
                    return CheckNode(((VariableDeclarationNode)node).expression.call, validVariables, validMethods, variableTypes, currentClass, targetReturnType);
                }
            }
            
            if (type == "IfStatementNode")  //if "If" statement
            {
                if (((IfStatementNode) node).expression.ExpressionType == "call")  //Check Call in condition
                {
                    return CheckCall(((IfStatementNode)node).expression.call, validVariables, validMethods, variableTypes, currentClass, "Boolean");
                }
                if (!CheckBody(((IfStatementNode) node).IfBodyNodes, validVariables, validMethods, variableTypes, currentClass, targetReturnType))  //check "if" branch
                {
                    Error(node);
                    return false;
                }
                if (!CheckBody(((IfStatementNode) node).ElseBodyNodes, validVariables, validMethods, variableTypes, currentClass, targetReturnType))  //check "else" branch
                {
                    Error(node);
                    return false;
                }
            }
            
            if (type == "WhileLoopNode")  //check "While" loop
            {
                if (((WhileLoopNode) node).expression.ExpressionType == "call")  //check condition
                {
                    return CheckCall(((WhileLoopNode)node).expression.call, validVariables, validMethods, variableTypes, currentClass, "Boolean");
                }
                if (!CheckBody(((WhileLoopNode) node).BodyNodes, validVariables, validMethods, variableTypes, currentClass, targetReturnType))  //check body
                {
                    Error(node);
                    return false;
                }
            }

            if (type == "AssignmentNode")  //check assignment operation
            {
                if (!validVariables.Contains(((AssignmentNode) node).name))
                {
                    Error(node);
                    return false;
                }
                if (((AssignmentNode) node).expression.ExpressionType == "call")
                {
                    string targetType = variableTypes[validVariables.IndexOf(((AssignmentNode) node).name)];
                    return CheckCall(((AssignmentNode)node).expression.call, validVariables, validMethods, variableTypes, currentClass, targetType);
                }
            }
            return true;
        }

        static bool CheckReturnStatement(ReturnStatementNode node, List<string> validVariables,
            List<string> validMethods,
            List<string> variableTypes, ClassDeclarationNode currentClass, string targetReturnType)
        {
            return CheckExpression(node.expression, validVariables, validMethods, variableTypes, currentClass, targetReturnType);
        }

        static bool CheckExpression(ExpressionNode node, List<string> validVariables, List<string> validMethods,
            List<string> variableTypes, ClassDeclarationNode currentClass, string targetReturnType)
        {
            if (node.ExpressionType == "call")  //Check Call
            {
                return CheckCall(node.call, validVariables, validMethods, variableTypes, currentClass, targetReturnType);
            }
            else
            {
                if (node.BooleanLiteral == null && targetReturnType == "Boolean" ||
                    node.FloatLiteral == null && targetReturnType == "Real" ||
                    node.IntegerLiteral == null && targetReturnType == "Integer")
                {
                    Error(node);
                    return false;
                }
            }

            return true;
        }
        
        static bool CheckCall(CallNode node, List<string> validVariables, List<string> validMethods,
            List<string> variableTypes, ClassDeclarationNode currentClass, string targetReturnType)
        {
            ClassDeclarationNode returnTypeClass = null;
            int argCount = 0;

            if (node.CallerName == "this")  //nothing
            {
                returnTypeClass = currentClass;
            }
            else  //variable
            {
                if (node.CallerName!=null)  // variable name check
                {
                    if (validVariables.Contains(node.CallerName))
                    {
                        returnTypeClass = findClass(variableTypes[validVariables.IndexOf(node.CallerName)]);
                    }
                    else
                    {
                        Error(node);
                        return false;
                    }
                }
            }

            if (returnTypeClass == null && node.CalleeNames.Count == 0)
            {
                Error(node);
                return false;
            }
            
            foreach (string calleeName in node.CalleeNames)  //check Callee Names
            {
                MethodDeclarationNode method = null;
                if (returnTypeClass == null)
                {
                    if (calleeName != node.CalleeNames[0])
                    {
                        Error(node);
                        return false;
                    }
                    if (validMethods.Contains(calleeName))  //method
                    {
                        method = GetMethod(currentClass, calleeName);
                    }else 
                    if (validVariables.Contains(calleeName)) //variable
                    {
                        method = null;
                        returnTypeClass = findClass(variableTypes[validVariables.IndexOf(calleeName)]);
                    }else 
                    if (findClass(calleeName)!=null)  //constructor
                    {
                        method = null;
                        ClassDeclarationNode constructorClass = findClass(calleeName);
                        bool isArgumentsOK = false;
                        string argumentSignature = "";
                        foreach (ExpressionNode argument in node.arguments[argCount])
                        {
                            argumentSignature += GetReturnType(argument, validVariables, validMethods, variableTypes,
                                currentClass).name;
                        }
                        foreach (ConstructorDeclarationNode constructor in constructorClass.ConstructorDeclarations)
                        {
                            if (constructor.ParameterTypes.Count == node.arguments[argCount].Count)
                            {
                                string parameterSignature = "";
                                foreach (string parameterType in constructor.ParameterTypes)
                                {
                                    parameterSignature += parameterType;
                                }

                                if (parameterSignature == argumentSignature)
                                {
                                    isArgumentsOK = true;
                                    break;
                                }
                            }
                        }

                        if (!isArgumentsOK)
                        {
                            Error(node);
                            return false;
                        }
                        returnTypeClass = constructorClass;
                    }
                    else
                    {
                        Error(node);
                        return false;
                    }
                }
                else
                {
                    if (GetMethods(returnTypeClass)[0].Contains(calleeName))
                    {
                        method = GetMethod(returnTypeClass, calleeName);
                    }else if (GetVariables(returnTypeClass)[0].Contains(calleeName))
                    {
                        returnTypeClass = findClass(GetVariables(returnTypeClass)[1][GetVariables(returnTypeClass)[0].IndexOf(calleeName)]);
                    }
                    else
                    {
                        Error(node);
                        return false;
                    }
                }
                

                if (method != null)  //if method, check arguments
                {
                    if (method.ParameterTypes.Count > 0)
                    {
                        if (method.ParameterTypes.Count != node.arguments[argCount].Count)
                        {
                            Error(node);
                            return false;
                        }

                        for (int i = 0; i < node.arguments[argCount].Count; i++)
                        {
                            if (!CheckExpression(node.arguments[argCount][i], validVariables, validMethods,
                                variableTypes, currentClass, method.ParameterTypes[i]))
                            {
                                Error(node.arguments[argCount][i]);
                                return false;
                            }
                        }
                    }
                    returnTypeClass = method.ReturnType == null ? null : findClass(method.ReturnType);
                }
                argCount++;
            }

            if (targetReturnType != "-1" && returnTypeClass == null && targetReturnType != null) //check maybe we shouldn't return anything
            {
                Error(node);
                return false;
            }

            if (returnTypeClass == null && targetReturnType == null)
            {
                return true;
            }
            if (targetReturnType != "-1" && returnTypeClass.name!=targetReturnType && !GetSuperClasses(returnTypeClass).Contains(targetReturnType))
            {
                Error(node);
                return false;
            }

            return true;
        }

        static List<string>[] GetVariables(ClassDeclarationNode classDeclarationNode)  //Get All Variables From Class
        {
            List<string> result = new List<string>();
            List<string> type = new List<string>();
            foreach (VariableDeclarationNode variable in classDeclarationNode.VariableDeclarations)
            {
                result.Add(variable.name);
                type.Add(variable.expression.call.CalleeNames[0]);
            }
            
            return new []{result,type};
        }
        
        static List<string>[] GetMethods(ClassDeclarationNode classDeclarationNode)  //Get All Methods From Class
        {
            List<string> result = new List<string>();
            List<string> type = new List<string>();
            foreach (MethodDeclarationNode method in classDeclarationNode.MethodDeclarations)
            {
                result.Add(method.name);
                type.Add(method.ReturnType);
            }

            foreach (string name in GetSuperClasses(classDeclarationNode))
            {
                classDeclarationNode = findClass(name);
                foreach (MethodDeclarationNode method in classDeclarationNode.MethodDeclarations)
                {
                    result.Add(method.name);
                    type.Add(method.ReturnType);
                }
            }
            
            return new []{result,type};
        }

        static ClassDeclarationNode GetReturnType(ExpressionNode nod, List<string> validVariables, List<string> validMethods,
            List<string> variableTypes, ClassDeclarationNode currentClass)
        {
            if (nod.ExpressionType == "call")  //Check Call
            {
                ClassDeclarationNode returnTypeClass = null;
                CallNode node = nod.call;
                int argCount = 0;
                if (node.CallerName == "this")  //nothing
                {
                    returnTypeClass = currentClass;
                }
                else  //variable
                {
                    if (node.CallerName!=null)  // variable name check
                    {
                        if (validVariables.Contains(node.CallerName))
                        {
                            returnTypeClass = findClass(variableTypes[validVariables.IndexOf(node.CallerName)]);
                        }
                        else
                        {
                            Error(node);
                        }
                    }
                }

                if (returnTypeClass == null && node.CalleeNames.Count == 0)
                {
                    Error(node);
                }
                
                foreach (string calleeName in node.CalleeNames)  //check Callee Names
                {
                    MethodDeclarationNode method = null;
                    if (returnTypeClass == null)
                    {
                        if (calleeName != node.CalleeNames[0])
                        {
                            Error(node);
                        }
                        if (validMethods.Contains(calleeName))  //method
                        {
                            method = GetMethod(currentClass, calleeName);
                        }else 
                        if (validVariables.Contains(calleeName)) //variable
                        {
                            method = null;
                            returnTypeClass = findClass(variableTypes[validVariables.IndexOf(calleeName)]);
                        }else 
                        if (findClass(calleeName)!=null)  //constructor
                        {
                            method = null;
                            ClassDeclarationNode constructorClass = findClass(calleeName);
                            bool isArgumentsOK = false;
                            string argumentSignature = "";
                            foreach (ExpressionNode argument in node.arguments[argCount])
                            {
                                argumentSignature += GetReturnType(argument, validVariables, validMethods, variableTypes,
                                    currentClass).name;
                            }
                            foreach (ConstructorDeclarationNode constructor in constructorClass.ConstructorDeclarations)
                            {
                                if (constructor.ParameterTypes.Count != node.arguments[argCount].Count)
                                {
                                    string parameterSignature = "";
                                    foreach (string parameterType in constructor.ParameterTypes)
                                    {
                                        parameterSignature += parameterType;
                                    }

                                    if (parameterSignature == argumentSignature)
                                    {
                                        isArgumentsOK = true;
                                        break;
                                    }
                                }
                            }

                            if (!isArgumentsOK)
                            {
                                Error(node);
                            }
                            returnTypeClass = constructorClass;
                        }
                        else
                        {
                            Error(node);
                        }
                    }
                    else
                    {
                        if (GetMethods(returnTypeClass)[0].Contains(calleeName))
                        {
                            method = GetMethod(returnTypeClass, calleeName);
                        }else if (GetVariables(returnTypeClass)[0].Contains(calleeName))
                        {
                            returnTypeClass = findClass(GetVariables(returnTypeClass)[1][GetVariables(returnTypeClass)[0].IndexOf(calleeName)]);
                        }
                        else
                        {
                            Error(node);
                        }
                    }
                    

                    if (method != null)  //if method, check arguments
                    {
                        if (method.ParameterTypes.Count > 0)
                        {
                            if (method.ParameterTypes.Count != node.arguments[argCount].Count)
                            {
                                Error(node);
                            }

                            for (int i = 0; i < node.arguments[argCount].Count; i++)
                            {
                                if (!CheckExpression(node.arguments[argCount][i], validVariables, validMethods,
                                    variableTypes, currentClass, method.ParameterTypes[i]))
                                {
                                    Error(node.arguments[argCount][i]);
                                }
                            }
                        }
                        returnTypeClass = method.ReturnType == null ? null : findClass(method.ReturnType);
                    }
                    argCount++;
                }

                return returnTypeClass;
            }
            else
            {
                if (nod.BooleanLiteral != null)
                {
                    return findClass("Boolean");
                }
                if (nod.IntegerLiteral != null)
                {
                    return findClass("Integer");
                }
                if (nod.FloatLiteral != null)
                {
                    return findClass("Real");
                }
            }

            return null;
        }

        static MethodDeclarationNode GetMethod(ClassDeclarationNode classDeclarationNode, string methodName)
        {
            foreach (MethodDeclarationNode method in classDeclarationNode.MethodDeclarations)
            {
                if (method.name == methodName)
                {
                    return method;
                }
            }

            return null;
        }

        static List<string> GetSuperClasses(ClassDeclarationNode classDeclarationNode)//TODO use
        {
            List<string> result = new List<string>();
            ClassDeclarationNode tmp = classDeclarationNode;
            while (tmp.SuperClass != null)
            {
                result.Add(classDeclarationNode.SuperClass);
                tmp = findClass(classDeclarationNode.SuperClass);
                if (tmp == null)
                {
                    Error(classDeclarationNode);
                }
            }

            return result;
        }
        static void AddDefaultConstructors()  //Adds empty constructors to every class, if they aren't present
        {
            foreach(ClassDeclarationNode classDeclaration in _rootNode.ClassDeclarations)
            {
                bool emptyConstructor = false;
                foreach (ConstructorDeclarationNode contructor in classDeclaration.ConstructorDeclarations)
                {
                    if (contructor.ParameterNames.Count == 0)
                    {
                        emptyConstructor = true;
                        break;
                    }
                }
                if (!emptyConstructor)
                {
                    classDeclaration.AddConstructorDeclaraton(new ConstructorDeclarationNode());
                }
            }
        }

        static void AddDestructors()  //Adds destructor method for every class
        {
            foreach(ClassDeclarationNode classDeclaration in _rootNode.ClassDeclarations)
            {
                bool noDestructor = true;
                foreach (MethodDeclarationNode method in classDeclaration.MethodDeclarations)
                {
                    if (method.isDestruction)
                    {
                        noDestructor = false;
                        break;
                    }
                }
                if (noDestructor)
                {
                    classDeclaration.AddMethodDeclaration(GenerateDestructor(classDeclaration));
                }
            }
        }

        static MethodDeclarationNode GenerateDestructor(ClassDeclarationNode classDeclarationNode)  //Generates destruction method from variable list in class
        {
            foreach (MethodDeclarationNode method in classDeclarationNode.MethodDeclarations)  //check if method already given
            {
                if (method.isDestruction)
                {
                    return method;
                }
            }
            MethodDeclarationNode destructor = new MethodDeclarationNode(true);
            foreach (VariableDeclarationNode declaration in classDeclarationNode.VariableDeclarations)  //for every variable collect a destruction function
            {
                CallNode call = new CallNode();
                call.SetCallerName(declaration.name);
                string className = declaration.expression.call.CalleeNames[0];
                foreach (ClassDeclarationNode classDeclaration in _rootNode.ClassDeclarations)
                {
                    if (classDeclaration.name == className)
                    {
                        call.AddCallee(GenerateDestructor(classDeclaration).name, new List<ExpressionNode>());
                    }
                }
                destructor.AddBodyNode(call);  //Add destruction call to method
            }
            return destructor;
        }

        static void AddBasicClasses()  //Adds Integer, Real, Boolean, Array and List class declarations
        {
            ClassDeclarationNode classInteger = new ClassDeclarationNode("Integer", "AnyValue",
                new List<Node>()
                {
                    new MethodDeclarationNode("toReal", "Real"),
                    new MethodDeclarationNode("toBoolean", "Boolean"),
                    new MethodDeclarationNode("UnaryMinus", "Integer"),
                    new MethodDeclarationNode("UnaryPlus", "Integer"),
                    new MethodDeclarationNode("Plus", "Integer", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("Minus", "Integer", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("Mult", "Integer", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("Div", "Integer", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("Plus", "Real", new List<string>() {"p"}, new List<string>() {"Real"}),
                    new MethodDeclarationNode("Minus", "Real", new List<string>() {"p"}, new List<string>() {"Real"}),
                    new MethodDeclarationNode("Mult", "Real", new List<string>() {"p"}, new List<string>() {"Real"}),
                    new MethodDeclarationNode("Div", "Real", new List<string>() {"p"}, new List<string>() {"Real"}),
                    new MethodDeclarationNode("Rem", "Integer", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("Less", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("LessEqual", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("Greater", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("GreaterEqual", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("Equal", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("Less", "Boolean", new List<string>() {"p"}, new List<string>() {"Real"}),
                    new MethodDeclarationNode("LessEqual", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Real"}),
                    new MethodDeclarationNode("Greater", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Real"}),
                    new MethodDeclarationNode("GreaterEqual", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Real"}),
                    new MethodDeclarationNode("Equal", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Real"}),
                    new MethodDeclarationNode(true)
                },
                new List<Node>()
                {
                    new ConstructorDeclarationNode(new List<string>() {"p"}, new List<string>() {"Integer"}),
                    new ConstructorDeclarationNode(new List<string>() {"p"}, new List<string>() {"Real"})
                },
                new List<Node>()
                {
                    new VariableDeclarationNode("Min", "Integer"),
                    new VariableDeclarationNode("Max", "Integer")
                });
            ClassDeclarationNode classReal = new ClassDeclarationNode("Real", "AnyValue",
                new List<Node>()
                {
                    new MethodDeclarationNode("toInteger", "Integer"),
                    new MethodDeclarationNode("toBoolean", "Boolean"),
                    new MethodDeclarationNode("UnaryMinus", "Real"),
                    new MethodDeclarationNode("UnaryPlus", "Real"),
                    new MethodDeclarationNode("Plus", "Integer", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("Minus", "Integer", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("Mult", "Integer", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("Div", "Integer", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("Plus", "Real", new List<string>() {"p"}, new List<string>() {"Real"}),
                    new MethodDeclarationNode("Minus", "Real", new List<string>() {"p"}, new List<string>() {"Real"}),
                    new MethodDeclarationNode("Mult", "Real", new List<string>() {"p"}, new List<string>() {"Real"}),
                    new MethodDeclarationNode("Div", "Real", new List<string>() {"p"}, new List<string>() {"Real"}),
                    new MethodDeclarationNode("Rem", "Real", new List<string>() {"p"}, new List<string>() {"Integer"}),
                    new MethodDeclarationNode("Less", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("LessEqual", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("Greater", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("GreaterEqual", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("Equal", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("Less", "Boolean", new List<string>() {"p"}, new List<string>() {"Real"}),
                    new MethodDeclarationNode("LessEqual", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Real"}),
                    new MethodDeclarationNode("Greater", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Real"}),
                    new MethodDeclarationNode("GreaterEqual", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Real"}),
                    new MethodDeclarationNode("Equal", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Real"}),
                    new MethodDeclarationNode(true)
                },
                new List<Node>()
                {
                    new ConstructorDeclarationNode(new List<string>() {"p"}, new List<string>() {"Integer"}),
                    new ConstructorDeclarationNode(new List<string>() {"p"}, new List<string>() {"Real"})
                },
                new List<Node>()
                {
                    new VariableDeclarationNode("Min", "Real"),
                    new VariableDeclarationNode("Max", "Real"),
                    new VariableDeclarationNode("Epsilon", "Real")
                });
            ClassDeclarationNode classBoolean = new ClassDeclarationNode("Boolean", "AnyValue",
                new List<Node>()
                {
                    new MethodDeclarationNode("toInteger", "Integer"),
                    new MethodDeclarationNode("Or", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Boolean"}),
                    new MethodDeclarationNode("And", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Boolean"}),
                    new MethodDeclarationNode("Xor", "Boolean", new List<string>() {"p"},
                        new List<string>() {"Boolean"}),
                    new MethodDeclarationNode("Not", "Boolean"),
                    new MethodDeclarationNode(true)
                }
                , new List<Node>()
                {
                    new ConstructorDeclarationNode(new List<string>(){"p"},new List<string>(){"Boolean"} )
                }, 
                    new List<Node>());
            ClassDeclarationNode classArray = new ClassDeclarationNode("Array","AnyRef","T",
                new List<Node>()
                {
                    new MethodDeclarationNode("toList", "List"),
                    new MethodDeclarationNode("Length", "Integer"),
                    new MethodDeclarationNode("get", "T", new List<string>() {"i"},
                        new List<string>() {"Integer"}),
                    new MethodDeclarationNode("set", new List<string>() {"i", "v"},
                        new List<string>() {"Integer", "T"}),
                    new MethodDeclarationNode(true)
                }
                , new List<Node>()
                {
                    new ConstructorDeclarationNode(new List<string>(){"l"},new List<string>(){"Integer"} )
                }, 
                new List<Node>());
            ClassDeclarationNode classList = new ClassDeclarationNode("List","AnyRef","T",
                new List<Node>()
                {
                    new MethodDeclarationNode("append", "List", new List<string>() {"v"},
                        new List<string>() {"T"}),
                    new MethodDeclarationNode("head", "T"),
                    new MethodDeclarationNode("tail", "T"),
                    new MethodDeclarationNode(true)
                }
                , new List<Node>()
                {
                    new ConstructorDeclarationNode(new List<string>(){},new List<string>(){} ),
                    new ConstructorDeclarationNode(new List<string>(){"p"},new List<string>(){"T"} ),
                    new ConstructorDeclarationNode(new List<string>(){"p","count"},new List<string>(){"T","Integer"} )
                }, 
                new List<Node>());
            ClassDeclarationNode anyValue = new ClassDeclarationNode("AnyValue", null, new List<Node>(),new List<Node>(),new List<Node>());
            ClassDeclarationNode anyRef = new ClassDeclarationNode("AnyRef", null, new List<Node>(),new List<Node>(),new List<Node>());

            _rootNode.ClassDeclarations.AddRange(new List<Node>(){classInteger,classReal,classBoolean, classArray, classList, anyValue, anyRef});
        }
        static void Error(Node node)
        {
            Console.WriteLine("ERROR!!1!");
            Console.WriteLine();
            Token token = _tokens[node.TokenNumber-1];
            Console.WriteLine($"Invalid semantics at {token.Line}:{token.Column}.");
            /*
            Console.WriteLine($"Invalid token at {line}:{column}.");
            Console.WriteLine("Press any key to exit...");*/
            Console.ReadKey();
            Environment.Exit(0);
            
        }
    }
}