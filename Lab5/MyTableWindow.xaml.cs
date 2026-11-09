using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Lab5;

public class TableRow
{
    public int Id { get; init; }

    public string Type { get; init; } = "";

    public double X1 { get; init; }
    public double Y1 { get; init; }

    public double X2 { get; init; }
    public double Y2 { get; init; }
}


public partial class MyTableWindow : Window
{
    private readonly ObservableCollection<TableRow> _rows = new();
    private bool _updating;
    public event Action<int?>? RowSelected;
    public event Action<int>? DeleteRequested;


    public MyTableWindow()
    {
        InitializeComponent();
        objectsGrid.ItemsSource = _rows;
    }


    public void SetRows(IEnumerable<TableRow> rows)
    {
        int? selectedId = (objectsGrid.SelectedItem as TableRow)?.Id;
        _updating = true;

        try
        {
            _rows.Clear();

            foreach (TableRow row in rows)
            {
                _rows.Add(row);
            }

            if (selectedId != null &&
                deleteModeCheckBox.IsChecked != true)
            {
                objectsGrid.SelectedItem = _rows
                    .FirstOrDefault(row => row.Id == selectedId);
            }
        }
        finally
        {
            _updating = false;
        }
    }


    private void ObjectsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_updating)
        {
            return;
        }

        if (objectsGrid.SelectedItem is not TableRow row)
        {
            RowSelected?.Invoke(null);
            return;
        }

        if (deleteModeCheckBox.IsChecked == true)
        {
            DeleteRequested?.Invoke(row.Id);
            objectsGrid.SelectedItem = null;
        }
        else
        {
            RowSelected?.Invoke(row.Id);
        }
    }


    private void DeleteMode_Checked(object sender, RoutedEventArgs e)
    {
        objectsGrid.SelectedItem = null;
        RowSelected?.Invoke(null);
    }
}