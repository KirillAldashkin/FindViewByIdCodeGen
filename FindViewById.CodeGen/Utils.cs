﻿using System.Text;

namespace KiDev.AndroidAutoFinder.CodeGen;

static class Utils
{
    /* Codegen */
    // Append some information about class
    public static StringBuilder AppendComments(this StringBuilder sb, ClassToGen c) => sb
        .AppendLine("// Code below was generated by \"FindViewById\" code generator. This file contains:")
        .AppendLine($"//  Class: {c.FullName} [OnCreate = {c.OnCreateDefined}; AfterOnCreate = {c.AfterOnCreateDefined}] | SetContentView({c.ContentViewName})")
        .AppendLine(c.Fields.Select(f => $"//   Field: {f.Name} | FindViewById({f.FindViewName})"));

    // Append AutoFind() method for class
    public static StringBuilder AppendAutoFind(this StringBuilder sb, ClassToGen c) => sb
        .AppendLine("\t\tprivate void AutoFind() {")
        .AppendLineIf(c.ContentViewId is not null, $"\t\t\t{c.SetContentViewString}")
        .AppendLine(c.Fields.Select(f => $"\t\t\t{f.FindViewByIdString}"))
        .AppendLine("\t\t}");

    // Append OnCreate(Bundle) method for class
    public static StringBuilder AppendOnCreate(this StringBuilder sb, ClassToGen c) => sb
        .AppendLine("\t\tprotected override void OnCreate(Android.OS.Bundle bundle) {")
        .AppendLine("\t\t\tbase.OnCreate(bundle);")
        .AppendLineIf(c.ContentViewId is not null, $"\t\t\t{c.SetContentViewString}")
        .AppendLine(c.Fields.Select(f => $"\t\t\t{f.FindViewByIdString}"))
        .AppendLineIf(c.AfterOnCreateDefined, $"\t\t\t{c.AfterOnCreateString}")
        .AppendLine("\t\t}");

    /* Utils */
    // Appends string if condition is true
    public static StringBuilder AppendLineIf(this StringBuilder sb, bool cond, string @true)
    {
        if (cond) sb.AppendLine(@true);
        return sb;
    }

    // Applies specified function
    public static StringBuilder ApplyFunc(this StringBuilder sb, Func<StringBuilder, StringBuilder> data) => data(sb);

    // Appends every line in specified enumeration of strings
    public static StringBuilder AppendLine(this StringBuilder sb, IEnumerable<string> data) => 
        data.Aggregate(sb, (sb, str) => sb.AppendLine(str));
}