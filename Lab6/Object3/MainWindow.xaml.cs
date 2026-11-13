using System;
using System.Windows;
using System.Runtime.InteropServices;
using System.Numerics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Object3;

public partial class MainWindow : Window
{
    private readonly CancellationTokenSource _stopListening = new();
    public MainWindow()
    {
        InitializeComponent();
        _ = ListenAsync();
        Closed += (_, _) => _stopListening.Cancel();
    }

    private static int[,] TextToMatrix(string text)
    {
        string[] lines = text.Split(
            new[] { '\r', '\n' },
            StringSplitOptions.RemoveEmptyEntries
        );

        if (lines.Length == 0 ||
            !int.TryParse(lines[0], out int n) ||
            n <= 0 ||
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

    private async Task ListenAsync()
    {
        try
        {
            while (!_stopListening.IsCancellationRequested)
            {
                using var pipe = new NamedPipeServerStream(
                    "Lab6_Object3",
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous
                );

                await pipe.WaitForConnectionAsync(_stopListening.Token);

                using var reader = new StreamReader(pipe);
                using var writer = new StreamWriter(pipe)
                {
                    AutoFlush = true
                };

                string? command = await reader.ReadLineAsync(_stopListening.Token);

                if (command == "QUIT")
                {
                    await writer.WriteLineAsync("BYE");
                    _ = Dispatcher.BeginInvoke(new Action(Close));
                    return;
                }

                string response = await ProcessCommandAsync(command);
                await writer.WriteLineAsync(response);
            }
        }
        catch (OperationCanceledException)
        {
            // Очікування завершено під час закриття програми
        }
        catch (IOException ex)
        {
            MessageBox.Show($"Помилка каналу: {ex.Message}");
        }
    }

    private async Task<string> ProcessCommandAsync(string? command)
    {
        if (command == "PING")
        {
            return "PONG";
        }

        if (command != null && command.StartsWith("MOVE|"))
        {
            string[] values = command.Split('|');

            if (values.Length != 5 ||
                !int.TryParse(values[1], out int x) ||
                !int.TryParse(values[2], out int y) ||
                !int.TryParse(values[3], out int width) ||
                !int.TryParse(values[4], out int height) ||
                width <= 0 || height <= 0)
            {
                return "ERROR:POSITION";
            }

            await Dispatcher.InvokeAsync(() =>
            {
                WindowState = WindowState.Normal;

                Left = x;
                Top = y;
                Width = width;
                Height = height;
            });

            return "OK";
        }

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