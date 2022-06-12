using System.Text;
using System.Xml;

namespace KiDev.AndroidAutoFinder.CodeGen;

internal class BindCodeGenerator
{
    public string? RootNameSpace { get; set; }

    public void AddLayoutFiles(IEnumerable<FileInfo> newFiles) => layoutFiles = layoutFiles.Concat(newFiles);
    private IEnumerable<FileInfo> layoutFiles = Enumerable.Empty<FileInfo>();

    public string Generate(ClassToBind cl)
    {
        Dictionary<string, string> views = ParseLayoutXml(cl);
        var rsrc = new StringBuilder();

        AppendHeader(rsrc, cl);
        AppendViewFields(rsrc, views);
        AppendBinderMethod(rsrc, cl, views);
        AppendOnCreate(rsrc, cl);

        return rsrc.AppendLine($"\t}}")
                   .AppendLine($"}}")
                   .ToString();
    }

    private Dictionary<string, string> ParseLayoutXml(ClassToBind cl)
    {
        var xmlPath = layoutFiles.First(file => file.Name == $"{cl.LayoutName}.xml").FullName;
        var xml = new XmlDocument();
        xml.Load(xmlPath);

        var x = from node in xml.AllNodes()
                select (node.Name, node.Attributes?["android:id"]?.Value) into pair
                where pair.Value is not null
                select (pair.Name, pair.Value.Split('/').Last());
        // Key: string id; Value: view type
        return x.ToDictionary(p => p.Item2!, p => p.Name);
    }

    private void AppendHeader(StringBuilder to, ClassToBind cl)
    {
        // file header
        to.AppendLine($"// Auto generated code for \"{cl.FullName}\" by \"KiDev.AndroidAutoFinder\"")
          .AppendLine($"using System;")
          .AppendLine($"using Android.Widget;");

        if (RootNameSpace is not null) to.AppendLine($"using {RootNameSpace};");

        // namespace and class definition
        to.AppendLine()
          .AppendLine($"namespace {cl.NameSpace} {{")
          .AppendLine($"\tpartial class {cl.Name} {{");
    }

    private static void AppendViewFields(StringBuilder to, Dictionary<string, string> views)
    {
        foreach (var pair in views)
            to.AppendLine($"\t\tprivate {pair.Value} {pair.Key};");
    }

    private void AppendBinderMethod(StringBuilder to, ClassToBind cl, Dictionary<string, string> views)
    {
        //find views
        to.AppendLine()
          .AppendLine($"\t\t[System.Runtime.CompilerServices.CompilerGenerated]")
          .AppendLine($"\t\tprivate void AutoBind() {{")
          .AppendLine($"\t\t\tSetContentView(Resource.Layout.{cl.LayoutName});");
        foreach (var pair in views)
            to.AppendLine($"\t\t\t{pair.Key} = FindViewById<{pair.Value}>(Resource.Id.{pair.Key}) ?? " +
                                             $"throw new Exception(\"Couldn't bind \\\"{pair.Key}\\\"\");");

        //bind handlers
        foreach (var (name, handlers) in cl.GetHandlerMethods())
        {
            foreach(var (kind, id) in handlers)
            {
                to.AppendLine($"\t\t\t{id}.{kind} += {name};");
            }
        }

        to.AppendLine($"\t\t}}");
    }

    private void AppendOnCreate(StringBuilder to, ClassToBind cl)
    {
        if (!cl.HasMethod("OnCreate"))
        {
            to.AppendLine()
              .AppendLine($"\t\t[System.Runtime.CompilerServices.CompilerGenerated]")
              .AppendLine($"\t\tprotected override void OnCreate(Android.OS.Bundle state)")
              .AppendLine($"\t\t{{")
              .AppendLine($"\t\t\tbase.OnCreate(state);")
              .AppendLine($"\t\t\tAutoBind();");
            if (cl.HasMethod("AfterOnCreate", 1)) to.AppendLine("\t\t\tAfterOnCreate(state);");
            else if (cl.HasMethod("AfterOnCreate")) to.AppendLine("\t\t\tAfterOnCreate();");
            to.AppendLine($"\t\t}}");
        }
    }
}