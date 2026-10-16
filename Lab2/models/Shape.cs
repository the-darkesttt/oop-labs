using System.Windows;

namespace Lab2.Models;

// абстрактний клас (неможливо створити екземпляр, призначений лише для наслідування)
// далі override у різних дочірніх класів - поліморфізм
internal abstract class Shape
{
    protected Shape(Point start, Point end)
    {
        Start = start;
        End = end;
    }

    public Point Start { get; }
    public Point End { get; }

    public abstract UIElement CreateVisual(bool isPreview);
}