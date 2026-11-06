using System.Windows;
using Lab4.Models;

namespace Lab4;

internal static class ShapeFactory
{
    public static Shape Create(
        DrawingTool tool,
        Point start,
        Point end)
    {
        return tool switch
        {
            DrawingTool.Point =>
                new PointShape(start),

            DrawingTool.Line =>
                new LineShape(start, end),

            DrawingTool.Rectangle =>
                new RectangleShape(start, end),

            DrawingTool.Ellipse =>
                new EllipseShape(start, end),

            DrawingTool.LineWithCircles =>
                new LineWithCirclesShape(start, end),

            DrawingTool.Cube =>
                new CubeShape(start, end),

            _ => throw new ArgumentOutOfRangeException(nameof(tool))
        };
    }
}