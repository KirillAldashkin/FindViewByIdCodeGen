namespace KiDev.AndroidAutoFinder.CodeGen;

class MySyntaxReceiver : ISyntaxContextReceiver
{
    public List<ClassToGen> Classes { get; private set; } = new(); // Classes for codegen found by this receiver

    // Returns class for specified symbol it if it already exists,
    // otherwise adds new class for specified symbol and returns it
    ClassToGen FindOrAddClass(ITypeSymbol classSym)
    {
        string name = classSym.ToDisplayString();
        int index = Classes.FindIndex(@class => @class.FullName == name);
        if (index > -1) 
            return Classes[index];
        else
        {
            ClassToGen @class = new(classSym);
            Classes.Add(@class);
            return @class;
        }
    }

    // C# Compiler will call this method on every syntaxnode it finds
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is ClassDeclarationSyntax classDecl)
        {
            var classSymbol = (ITypeSymbol)context.SemanticModel.GetDeclaredSymbol(classDecl);
            if (classSymbol is null) return;
            // Add class using "FindOrAddClass" to prevent duplication of classes (because of 'partial' keyword)
            if (ClassToGen.Check(classSymbol)) FindOrAddClass(classSymbol);
        }
        else if (context.Node is FieldDeclarationSyntax fieldDecl &&
                 fieldDecl.Parent is ClassDeclarationSyntax pClassDecl)
        {
            foreach (VariableDeclaratorSyntax varDecl in fieldDecl.Declaration.Variables)
            {
                var varSymbol = (IFieldSymbol)context.SemanticModel.GetDeclaredSymbol(varDecl);
                var classSymbol = (ITypeSymbol)context.SemanticModel.GetDeclaredSymbol(pClassDecl);
                if (varSymbol is null || classSymbol is null) return;
                // Add a class using "FindOrAddClass", since the class in which this field is located may not
                // have been added at this time, for example, if it is not marked with the "etView" attribute
                if (FieldToGen.Check(varSymbol)) FindOrAddClass(classSymbol).AddField(varSymbol);
            }
        }
    }
}