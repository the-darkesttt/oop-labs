using System.Windows;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;

namespace Lab6;

public partial class MainWindow : Window
{
    private bool _isShuttingDown;
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Run_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ParametersDialog
        {
            Owner = this
        };

        // поки користувач не ввів числа, головне вікно заблоковане
        if (dialog.ShowDialog() != true)
        {
            return;
        }

        int n = dialog.N;
        int min = dialog.MinValue;
        int max = dialog.MaxValue;

        try
        {
            statusText.Text = "Стан: запуск Object2...";

            await EnsureComponentAsync("Object2");

            statusText.Text = "Стан: запуск Object3...";

            await EnsureComponentAsync("Object3");

            statusText.Text = "Стан: розташування вікон...";

            await ArrangeWindowsAsync();

            statusText.Text = "Стан: генерація матриці...";

            string? response2 = await SendCommandAsync(
                "Lab6_Object2",
                $"GENERATE|{n}|{min}|{max}"
            );

            if (response2 != "READY")
            {
                statusText.Text = $"Помилка Object2: {response2 ?? "Немає відповіді"}";
                return;
            }

            statusText.Text = "Стан: обчислення визначника...";

            string? response3 = await SendCommandAsync(
                "Lab6_Object3",
                "CALCULATE"
            );

            if (response3 == "READY")
            {
                statusText.Text = "Готово! Визначник обчислено.";
            }
            else
            {
                statusText.Text =
                    $"Помилка Object3: {response3 ?? "Немає відповіді"}";
            }
        }
        catch (Exception ex) when (
            ex is IOException or
            TimeoutException or
            System.ComponentModel.Win32Exception)
        {
            statusText.Text = $"Помилка: {ex.Message}";
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private static async Task<string?> SendCommandAsync(string pipeName, string command)
    {
        using var pipe = new NamedPipeClientStream(
            ".",
            pipeName,
            PipeDirection.InOut,
            PipeOptions.Asynchronous
        );

        await pipe.ConnectAsync(2000);

        using var reader = new StreamReader(pipe);
        using var writer = new StreamWriter(pipe)
        {
            AutoFlush = true
        };

        await writer.WriteLineAsync(command);

        return await reader.ReadLineAsync();
    }

    private static string GetExecutablePath(string name)
    {
        // шлях до лаб 6
        var output = new DirectoryInfo(AppContext.BaseDirectory);

        string framework = output.Name;
        string configuration = output.Parent!.Name;

        string labFolder = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..")
        );

        return Path.Combine(
            labFolder,
            name,
            "bin",
            configuration,
            framework,
            $"{name}.exe"
        );
    }

    private static async Task EnsureComponentAsync(string name)
    {
        string pipeName = $"Lab6_{name}";

        // виконується тільки якщо процес ще не запущений, 
        // їх можна буде використовувати не відкриваючи повторно
        if (Process.GetProcessesByName(name).Length == 0)
        {
            string path = GetExecutablePath(name);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    $"Не знайдено виконуваний файл {name}.exe"
                );
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                WorkingDirectory = Path.GetDirectoryName(path)!,
                UseShellExecute = true
            });
        }

        for (int attempt = 0; attempt < 15; attempt++)
        {
            try
            {
                string? response = await SendCommandAsync(
                    pipeName,
                    "PING"
                );

                if (response == "PONG")
                {
                    return;
                }
            }
            catch (Exception ex) when (
                ex is IOException or TimeoutException)
            {
                // Компонент ще запускається
            }

            await Task.Delay(300);
        }

        throw new IOException($"{name} не відповідає на повідомлення.");
    }

    private async Task ArrangeWindowsAsync()
    {
        // повертає область монітору 
        Rect area = SystemParameters.WorkArea;
        int margin = (int)(area.Height * 0.1);

        int x = (int)area.Left;
        int y = (int)area.Top + margin;
        int width = (int)(area.Width / 3);
        int height = (int)area.Height - 2 * margin;

        string? response2 = await SendCommandAsync(
            "Lab6_Object2",
            $"MOVE|{x + width}|{y}|{width}|{height}"
        );

        string? response3 = await SendCommandAsync(
            "Lab6_Object3",
            $"MOVE|{x + width * 2}|{y}|{(int)area.Width - width * 2}|{height}"
        );

        if (response2 != "OK" || response3 != "OK")
        {
            throw new IOException(
                $"Object2: {response2 ?? "Немає відповіді"}, " +
                $"Object3: {response3 ?? "Немає відповіді"}"
            );
        }

        WindowState = WindowState.Normal;

        Left = x;
        Top = y;
        Width = width;
        Height = height;
    }

    private static async Task StopComponentAsync(string name)
    {
        try
        {
            string? response = await SendCommandAsync(
                $"Lab6_{name}",
                "QUIT"
            ).WaitAsync(TimeSpan.FromSeconds(3));

            if (response != "BYE")
            {
                Debug.WriteLine($"{name}: {response}");
            }
        }
        catch (Exception ex) when (
            ex is IOException or TimeoutException)
        {
            Debug.WriteLine($"{name}: {ex.Message}");
        }
    }

    private async void MainWindow_Closing(object? sender, CancelEventArgs e)
    {
        e.Cancel = true;

        // захист від подвійного натискання
        if (_isShuttingDown)
        {
            return;
        }

        _isShuttingDown = true;
        IsEnabled = false;

        try
        {
            await StopComponentAsync("Object2");
            await StopComponentAsync("Object3");
        }
        finally
        {
            Closing -= MainWindow_Closing;
            Close();
        }
    }
}