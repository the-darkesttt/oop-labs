using System.Windows;

namespace Lab5;

public partial class App : Application
{
    internal static MyEditor Editor => MyEditor.Instance;
}