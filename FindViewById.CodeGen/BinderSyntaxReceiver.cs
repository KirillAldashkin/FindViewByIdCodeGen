namespace KiDev.AndroidAutoFinder.CodeGen;

internal class BinderSyntaxReceiver : ISyntaxContextReceiver
{
    private const string BindAllAttributeName = "KiDev.AndroidAutoFinder.BindAllAttribute";

    public IReadOnlyList<ClassToBind> Classes => classes;
    private List<ClassToBind> classes = new();

    // C# Compiler will call this method on every syntax node it finds
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is ClassDeclarationSyntax classDecl)
        {
            var classSymbol = (ITypeSymbol)context.SemanticModel.GetDeclaredSymbol(classDecl)!;
            var attr = Utils.GetAttribute(classSymbol, BindAllAttributeName);
            if(attr is not null)
            {
                classes.Add(new(classSymbol, attr));
            }
        }
    }
}