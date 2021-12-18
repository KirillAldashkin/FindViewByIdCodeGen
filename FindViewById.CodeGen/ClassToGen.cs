namespace KiDev.AndroidAutoFinder.CodeGen;

// Information about class need for codegen
class ClassToGen
{
    public List<FieldToGen> Fields = new(); // Fields to codegen
    public int? ContentViewId = null; // Numerical ID to add to SetContentView(ID). Null if no attribute is specified
    public string ContentViewName = null; // Constant ID value as in original source code. Null if no attribute is specified
    public string FullName; // Full name of class with namespace
    public bool OnCreateDefined = false; // Whether OnCreate(Bundle) method is defined in this class
    public bool AfterOnCreateDefined = false; // Whether AfterOnCreate() method is defined in this class
    public bool AfterOnCreateHasParam = false; // Whether AfterOnCreate() method have 'Bundle' parameter

    public string NameSpace => FullName.Substring(0,FullName.LastIndexOf('.')); // Namespace in which this class is located
    public string Name => FullName.Substring(FullName.LastIndexOf('.')+1); // Name of class
    public string SetContentViewString => $"SetContentView({ContentViewId}); // \"{ContentViewName}\" in your source code";
    public string AfterOnCreateString => $"AfterOnCreate({(AfterOnCreateHasParam ? "bundle" : "")});";

    public ClassToGen(ITypeSymbol classSym)
    {
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
    public static bool Check(ITypeSymbol classSym) => classSym.GetAttributes().Any(CheckAttribute);
    static bool CheckAttribute(AttributeData attrib) => 
        attrib.AttributeClass.ToDisplayString() == "KiDev.AndroidAutoFinder.SetViewAttribute";
}