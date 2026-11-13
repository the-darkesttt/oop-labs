using System.Windows;

namespace Lab6;

public partial class ParametersDialog : Window
{
    public int N { get; private set; }
    public int MinValue { get; private set; }
    public int MaxValue { get; private set; }

    public ParametersDialog()
    {
        InitializeComponent();
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(nBox.Text, out int n) ||
            !int.TryParse(minBox.Text, out int min) ||
            !int.TryParse(maxBox.Text, out int max))
        {
            MessageBox.Show("Введіть цілі числа.");
            return;
        }

        if (n <= 0 || min > max)
        {
            MessageBox.Show("Перевірте розмір матриці та діапазон.");
            return;
        }

        N = n;
        MinValue = min;
        MaxValue = max;

        DialogResult = true;
    }
}