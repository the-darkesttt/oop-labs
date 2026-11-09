using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Lab5.Models;

internal sealed class CubeShape : Shape
{
    public CubeShape(Point start, Point end)
        : base(start, end)
    {
    }

    public override UIElement CreateVisual(bool isPreview)
    {
        double width = Math.Abs(End.X - Start.X);
        double height = Math.Abs(End.Y - Start.Y);

        double side = Math.Max(width, height);

        double left = Math.Min(Start.X, End.X);
        double top = Math.Min(Start.Y, End.Y);

        double depth = side * 0.25;

        Point[] front =
        {
            new Point(left, top),
            new Point(left + side, top),
            new Point(left + side, top + side),
            new Point(left, top + side)
        };

        Point[] back = new Point[4];

        for (int i = 0; i < 4; i++)
        {
            back[i] = new Point(
                front[i].X + depth,
                front[i].Y - depth
            );
        }

        Canvas group = new Canvas
        {
            Width = Math.Max(1, left + side + depth + 2),
            Height = Math.Max(1, top + side + 2),
            IsHitTestVisible = false
        };

        RectangleShape backRectangle = new RectangleShape(back[0], back[2]);
        Rectangle backVisual = (Rectangle)backRectangle.Show(isPreview);
        backVisual.Fill = null;

        group.Children.Add(backVisual);

        //  з'єднувальні лінії
        for (int i = 0; i < 4; i++)
        {
            LineShape edge = new LineShape(
                front[i],
                back[i]
            );

            group.Children.Add(edge.Show(isPreview));
        }

        RectangleShape frontRectangle = new RectangleShape(front[0], front[2]);
        Rectangle frontVisual = (Rectangle)frontRectangle.Show(isPreview);
        frontVisual.Fill = null;

        group.Children.Add(frontVisual);

        return group;
    }
}