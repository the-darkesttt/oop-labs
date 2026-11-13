using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Windows;

namespace Lab6.Shared;

internal sealed class PipeCommandServer
{
    private readonly Window _window;
    private readonly string _pipeName;
    private readonly Func<string, Task<string>> _handleCommand;

    private readonly CancellationTokenSource _stop = new();

    internal PipeCommandServer(
    Window window,
    string pipeName,
    Func<string, Task<string>> handleCommand)
    {
        _window = window;
        _pipeName = pipeName;
        _handleCommand = handleCommand;
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

            await _window.Dispatcher.InvokeAsync(() =>
            {
                _window.WindowState = WindowState.Normal;
                _window.Left = x;
                _window.Top = y;
                _window.Width = width;
                _window.Height = height;
            });

            return "OK";
        }

        return await _handleCommand(command);
    }

    internal async Task ListenAsync()
    {
        while (!_stop.IsCancellationRequested)
        {
            try
            {
                using var pipe = new NamedPipeServerStream(
                    _pipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous
                );

                await pipe.WaitForConnectionAsync(_stop.Token);

                using var reader = new StreamReader(pipe);

                using var writer = new StreamWriter(pipe)
                {
                    AutoFlush = true
                };

                string? command = await reader.ReadLineAsync(
                    _stop.Token
                );

                if (command == "QUIT")
                {
                    await writer.WriteLineAsync("BYE");

                    _ = _window.Dispatcher.BeginInvoke(
                        new Action(_window.Close)
                    );

                    return;
                }

                string response = await ProcessCommandAsync(command);

                await writer.WriteLineAsync(response);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (IOException ex)
            {
                Debug.WriteLine(
                    $"Помилка каналу {_pipeName}: {ex.Message}"
                );

                await Task.Delay(200);
            }
        }
    }

    internal void Stop()
    {
        _stop.Cancel();
    }
}