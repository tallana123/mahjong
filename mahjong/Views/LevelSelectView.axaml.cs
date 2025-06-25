using Avalonia.Controls;
using System.IO;

namespace mahjong.Views;

public partial class LevelSelectView : UserControl
{
    public LevelSelectView(MainWindow mainWindow)
    {
        InitializeComponent();
        var stackPanel = this.FindControl<StackPanel>("LevelsPanel");
        if (stackPanel == null) return;

        string path = Path.Combine(Directory.GetCurrentDirectory(), "levels");
        if (!Directory.Exists(path))
        {
            return;
        }

        string[] filePaths = Directory.GetFiles(path, "*.json");
        string[] fileNamesWithoutExtension = new string[filePaths.Length];

        for (int i = 0; i < filePaths.Length; i++)
        {
            fileNamesWithoutExtension[i] = Path.GetFileNameWithoutExtension(filePaths[i]);
        }

        foreach (var levelFileName in fileNamesWithoutExtension)
        {
            string buttonText;
            if (levelFileName == "0")
            {
                buttonText = "Tutorial";
            }
            else
            {
                buttonText = $"Poziom {levelFileName}";
            }

            var button = new Button { Content = buttonText };
            if (int.TryParse(levelFileName, out int levelNumber))
            {
                button.Click += (s, e) => mainWindow.NavigateTo(new GameView(mainWindow, levelNumber));
                stackPanel.Children.Add(button);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Ignorowanie pliku poziomu o nienumerycznej nazwie: {levelFileName}.json");
            }
        }
    }
}