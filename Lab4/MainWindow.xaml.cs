using System.Windows;
using System.Windows.Input;

namespace Lab4;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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