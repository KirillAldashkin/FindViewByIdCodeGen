namespace KiDev.AndroidAutoFinder.CodeGen;

internal class ClassToBind
{
    private ITypeSymbol symbol;
    private string layoutFileName;

    public ClassToBind(ITypeSymbol symbol, AttributeData bindAll)
    {
        this.symbol = symbol;
        var bindAllSyntax = (AttributeSyntax)bindAll.ApplicationSyntaxReference!.GetSyntax();
        layoutFileName = bindAllSyntax.ArgumentList!.Arguments[0].ToString().Split('.').Last();
    }

    public string LayoutName => layoutFileName;
    public string FullName => symbol.ToDisplayString();
    public string NameSpace => FullName.Substring(0, FullName.LastIndexOf('.'));
    public string Name => FullName.Substring(FullName.LastIndexOf('.') + 1);

    public IEnumerable<(string name, IEnumerable<(string kind, string id)>)> GetHandlerMethods()
    {
        return from m in symbol.GetMembers()
               where m is IMethodSymbol
               select m as IMethodSymbol into method
               select (method.Name, method.GetAttributes().Where(IsHandlerAttribute)) into pair
               where pair.Item2.Any()
               select (pair.Name, pair.Item2.Select(GetTypeAndId));

        static (string, string) GetTypeAndId(AttributeData data)
        {
            var kind = data.AttributeClass?.Name!;
            kind = kind.Substring(2, kind.Length - 11);
            var syntax = (AttributeSyntax)data.ApplicationSyntaxReference!.GetSyntax();
            var id = syntax.ArgumentList!.Arguments[0].ToString().Split('.').Last();
            return (kind, id);
        }
        static bool IsHandlerAttribute(AttributeData data)
        {
            string[] names = {
                "KiDev.AndroidAutoFinder.OnClickAttribute",
                "KiDev.AndroidAutoFinder.OnTextChangedAttribute",
            };
            return names.Contains(data.AttributeClass?.ToDisplayString()!);
        }
    }

    public bool HasMethod(string name) =>
        symbol.GetMembers().Any(member => member is IMethodSymbol && member.Name == name);

    public bool HasMethod(string name, int args) =>
        symbol.GetMembers().Any(member => member is IMethodSymbol met && 
                                          member.Name == name && 
                                          met.Parameters.Length == args);
}