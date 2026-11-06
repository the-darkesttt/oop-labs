using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab4.Models;

internal class EllipseShape : Shape
{
    public EllipseShape(Point center, Point corner)
    : base(center, corner)
    {
    }

    public override UIElement CreateVisual(bool isPreview)
    {
        double halfWidth = Math.Abs(End.X - Start.X);
        double halfHeight = Math.Abs(End.Y - Start.Y);

        double width = halfWidth * 2;
        double height = halfHeight * 2;

        double left = Start.X - halfWidth;
        double top = Start.Y - halfHeight;

        var ellipse = new Ellipse
        {
            Width = width,
            Height = height,

            Stroke = isPreview ? Brushes.Blue : Brushes.Black,
            StrokeThickness = 2,

            StrokeDashArray = isPreview ? new DoubleCollection { 4, 3 } : null,

            Fill = null,
            IsHitTestVisible = false
        };

        Canvas.SetLeft(ellipse, left);
        Canvas.SetTop(ellipse, top);

        return ellipse;
    }
}