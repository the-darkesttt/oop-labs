using System.Windows;
using System.Windows.Controls;
namespace Lab1;

internal class SliderDialog : Window
{
    public int SelectedNumber { get; private set; } = 1;
    public SliderDialog()
    {
        Title = "Робота 1 - вибір числа";
        Width = 450;
        Height = 250;

        var panel = new StackPanel
        {
            Margin = new Thickness(20)
        };
        Content = panel;

        var message = new TextBlock
        {
            Text = "Оберіть число від 1 до 100"
        };
        panel.Children.Add(message);

        var numberSlider = new Slider
        {
            Minimum = 1,
            Maximum = 100,
            Value = 1,
            TickFrequency = 1,
            IsSnapToTickEnabled = true
        };

        panel.Children.Add(numberSlider);

        var valueText = new TextBlock
        {
            Text = "1"
        };

        panel.Children.Add(valueText);

        numberSlider.ValueChanged += (sender, e) =>
        {
            valueText.Text = numberSlider.Value.ToString("0");
        };

        var buttonsPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 20, 0, 0)
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
        buttonsPanel.Children.Add(okButton);
        buttonsPanel.Children.Add(cancelButton);

        panel.Children.Add(buttonsPanel);

        okButton.Click += (sender, e) =>
        {
            SelectedNumber = (int)numberSlider.Value;
            DialogResult = true;
        };
        cancelButton.Click += (sender, e) =>
        {
            DialogResult = false;
        };
    }
}