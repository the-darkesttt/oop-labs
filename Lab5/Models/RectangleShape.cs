using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab5.Models;

internal class RectangleShape : Shape
{
    public RectangleShape(Point firstCorner, Point oppositeCorner)
    : base(firstCorner, oppositeCorner)
    {
    }

    public override UIElement CreateVisual(bool isPreview)
    {
        double left = Math.Min(Start.X, End.X);
        double top = Math.Min(Start.Y, End.Y);

        double width = Math.Abs(End.X - Start.X);
        double height = Math.Abs(End.Y - Start.Y);

        var rectangle = new Rectangle
        {
            Width = width,
            Height = height,

            Stroke = isPreview ? Brushes.Blue : Brushes.Black,
            StrokeThickness = 2,

            StrokeDashArray = isPreview ? new DoubleCollection { 4, 3 } : null,

            Fill = isPreview ? null : Brushes.White,
            IsHitTestVisible = false
        };

        Canvas.SetLeft(rectangle, left);
        Canvas.SetTop(rectangle, top);

        return rectangle;
    }
}