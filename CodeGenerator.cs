using System;
using System.Reflection;
using System.Reflection.Emit;
using INNO_F20_CC.SyntaxAnalyzer;

namespace INNO_F20_CC
{
    public class CodeGenerator
    {
        void GenerateCode()
        {
            AssemblyName aName = new AssemblyName(Guid.NewGuid().ToString());
            AssemblyBuilder ab =  AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
            ModuleBuilder mb = ab.DefineDynamicModule(aName.Name + ".dll");
        }
        Type TypeGenerator(ModuleBuilder mb, ClassDeclarationNode classDeclarationNode)
        {
            
            TypeBuilder tb = mb.DefineType(
                classDeclarationNode.name,
                TypeAttributes.Public);
            
            
            FieldBuilder[] fields = new FieldBuilder[classDeclarationNode.VariableDeclarations.Count];
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = tb.DefineField(
                    ((VariableDeclarationNode)classDeclarationNode.VariableDeclarations[i]).name,
                    typeof(int),
                    FieldAttributes.Private);
            }
            
            Type[] parameterTypes = { typeof(int) };
            ConstructorBuilder ctor1 = tb.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                parameterTypes);

            return tb.CreateType();
        }
    }
}