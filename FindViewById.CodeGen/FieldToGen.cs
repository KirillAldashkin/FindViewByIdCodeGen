namespace FindViewById.CodeGen;

// Information about field need for codegen
class FieldToGen
{
    public string Type; // Type of field to add to FindViewById<Type>
    public string FullName; // Full name of field with class and namespace
    public int? FindViewId; // Numerical ID to add to FindViewById<>(ID)
    public string FindViewName; // Constant ID value as in original source code

    public string Name => FullName.Substring(FullName.LastIndexOf('.')+1); // Name of field
    public string FindViewByIdString => $"{Name} = FindViewById<{Type}>({FindViewId}); // \"{FindViewName}\" in your source code";

    public FieldToGen(IFieldSymbol fieldSym)
    {
        FullName = fieldSym.ToDisplayString();
        Type = fieldSym.Type.ToDisplayString();
        Type = Type.Substring(Type.LastIndexOf('.')+1);
        var attrib = fieldSym.GetAttributes().FirstOrDefault(CheckAttribute);
        FindViewId = (int)attrib.ConstructorArguments[0].Value;
        var attribSyntax = (AttributeSyntax)attrib.ApplicationSyntaxReference.GetSyntax();
        FindViewName = attribSyntax.ArgumentList.Arguments[0].ToString();
    }

    // Checks whether specified field should be added to codegen
    public static bool Check(IFieldSymbol fieldSym) => fieldSym.GetAttributes().Any(CheckAttribute);
    static bool CheckAttribute(AttributeData attrib) => 
        attrib.AttributeClass.ToDisplayString() == "FindViewById.FindByIdAttribute";
}