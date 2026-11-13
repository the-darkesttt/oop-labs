using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text;
using System.Runtime.InteropServices;
using Lab6.Shared;

namespace Object2;

public partial class MainWindow : Window
{
    private readonly PipeCommandServer _server;

    public MainWindow()
    {
        InitializeComponent();

        _server = new PipeCommandServer(
            this,
            "Lab6_Object2",
            HandleSpecificCommandAsync
        );

        _ = _server.ListenAsync();
        Closed += (_, _) => _server.Stop();
    }

    private static int[,] CreateMatrix(int n, int min, int max)
    {
        int[,] matrix = new int[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                matrix[i, j] = (int)Random.Shared.NextInt64(min, (long)max + 1);
            }
        }

        return matrix;
    }

    private static string MatrixToText(int[,] matrix)
    {
        int n = matrix.GetLength(0);

        var text = new StringBuilder();
        text.AppendLine(n.ToString());

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (j > 0)
                {
                    text.Append(' ');
                }

                text.Append(matrix[i, j]);
            }
            // новий рядок
            text.AppendLine();
        }

        return text.ToString();
    }

    private void DisplayMatrix(int[,] matrix)
    {
        int n = matrix.GetLength(0);

        matrixGrid.Rows = n;
        matrixGrid.Columns = n;
        // видалити попередні
        matrixGrid.Children.Clear();

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                var cell = new Border
                {
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1),
                    Background = Brushes.White,
                    MinWidth = 55,
                    MinHeight = 45,

                    Child = new TextBlock
                    {
                        Text = matrix[i, j].ToString(),
                        FontSize = 16,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };

                matrixGrid.Children.Add(cell);
            }
        }
    }

    public bool GenerateAndShow(int n, int min, int max)
    {
        if (n < 1 || n > MatrixSettings.MaxMatrixSize || min > max)
        {
            throw new ArgumentException("Некоректні параметри матриці.");
        }

        int[,] matrix = CreateMatrix(n, min, max);
        DisplayMatrix(matrix);

        try
        {
            Clipboard.SetText(MatrixToText(matrix));
        }
        catch (ExternalException)
        {
            infoText.Text = "Не вдалося записати матрицю до Clipboard.";
            return false;
        }

        infoText.Text =
            $"Матриця {n} × {n}, діапазон: [{min}; {max}]\n" +
            "Дані записано до Clipboard.";

        return true;
    }

    private async Task<string> HandleSpecificCommandAsync(string command)
    {
        string[] parts = command.Split('|');

        if (parts[0] != "GENERATE")
        {
            return "UNKNOWN_COMMAND";
        }

        if (parts.Length != 4 ||
            !int.TryParse(parts[1], out int n) ||
            !int.TryParse(parts[2], out int min) ||
            !int.TryParse(parts[3], out int max) ||
            n < 1 || n > MatrixSettings.MaxMatrixSize || min > max)
        {
            return "ERROR:PARAMETERS";
        }

        bool success = await Dispatcher.InvokeAsync(
            () => GenerateAndShow(n, min, max)
        );

        return success ? "READY" : "ERROR:CLIPBOARD";
    }
}