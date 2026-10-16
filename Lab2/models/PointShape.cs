using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab2.Models;

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
            Fill = isPreview ? Brushes.Red : Brushes.Black
        };

        Canvas.SetLeft(dot, Start.X - diameter / 2);
        Canvas.SetTop(dot, Start.Y - diameter / 2);

        return dot;
    }
}