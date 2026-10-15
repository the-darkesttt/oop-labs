using System.Windows;
using System.Windows.Controls;

namespace Lab1;

internal enum SecondStepAction
{
    Back,
    Confirm,
    Cancel
}

internal class SecondStepDialog : Window
{
    public SecondStepAction Action { get; private set; }
    = SecondStepAction.Cancel;
    public SecondStepDialog()
    {
        Title = "Робота2 - Друге вікно";
        Width = 430;
        Height = 180;

        var panel = new StackPanel
        {
            Margin = new Thickness(20),
        };
        Content = panel;
        var text = new TextBlock
        {
            Text = "Друге діалогове вікно",
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        panel.Children.Add(text);
        var buttonsPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 20, 0, 0)
        };
        panel.Children.Add(buttonsPanel);
        var backButton = new Button
        {
            Content = "< Назад",
            Width = 90
        };
        var okButton = new Button
        {
            Content = "Так",
            Width = 90
        };
        var cancelButton = new Button
        {
            Content = "Відміна",
            Width = 90
        };
        buttonsPanel.Children.Add(backButton);
        buttonsPanel.Children.Add(okButton);
        buttonsPanel.Children.Add(cancelButton);

        backButton.Click += (sender, e) =>
        {
            Action = SecondStepAction.Back;
            DialogResult = true;
        };
        okButton.Click += (sender, e) =>
        {
            Action = SecondStepAction.Confirm;
            DialogResult = true;
        };
        cancelButton.Click += (sender, e) =>
        {
            Action = SecondStepAction.Cancel;
            DialogResult = false;
        };
    }
}