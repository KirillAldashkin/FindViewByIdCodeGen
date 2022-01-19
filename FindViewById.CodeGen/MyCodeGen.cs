global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace KiDev.AndroidAutoFinder.CodeGen;

[Generator]
public class MyCodeGen : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not MySyntaxReceiver receiver) return;
        // generate source file for every class we found
        foreach (var @class in receiver.Classes)
            context.AddSource($"{@class.FullName}.GenBy.FindViewById.cs", GenerateCode(@class));
    }

    // register syntax receiver so C# compiler will notify us about every syntaxnode it finds
    public void Initialize(GeneratorInitializationContext context) => 
        context.RegisterForSyntaxNotifications(() => new MySyntaxReceiver());

    string GenerateCode(ClassToGen @class) => new StringBuilder()
        .AppendComments(@class)
        .AppendLine($"namespace {@class.NameSpace} {{")
        .AppendLine($"\tpartial class {@class.Name} {{")
        // if 'OnCreate(Bundle)' is already defined then generate 'AutoFind()' method
        // that programmer will explicitly call otherwise generate 'OnCreate(Bundle)'
        .ApplyFunc(sb => @class.OnCreateDefined ? sb.AppendAutoFind(@class) : sb.AppendOnCreate(@class))
        .AppendLine("\t}")
        .Append("}")
        .ToString();
}