namespace KiDev.AndroidAutoFinder.CodeGen;

// Information about class need for codegen
class ClassToGen
{
    public List<FieldToGen> Fields { get; private set; } = new(); // Fields to codegen
    public int? ContentViewId { get; private set; } // Numerical ID to add to SetContentView(ID). Null if no attribute is specified
    public string ContentViewName { get; private set; } = null; // Constant ID value as in original source code. Null if no attribute is specified
    public string FullName { get; private set; } // Full name of class with namespace
    public bool OnCreateDefined { get; private set; } = false; // Whether OnCreate(Bundle) method is defined in this class
    public bool AfterOnCreateDefined { get; private set; } = false; // Whether AfterOnCreate() method is defined in this class
    public bool AfterOnCreateHasParam { get; private set; } = false; // Whether AfterOnCreate() method have 'Bundle' parameter
    public ClassToGenType ClassType { get; private set; } // Type of class
    public string NameSpace => FullName.Substring(0,FullName.LastIndexOf('.')); // Namespace in which this class is located
    public string Name => FullName.Substring(FullName.LastIndexOf('.')+1); // Name of class
    public string SetContentViewString => $"SetContentView({ContentViewId}); // \"{ContentViewName}\" in your source code";
    public string AfterOnCreateString => $"AfterOnCreate({(AfterOnCreateHasParam ? "bundle" : "")});";

    public ClassToGen(ITypeSymbol classSym)
    {
        ClassType = GetClassType(classSym);
        FullName = classSym.ToDisplayString();
        var attrib = classSym.GetAttributes().FirstOrDefault(CheckAttribute);
        if (attrib != null)
        {
            ContentViewId = (int?)attrib.ConstructorArguments[0].Value;
            var attribSyntax = (AttributeSyntax)attrib.ApplicationSyntaxReference.GetSyntax();
            ContentViewName = attribSyntax.ArgumentList.Arguments[0].ToString();
        }
        foreach (var member in classSym.GetMembers())
        {
            if (member is not IMethodSymbol method) continue;
            if (method.Name == "OnCreate") OnCreateDefined = true;
            if (method.Name == "AfterOnCreate")
            {
                AfterOnCreateDefined = true;
                if (!method.Parameters.IsEmpty) AfterOnCreateHasParam = true;
            }
        }
    }

    // Adds field to this class for codegen
    public void AddField(IFieldSymbol fieldSym) => Fields.Add(new FieldToGen(fieldSym));

    // Checks whether specified class should be added to codegen
    public static bool Check(ITypeSymbol classSym) => 
        GetClassType(classSym) != ClassToGenType.WrongType && 
        classSym.GetAttributes().Any(CheckAttribute);

    static ClassToGenType GetClassType(ITypeSymbol classSym)
    {
        var type = ClassToGenType.WrongType;
        var typeSymbol = classSym;
        while (typeSymbol is not null && type == ClassToGenType.WrongType)
        {
            type = typeSymbol.Name switch
            {
                "Activity" => ClassToGenType.Activity,
                _ => ClassToGenType.WrongType
            };
            typeSymbol = typeSymbol.BaseType;
        }
        return type;
    }

    static bool CheckAttribute(AttributeData attrib) => 
        attrib.AttributeClass.ToDisplayString() == "KiDev.AndroidAutoFinder.SetViewAttribute";
}