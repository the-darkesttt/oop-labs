using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab2.Models;

internal class EllipseShape : Shape
{
    public EllipseShape(Point start, Point end)
        : base(start, end)
    {
    }

    public override UIElement CreateVisual(bool isPreview)
    {
        double left = Math.Min(Start.X, End.X);
        double top = Math.Min(Start.Y, End.Y);

        double width = Math.Abs(End.X - Start.X);
        double height = Math.Abs(End.Y - Start.Y);

        var ellipse = new Ellipse
        {
            Width = width,
            Height = height,
            Stroke = isPreview ? Brushes.Red : Brushes.Black,
            StrokeThickness = 2,
            Fill = isPreview ? null : Brushes.SkyBlue,
            IsHitTestVisible = false
        };

        Canvas.SetLeft(ellipse, left);
        Canvas.SetTop(ellipse, top);

        return ellipse;
    }
}