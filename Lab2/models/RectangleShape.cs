using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab2.Models;

internal class RectangleShape : Shape
{
    public RectangleShape(Point center, Point corner)
        : base(center, corner)
    {
    }

    public override UIElement CreateVisual(bool isPreview)
    {
        double halfWidth = Math.Abs(End.X - Start.X);
        double halfHeight = Math.Abs(End.Y - Start.Y);

        var rectangle = new Rectangle
        {
            Width = halfWidth * 2,
            Height = halfHeight * 2,
            Stroke = isPreview ? Brushes.Red : Brushes.Black,
            StrokeThickness = 2,
            Fill = null,
            IsHitTestVisible = false
        };

        Canvas.SetLeft(rectangle, Start.X - halfWidth);
        Canvas.SetTop(rectangle, Start.Y - halfHeight);

        return rectangle;
    }
}