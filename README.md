# KiDev.AndroidAutoFinder
## About
.NET Source generator for Android projects
## Using
1) Install - This package is available on NuGet: `dotnet add package KiDev.AndroidAutoFinder`
2) Mark your **Activity** class as `partial`

That's all! Now you can use attributes to automate some tasks:
```CSharp
using KiDev.AndroidAutoFinder;
using static TestApp.Resource;

namespace TestApp;

[SetView(Layout.activity_main)]
[Activity(Label = "@string/app_name", MainLauncher = true)]
public partial class MainActivity : Activity
{
    [FindById(Id.number_a)] EditText firstNumber;
    [FindById(Id.number_b)] EditText secondNumber;
    [FindById(Id.result_n)] TextView result;

    // OnCreate(Bundle) method is autogenerated and
    // AfterOnCreate() method is invoked if present
    void AfterOnCreate()
    {
        firstNumber.TextChanged += (_, _) => result.Text = GetSummString();
        secondNumber.TextChanged += (_, _) => result.Text = GetSummString();
    }
    private string GetSummString()
    {
        if (!double.TryParse(firstNumber.Text, out double a)) return "Left number is incorrect";
        if (!double.TryParse(secondNumber.Text, out double b)) return "Right number is incorrect";
        return $"Result: {a+b}";
    }
}

```