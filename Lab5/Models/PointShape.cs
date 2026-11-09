using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Lab5.Models;

internal class PointShape : Shape
{
    public PointShape(Point position) : base(position, position)
    {

    }

    public override UIElement CreateVisual(bool isPreview)
    {
        const double diameter = 6;

        var dot = new Ellipse
        {
            Width = diameter,
            Height = diameter,
            Fill = Brushes.Black,
            IsHitTestVisible = false
        };

        Canvas.SetLeft(dot, Start.X - diameter / 2);
        Canvas.SetTop(dot, Start.Y - diameter / 2);

        return dot;
    }
}