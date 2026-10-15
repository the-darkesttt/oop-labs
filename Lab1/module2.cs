using System.Windows;
using System.Windows.Controls;

namespace Lab1;

internal class FirstStepDialog : Window
{
    public FirstStepDialog()
    {
        Title = "Робота2 - Перше вікно";
        Width = 380;
        Height = 180;

        var panel = new StackPanel
        {
            Margin = new Thickness(20)
        };
        Content = panel;
        var text = new TextBlock
        {
            Text = "Перше діалогове вікно",
            FontSize = 20,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        panel.Children.Add(text);

        var buttonsPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 20, 0, 0)
        };

        var nextButton = new Button
        {
            Content = "Далі >",
            Width = 90
        };
        var cancelButton = new Button
        {
            Content = "Відміна",
            Width = 90
        };
        buttonsPanel.Children.Add(nextButton);
        buttonsPanel.Children.Add(cancelButton);

        panel.Children.Add(buttonsPanel);

        nextButton.Click += (sender, e) =>
        {
            DialogResult = true;
        };
        cancelButton.Click += (sender, e) =>
        {
            DialogResult = false;
        };

    }
}