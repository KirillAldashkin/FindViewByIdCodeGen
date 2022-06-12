global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KiDev.AndroidAutoFinder.CodeGen;

[Generator]
public class BinderCodeGen : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not BinderSyntaxReceiver receiver) return;

        var opts = context.AnalyzerConfigOptions.GlobalOptions;
        if (!opts.TryGetValue("build_property.projectdir", out var projectDir)) return;
        if (!opts.TryGetValue("build_property.rootnamespace", out var rootNamespace)) return;
        if (projectDir is null || rootNamespace is null) return;

        projectDir = Path.Combine(projectDir, "Resources", "layout");

        var generator = new BindCodeGenerator();
        generator.AddLayoutFiles(AllFiles(projectDir));
        generator.RootNameSpace = rootNamespace;

        foreach (ClassToBind c in receiver.Classes)
            context.AddSource($"{c.FullName}.Generator.KiDev.AndroidAutoFinder.cs", generator.Generate(c));
    }

    // register syntax receiver so C# compiler will notify us about every syntax node it finds
    public void Initialize(GeneratorInitializationContext context) => 
        context.RegisterForSyntaxNotifications(() => new BinderSyntaxReceiver());
}