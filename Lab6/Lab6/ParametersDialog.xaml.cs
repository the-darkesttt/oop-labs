using System.Windows;
using Lab6.Shared;

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

        if (n < 1 || n > MatrixSettings.MaxMatrixSize)
        {
            MessageBox.Show(
                $"Розмір матриці має бути від 1 до {MatrixSettings.MaxMatrixSize}."
            );
            return;
        }

        if (min > max)
        {
            MessageBox.Show("Мінімальне значення не може перевищувати максимальне.");
            return;
        }

        N = n;
        MinValue = min;
        MaxValue = max;

        DialogResult = true;
    }
}