using System.Windows;
using System.Windows.Input;
using Lab2.Models;

namespace Lab2;

public partial class MainWindow : Window
{
    private DrawingTool _currentTool = DrawingTool.Point;
    private Point? _dragStart;
    private UIElement? _previewShape;

    // не більше 109 ел в масиві
    private const int MaxShapes = 109;
    private readonly Shape?[] _shapes = new Shape?[MaxShapes];
    private int _shapeCount = 0;

    private void SelectTool(DrawingTool tool)
    {
        _currentTool = tool;
        // повертає значення, не виконує код
        string toolName = tool switch
        {
            DrawingTool.Point => "Крапка",
            DrawingTool.Line => "Лінія",
            DrawingTool.Rectangle => "Прямокутник",
            DrawingTool.Ellipse => "Еліпс",
            _ => "Невідомий інструмент"
        };

        Title = $"Lab2 - {toolName}";
    }
    private void DrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_shapeCount >= MaxShapes)
        {
            MessageBox.Show(
                $"Полотно заповнене. Максимальна кількість фігур — {MaxShapes}.",
                "Обмеження",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
            return;
        }
        Point position = e.GetPosition(drawingCanvas);
        if (_currentTool == DrawingTool.Point)
        {
            Shape point = new PointShape(position);
            AddShape(point);
            return;
        }

        if (_currentTool == DrawingTool.Line ||
            _currentTool == DrawingTool.Rectangle ||
            _currentTool == DrawingTool.Ellipse)
        {
            _dragStart = position;
            drawingCanvas.CaptureMouse();
        }
    }

    private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (_dragStart is not Point start)
        {
            return;
        }

        Point end = e.GetPosition(drawingCanvas);
        Shape? shape = CreateDraggedShape(start, end);

        if (shape == null)
        {
            return;
        }

        if (_previewShape != null)
        {
            drawingCanvas.Children.Remove(_previewShape);
        }

        _previewShape = shape.CreateVisual(true);
        drawingCanvas.Children.Add(_previewShape);
    }

    private void DrawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_dragStart is not Point start)
        {
            return;
        }
        Point end = e.GetPosition(drawingCanvas);

        if (_previewShape != null)
        {
            drawingCanvas.Children.Remove(_previewShape);
            _previewShape = null;
        }

        if ((end - start).Length >= 2)
        {
            Shape? shape = CreateDraggedShape(start, end);
            if (shape != null)
            {
                AddShape(shape);
            }
        }
        _dragStart = null;
        drawingCanvas.ReleaseMouseCapture();
    }
    private Shape? CreateDraggedShape(Point start, Point end)
    {
        return _currentTool switch
        {
            DrawingTool.Line => new LineShape(start, end),
            DrawingTool.Rectangle => new RectangleShape(start, end),
            DrawingTool.Ellipse => new EllipseShape(start, end),
            _ => null
        };
    }

    private void AddShape(Shape shape)
    {
        if (_shapeCount >= MaxShapes)
        {
            MessageBox.Show(
                "Досягнуто максимальної кількості фігур (109).",
                "Обмеження",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
            return;
        }

        _shapes[_shapeCount] = shape;
        _shapeCount++;
        drawingCanvas.Children.Add(shape.CreateVisual(false));

        UpdateShapeCount();
    }

    private void UpdateShapeCount()
    {
        shapeCountText.Text = $"Фігур: {_shapeCount} / {MaxShapes}";
    }
    public MainWindow()
    {
        InitializeComponent();
        SelectTool(DrawingTool.Point);
        UpdateShapeCount();
    }

    private void Point_Click(object sender, RoutedEventArgs e)
    {
        SelectTool(DrawingTool.Point);
    }
    private void Line_Click(object sender, RoutedEventArgs e)
    {
        SelectTool(DrawingTool.Line);
    }
    private void Rectangle_Click(object sender, RoutedEventArgs e)
    {
        SelectTool(DrawingTool.Rectangle);
    }
    private void Ellipse_Click(object sender, RoutedEventArgs e)
    {
        SelectTool(DrawingTool.Ellipse);
    }
}