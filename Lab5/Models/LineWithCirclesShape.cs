using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab5.Models;

internal sealed class LineWithCirclesShape : Shape
{
    private const double Radius = 7;
    public LineWithCirclesShape(Point start, Point end)
        : base(start, end)
    {
    }

    public override UIElement CreateVisual(bool isPreview)
    {
        Canvas group = new Canvas
        {
            Width = Math.Max(1, Math.Max(Start.X, End.X) + Radius + 2),
            Height = Math.Max(1, Math.Max(Start.Y, End.Y) + Radius + 2),
            IsHitTestVisible = false
        };

        LineShape line = new LineShape(Start, End);
        group.Children.Add(line.Show(isPreview));

        AddCircle(group, Start, isPreview);
        AddCircle(group, End, isPreview);

        return group;
    }

    private static void AddCircle(
        Canvas group,
        Point center,
        bool isPreview)
    {
        Point corner = new Point(
            center.X + Radius,
            center.Y + Radius
        );

        EllipseShape ellipse = new EllipseShape(center, corner);
        Ellipse visual = (Ellipse)ellipse.Show(isPreview);

        visual.Fill = isPreview ? null : Brushes.LightBlue;

        group.Children.Add(visual);
    }
}