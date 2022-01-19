namespace KiDev.AndroidAutoFinder.CodeGen;

// Information about field need for codegen
class FieldToGen
{
    public string Type { get; private set; } // Type of field to find
    public string FullName { get; private set; } // Full name of field with class and namespace
    public int? FindId { get; private set; } // Numerical ID of resource
    public string FindName { get; private set; } // Constant ID value as in original source code
    public FieldToGenType FieldType { get; private set; } // Type of field

    public string Name => FullName.Substring(FullName.LastIndexOf('.')+1); // Name of field

    public string GetSourceString() => FieldType switch
    {
        FieldToGenType.View => $"{Name} = FindViewById<{Type}>({FindId});",
        FieldToGenType.Bitmap => $"{Name} = Android.Graphics.BitmapFactory.DecodeResource(Resources, {FindId});",
        _ => throw new InvalidOperationException(),
    } + $" // \"{FindName}\" in your source code";

    public FieldToGen(IFieldSymbol fieldSym)
    {
        FullName = fieldSym.ToDisplayString();
        Type = fieldSym.Type.ToDisplayString();
        var attrib = fieldSym.GetAttributes().FirstOrDefault(CheckAttribute);
        FindId = (int)attrib.ConstructorArguments[0].Value;
        var attribSyntax = (AttributeSyntax)attrib.ApplicationSyntaxReference.GetSyntax();
        FindName = attribSyntax.ArgumentList.Arguments[0].ToString();
        FieldType = GetFieldType(fieldSym);
    }

    // Checks whether specified field should be added to codegen
    public static bool Check(IFieldSymbol fieldSym) => 
        GetFieldType(fieldSym) != FieldToGenType.WrongType && 
        fieldSym.GetAttributes().Any(CheckAttribute);

    static FieldToGenType GetFieldType(IFieldSymbol fieldSym)
    {
        var type = FieldToGenType.WrongType;
        var typeSymbol = fieldSym.Type;
        while (typeSymbol is not null && type == FieldToGenType.WrongType)
        {
            type = typeSymbol.Name switch
            {
                "View" => FieldToGenType.View,
                "Bitmap" => FieldToGenType.Bitmap,
                _ => FieldToGenType.WrongType
            };
            typeSymbol = typeSymbol.BaseType;
        }
        return type;
    }

    static bool CheckAttribute(AttributeData attrib) => 
        attrib.AttributeClass.ToDisplayString() == "KiDev.AndroidAutoFinder.FindByIdAttribute";
}