using System.Windows;
using System.Runtime.InteropServices;
using System.Numerics;
using Lab6.Shared;

namespace Object3;

public partial class MainWindow : Window
{
    private readonly PipeCommandServer _server;
    public MainWindow()
    {
        InitializeComponent();

        _server = new PipeCommandServer(
            this,
            "Lab6_Object3",
            HandleSpecificCommandAsync
        );

        _ = _server.ListenAsync();
        Closed += (_, _) => _server.Stop();
    }

    private static int[,] TextToMatrix(string text)
    {
        string[] lines = text.Split(
            new[] { '\r', '\n' },
            StringSplitOptions.RemoveEmptyEntries
        );

        if (lines.Length == 0 ||
            !int.TryParse(lines[0], out int n) ||
            n < 1 || n > MatrixSettings.MaxMatrixSize ||
            lines.Length != (long)n + 1)
        {
            throw new FormatException("Некоректний розмір матриці.");
        }

        int[,] matrix = new int[n, n];

        for (int i = 0; i < n; i++)
        {
            string[] numbers = lines[i + 1].Split(
                (char[]?)null,
                StringSplitOptions.RemoveEmptyEntries
            );

            if (numbers.Length != n)
            {
                throw new FormatException("Некоректна кількість елементів.");
            }

            for (int j = 0; j < n; j++)
            {
                if (!int.TryParse(numbers[j], out matrix[i, j]))
                {
                    throw new FormatException("Матриця містить некоректні числа.");
                }
            }
        }

        return matrix;
    }

    public bool ReadMatrixFromClipboard()
    {
        try
        {
            if (!Clipboard.ContainsText())
            {
                throw new FormatException("Clipboard не містить тексту.");
            }

            string text = Clipboard.GetText();
            int[,] matrix = TextToMatrix(text);
            BigInteger determinant = MatrixMath.Determinant(matrix);
            DisplayMatrix(matrix);

            resultText.Text = $"det(A) = {determinant}";

            return true;
        }
        catch (Exception ex) when (
            ex is FormatException or ExternalException)
        {
            matrixText.Clear();
            resultText.Text = $"Помилка: {ex.Message}";

            return false;
        }
    }

    private void DisplayMatrix(int[,] matrix)
    {
        int n = matrix.GetLength(0);

        var text = new System.Text.StringBuilder();

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                text.Append(matrix[i, j].ToString().PadLeft(6));
            }

            text.AppendLine();
        }

        matrixText.Text = text.ToString();
    }

    private async Task<string> HandleSpecificCommandAsync(string command)
    {
        if (command != "CALCULATE")
        {
            return "UNKNOWN_COMMAND";
        }

        bool success = await Dispatcher.InvokeAsync(
            () => ReadMatrixFromClipboard()
        );

        return success ? "READY" : "ERROR:DATA";
    }
}