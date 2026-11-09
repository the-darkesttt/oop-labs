using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Lab5;

internal static class FigureStorage
{
    internal static void Save(string path, IReadOnlyList<FigureData> figures)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(
            figures,
            options
        );

        File.WriteAllText(path, json);
    }


    internal static List<FigureData> Load(string path)
    {
        string json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<List<FigureData>>(json)
            ?? throw new InvalidDataException(
                "Файл не містить коректного списку фігур.");
    }
}