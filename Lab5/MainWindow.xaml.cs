using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using System.Text.Json;

namespace Lab5;

public partial class MainWindow : Window
{
    private MyTableWindow? _tableWindow;
    public MainWindow()
    {
        InitializeComponent();
        DrawToolbarIcons();
        App.Editor.AttachCanvas(drawingCanvas);
        App.Editor.ShapesChanged += Editor_ShapesChanged;
        SelectTool(DrawingTool.Point);
        UpdateShapeCount();
    }

    private void Editor_ShapesChanged()
    {
        UpdateShapeCount();
        RefreshTable();
    }

    private void SelectTool(DrawingTool tool)
    {
        MyEditor.Instance.SelectTool(tool);

        pointMenuItem.IsChecked =
            tool == DrawingTool.Point;

        lineMenuItem.IsChecked =
            tool == DrawingTool.Line;

        rectangleMenuItem.IsChecked =
            tool == DrawingTool.Rectangle;

        ellipseMenuItem.IsChecked =
            tool == DrawingTool.Ellipse;

        lineWithCirclesMenuItem.IsChecked =
            tool == DrawingTool.LineWithCircles;

        cubeMenuItem.IsChecked =
            tool == DrawingTool.Cube;
    }

    private void OnNotify(object sender, RoutedEventArgs e)
    {
        FrameworkElement element = (FrameworkElement)sender;
        string toolName = (string)element.Tag;
        DrawingTool tool = Enum.Parse<DrawingTool>(toolName);

        SelectTool(tool);
    }

    // масив кортежів (tuple)
    private void DrawToolbarIcons()
    {
        var icons = new (Canvas canvas, DrawingTool tool, Point start, Point end)[]
        {
        (pointIconCanvas, DrawingTool.Point,
            new Point(20, 16), new Point(20, 16)),

        (lineIconCanvas, DrawingTool.Line,
            new Point(6, 24), new Point(34, 8)),

        (rectangleIconCanvas, DrawingTool.Rectangle,
            new Point(8, 8), new Point(32, 24)),

        (ellipseIconCanvas, DrawingTool.Ellipse,
            new Point(20, 16), new Point(32, 24)),

        (lineWithCirclesIconCanvas, DrawingTool.LineWithCircles,
            new Point(11, 16), new Point(29, 16)),

        (cubeIconCanvas, DrawingTool.Cube,
            new Point(8, 12), new Point(26, 30))
        };

        foreach (var (canvas, tool, start, end) in icons)
        {
            canvas.Children.Add(
                ShapeFactory.Create(tool, start, end).Show(false)
            );
        }
    }

    private void ToggleTable_Click(object sender, RoutedEventArgs e)
    {
        if (_tableWindow != null)
        {
            _tableWindow.Close();
            return;
        }

        var table = new MyTableWindow
        {
            Owner = this
        };

        table.RowSelected += id =>
            App.Editor.SelectFigure(id);

        table.DeleteRequested += id =>
            App.Editor.DeleteFigure(id);

        table.Closed += (_, _) =>
        {
            _tableWindow = null;
            tableMenuItem.IsChecked = false;

            App.Editor.SelectFigure(null);
        };

        _tableWindow = table;

        RefreshTable();

        table.Show();

        tableMenuItem.IsChecked = true;
    }

    private void RefreshTable()
    {
        if (_tableWindow == null)
        {
            return;
        }

        var rows = new List<TableRow>();

        foreach (FigureData figure in App.Editor.GetFigureData())
        {
            rows.Add(new TableRow
            {
                Id = figure.Id,

                Type = GetFigureName(figure.Type),

                X1 = figure.StartX,
                Y1 = figure.StartY,

                X2 = figure.EndX,
                Y2 = figure.EndY
            });
        }

        _tableWindow.SetRows(rows);
    }

    private static string GetFigureName(string type)
    {
        return type switch
        {
            "Point" => "Крапка",
            "Line" => "Лінія",
            "Rectangle" => "Прямокутник",
            "Ellipse" => "Еліпс",
            "LineWithCircles" => "Лінія з кружечками",
            "Cube" => "Каркас куба",
            _ => type
        };
    }
    private void ClearCanvas_Click(object sender, RoutedEventArgs e)
    {
        App.Editor.Clear();
    }
    private void SaveFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "JSON files (*.json)|*.json",
            DefaultExt = ".json",
            FileName = "figures.json"
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        try
        {
            FigureStorage.Save(
                dialog.FileName,
                App.Editor.GetFigureData()
            );
        }
        catch (Exception ex) when (
            ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(
                $"Не вдалося зберегти файл: {ex.Message}");
        }
    }
    private void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json"
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        try
        {
            List<FigureData> figures =
                FigureStorage.Load(dialog.FileName);

            App.Editor.LoadFigures(figures);
        }
        catch (Exception ex) when (
            ex is IOException or
                  UnauthorizedAccessException or
                  JsonException or
                  ArgumentException or
                  InvalidOperationException)
        {
            MessageBox.Show(
                $"Не вдалося відкрити файл: {ex.Message}");
        }
    }

    private void DrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        Point position = e.GetPosition(drawingCanvas);
        bool success = App.Editor.BeginDrawing(position);

        if (!success)
        {
            MessageBox.Show(
                $"Досягнуто максимальної кількості фігур ({MyEditor.MaxShapes})."
            );
        }
    }

    private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        Point position = e.GetPosition(drawingCanvas);
        App.Editor.UpdateDrawing(position);
    }

    private void DrawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        Point position = e.GetPosition(drawingCanvas);
        App.Editor.EndDrawing(position);
    }

    private void UpdateShapeCount()
    {
        shapeCountText.Text = $"Фігур: {App.Editor.ShapeCount} / {MyEditor.MaxShapes}";
    }
}