using KiDev.AndroidAutoFinder;
using static TestApp.Resource;

namespace TestApp;

[BindAll(Layout.activity_main)]
[Activity(Label = "@string/app_name", MainLauncher = true)]
public partial class MainActivity : Activity
{
    [OnTextChanged(Id.leftNumberInput)]
    [OnTextChanged(Id.rightNumberInput)]
    private void GetSumText(object s, object args) => 
        sumTextView.Text = (double.TryParse(leftNumberInput.Text, out var left),
                            double.TryParse(rightNumberInput.Text, out var right)) switch
        {
            (false, _) => "Wrong left number",
            (_, false) => "Wrong right number",
            _ => $"{left}+{right}={left + right}"
        };
}