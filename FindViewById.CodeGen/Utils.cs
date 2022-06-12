global using static KiDev.AndroidAutoFinder.CodeGen.Utils;
using System.Xml;

namespace KiDev.AndroidAutoFinder.CodeGen;

static class Utils
{
    public static AttributeData? GetAttribute(this ITypeSymbol classSym, string name) =>
        classSym.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == name);

    public static IEnumerable<FileInfo> AllFiles(string path)
    {
        var dir = new DirectoryInfo(path);
        if (!dir.Exists) return Enumerable.Empty<FileInfo>();
        var ret = dir.EnumerateFiles();
        foreach (var subdir in dir.EnumerateDirectories()) 
            ret = ret.Concat(AllFiles(subdir!.FullName));
        return ret;
    }

    public static IEnumerable<XmlNode> AllNodes(this XmlNode root)
    {
        var ret = Enumerable.Empty<XmlNode>();
        foreach(XmlNode child in root.ChildNodes)
            ret = ret.Concat(new[] { child }).Concat(AllNodes(child));
        return ret;
    }
}