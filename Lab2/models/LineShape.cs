using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab2.Models;

internal class LineShape : Shape
{
    public LineShape(Point start, Point end) : base(start, end)
    {
    }

    public override UIElement CreateVisual(bool isPreview)
    {
        var line = new Line
        {
            X1 = Start.X,
            Y1 = Start.Y,
            X2 = End.X,
            Y2 = End.Y,

            Stroke = isPreview ? Brushes.Red : Brushes.Black,
            StrokeThickness = 2,
            IsHitTestVisible = false
        };

        return line;
    }
}