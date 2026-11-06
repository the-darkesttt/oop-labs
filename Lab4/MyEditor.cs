using System.Windows;
using System.Windows.Controls;
using Lab4.Models;

namespace Lab4;

internal sealed class MyEditor
{
    internal const int MaxShapes = 110;
    private readonly Shape?[] _shapes = new Shape?[MaxShapes];
    private int _shapeCount = 0;
    private DrawingTool _currentTool = DrawingTool.Point;
    private Point? _dragStart;
    private UIElement? _previewShape;
    private Canvas? _canvas;

    internal int ShapeCount => _shapeCount;

    internal void AttachCanvas(Canvas canvas)
    {
        _canvas = canvas;
    }

    private Canvas GetCanvas()
    {
        return _canvas
            ?? throw new InvalidOperationException("Полотно ще не підключене.");
    }

    internal void SelectTool(DrawingTool tool)
    {
        CancelDrawing();
        _currentTool = tool;
    }

    internal bool BeginDrawing(Point position)
    {
        if (_shapeCount >= MaxShapes)
        {
            return false;
        }

        if (_currentTool == DrawingTool.Point)
        {
            Shape point = ShapeFactory.Create(
                DrawingTool.Point,
                position,
                position
            );
            AddShape(point);

            return true;
        }

        _dragStart = position;
        GetCanvas().CaptureMouse();

        return true;
    }

    internal void UpdateDrawing(Point position)
    {
        if (_dragStart is not Point start)
        {
            return;
        }

        Canvas canvas = GetCanvas();

        if (_previewShape != null)
        {
            canvas.Children.Remove(_previewShape);
        }

        Shape shape = ShapeFactory.Create(
            _currentTool,
            start,
            position
        );

        _previewShape = shape.CreateVisual(true);
        canvas.Children.Add(_previewShape);
    }

    internal void EndDrawing(Point position)
    {
        if (_dragStart is not Point start)
        {
            return;
        }

        Canvas canvas = GetCanvas();

        if (_previewShape != null)
        {
            canvas.Children.Remove(_previewShape);
            _previewShape = null;
        }

        double dx = Math.Abs(position.X - start.X);
        double dy = Math.Abs(position.Y - start.Y);

        if (dx >= 2 || dy >= 2)
        {
            Shape shape = ShapeFactory.Create(
                _currentTool,
                start,
                position
            );

            AddShape(shape);
        }

        _dragStart = null;

        if (canvas.IsMouseCaptured)
        {
            canvas.ReleaseMouseCapture();
        }
    }

    private void AddShape(Shape shape)
    {
        if (_shapeCount >= MaxShapes)
        {
            return;
        }

        _shapes[_shapeCount] = shape;
        _shapeCount++;

        GetCanvas().Children.Add(
            shape.CreateVisual(false)
        );
    }

    private void CancelDrawing()
    {
        Canvas canvas = GetCanvas();

        if (_previewShape != null)
        {
            canvas.Children.Remove(_previewShape);
            _previewShape = null;
        }

        _dragStart = null;

        if (canvas.IsMouseCaptured)
        {
            canvas.ReleaseMouseCapture();
        }
    }
}