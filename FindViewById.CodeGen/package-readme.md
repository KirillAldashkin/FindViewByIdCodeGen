## FindViewById
.NET Source generator for Android projects
```CSharp
using FindViewById;
using static TestApp.Resource;

namespace TestApp;

[SetView(Layout.activity_main)]
[Activity(Label = "@string/app_name", MainLauncher = true)]
public partial class MainActivity : Activity
{
    [FindById(Id.number_a)] EditText firstNumber;
    [FindById(Id.number_b)] EditText secondNumber;
    [FindById(Id.result_n)] TextView result;

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