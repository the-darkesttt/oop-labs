using System.Windows;

namespace Lab1;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Work1_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SliderDialog();
        if (dialog.ShowDialog() == true)
        {
            resultText.Text = $"Вибране число: {dialog.SelectedNumber}";
        }
    }
    private void Work2_Click(object sender, RoutedEventArgs e)
    {
        while (true)
        {
            var dialog = new FirstStepDialog();
            if (dialog.ShowDialog() != true)
            {
                resultText.Text = "Скасування";
                return;
            }

            var secondDialog = new SecondStepDialog();
            secondDialog.ShowDialog();
            if (secondDialog.Action == SecondStepAction.Back)
            {
                continue;
            }

            if (secondDialog.Action == SecondStepAction.Confirm)
            {
                resultText.Text = "Підтверджено";
            }
            else
            {
                resultText.Text = "Скасування";
            }

            return;
        }
    }
}