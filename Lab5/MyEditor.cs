using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Lab5.Models;

namespace Lab5;

internal sealed class MyEditor
{
    // Singleton Меєрса
    private MyEditor()
    {
    }

    private static class InstanceHolder
    {
        static InstanceHolder()
        {
        }

        internal static readonly MyEditor Value = new MyEditor();
    }

    internal static MyEditor Instance => InstanceHolder.Value;


    internal const int MaxShapes = 110;

    private sealed record FigureEntry(
        int Id,
        DrawingTool Tool,
        Shape Model,
        UIElement Visual
    );

    private readonly FigureEntry?[] _shapes = new FigureEntry?[MaxShapes];

    private int _shapeCount;
    private int _nextId = 1;

    private DrawingTool _currentTool = DrawingTool.Point;

    private Point? _dragStart;
    private UIElement? _previewShape;
    private Canvas? _canvas;

    private UIElement? _selectedVisual;
    private Effect? _originalEffect;

    internal int ShapeCount => _shapeCount;

    internal event Action? ShapesChanged;


    internal void AttachCanvas(Canvas canvas)
    {
        _canvas = canvas;
    }

    private Canvas GetCanvas()
    {
        return _canvas
            ?? throw new InvalidOperationException(
                "Полотно ще не підключене.");
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
                _currentTool,
                position,
                position
            );

            AddShape(point, _currentTool);
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

        _previewShape = shape.Show(true);

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

        if ((position - start).Length >= 2)
        {
            Shape shape = ShapeFactory.Create(
                _currentTool,
                start,
                position
            );

            AddShape(shape, _currentTool);
        }

        _dragStart = null;

        if (canvas.IsMouseCaptured)
        {
            canvas.ReleaseMouseCapture();
        }
    }

    private void AddShape(Shape shape, DrawingTool tool)
    {
        if (_shapeCount >= MaxShapes)
        {
            return;
        }

        UIElement visual = shape.Show(false);

        _shapes[_shapeCount] = new FigureEntry(
            _nextId++,
            tool,
            shape,
            visual
        );

        _shapeCount++;

        GetCanvas().Children.Add(visual);

        ShapesChanged?.Invoke();
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

    internal List<FigureData> GetFigureData()
    {
        var result = new List<FigureData>();

        for (int i = 0; i < _shapeCount; i++)
        {
            FigureEntry entry = _shapes[i]!;

            result.Add(new FigureData
            {
                Id = entry.Id,
                Type = entry.Tool.ToString(),

                StartX = entry.Model.Start.X,
                StartY = entry.Model.Start.Y,

                EndX = entry.Model.End.X,
                EndY = entry.Model.End.Y
            });
        }

        return result;
    }

    internal void SelectFigure(int? id)
    {
        if (_selectedVisual != null)
        {
            _selectedVisual.Effect = _originalEffect;
            _selectedVisual = null;
            _originalEffect = null;
        }

        if (id == null)
        {
            return;
        }

        for (int i = 0; i < _shapeCount; i++)
        {
            FigureEntry entry = _shapes[i]!;

            if (entry.Id == id)
            {
                _selectedVisual = entry.Visual;
                _originalEffect = entry.Visual.Effect;

                entry.Visual.Effect = new DropShadowEffect
                {
                    Color = (Color)ColorConverter.ConvertFromString("#8dda19"),
                    ShadowDepth = 0,
                    BlurRadius = 20,
                    Opacity = 1
                };

                break;
            }
        }
    }

    internal bool DeleteFigure(int id)
    {
        for (int i = 0; i < _shapeCount; i++)
        {
            FigureEntry entry = _shapes[i]!;

            if (entry.Id != id)
            {
                continue;
            }

            if (ReferenceEquals(_selectedVisual, entry.Visual))
            {
                SelectFigure(null);
            }

            GetCanvas().Children.Remove(entry.Visual);
            // зміщуємо всі елементи на один назад, щоб прибрати пустоту
            for (int j = i; j < _shapeCount - 1; j++)
            {
                _shapes[j] = _shapes[j + 1];
            }

            _shapeCount--;
            _shapes[_shapeCount] = null;

            ShapesChanged?.Invoke();

            return true;
        }

        return false;
    }

    internal void Clear()
    {
        CancelDrawing();
        SelectFigure(null);

        GetCanvas().Children.Clear();
        Array.Clear(_shapes, 0, _shapes.Length);

        _shapeCount = 0;
        _nextId = 1;

        ShapesChanged?.Invoke();
    }

    internal void LoadFigures(IReadOnlyList<FigureData> figures)
    {
        if (figures.Count > MaxShapes)
        {
            throw new InvalidDataException("У файлі забагато фігур.");
        }

        var prepared = new List<FigureEntry>();
        var usedIds = new HashSet<int>();

        foreach (FigureData data in figures)
        {
            if (data == null ||
                data.Id <= 0 ||
                data.Id >= int.MaxValue ||
                !usedIds.Add(data.Id))
            {
                throw new InvalidDataException("Некоректні ідентифікатори фігур.");
            }

            if (!Enum.TryParse(data.Type, out DrawingTool tool) ||
                !Enum.IsDefined(tool))
            {
                throw new InvalidDataException("Невідомий тип геометричної фігури.");
            }

            if (!double.IsFinite(data.StartX) ||
                !double.IsFinite(data.StartY) ||
                !double.IsFinite(data.EndX) ||
                !double.IsFinite(data.EndY))
            {
                throw new InvalidDataException("Некоректні координати фігури.");
            }

            Shape shape = ShapeFactory.Create(
                tool,
                new Point(data.StartX, data.StartY),
                new Point(data.EndX, data.EndY)
            );

            prepared.Add(new FigureEntry(
                data.Id,
                tool,
                shape,
                shape.Show(false)
            ));
        }

        // змінюємо полотно лише після перевірки всіх фігур
        CancelDrawing();
        SelectFigure(null);

        Canvas canvas = GetCanvas();
        canvas.Children.Clear();

        Array.Clear(_shapes, 0, _shapes.Length);
        _shapeCount = 0;
        _nextId = 1;

        foreach (FigureEntry entry in prepared)
        {
            _shapes[_shapeCount++] = entry;
            canvas.Children.Add(entry.Visual);

            _nextId = Math.Max(_nextId, entry.Id + 1);
        }

        ShapesChanged?.Invoke();
    }
}