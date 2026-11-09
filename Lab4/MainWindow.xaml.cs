using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace Lab4;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DrawToolbarIcons();
        App.Editor.AttachCanvas(drawingCanvas);
        SelectTool(DrawingTool.Point);
        UpdateShapeCount();
    }

    private void SelectTool(DrawingTool tool)
    {
        App.Editor.SelectTool(tool);

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

        UpdateShapeCount();
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

        UpdateShapeCount();
    }

    private void UpdateShapeCount()
    {
        shapeCountText.Text = $"Фігур: {App.Editor.ShapeCount} / {MyEditor.MaxShapes}";
    }
}