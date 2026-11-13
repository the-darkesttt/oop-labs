using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Object2;

public partial class MainWindow : Window
{
    private int[,]? _matrix;
    private readonly CancellationTokenSource _stopListening = new();

    public MainWindow()
    {
        InitializeComponent();

        _ = ListenAsync();
        Closed += (_, _) => _stopListening.Cancel();
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
        if (n <= 0 || min > max)
        {
            throw new ArgumentException("Некоректні параметри матриці.");
        }

        _matrix = CreateMatrix(n, min, max);
        DisplayMatrix(_matrix);

        try
        {
            Clipboard.SetText(MatrixToText(_matrix));
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

    private async Task ListenAsync()
    {
        try
        {
            while (!_stopListening.IsCancellationRequested)
            {
                using var pipe = new NamedPipeServerStream(
                    "Lab6_Object2",
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
            // очікування скасовано під час закриття програми
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

        if (command == null)
        {
            return "ERROR:EMPTY_COMMAND";
        }

        if (command.StartsWith("MOVE|"))
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

        string[] parts = command.Split('|');

        if (parts[0] != "GENERATE")
        {
            return "UNKNOWN_COMMAND";
        }

        if (parts.Length != 4 ||
            !int.TryParse(parts[1], out int n) ||
            !int.TryParse(parts[2], out int min) ||
            !int.TryParse(parts[3], out int max) ||
            n <= 0 || min > max)
        {
            return "ERROR:PARAMETERS";
        }

        bool success = await Dispatcher.InvokeAsync(
            () => GenerateAndShow(n, min, max)
        );

        return success ? "READY" : "ERROR:CLIPBOARD";
    }
}