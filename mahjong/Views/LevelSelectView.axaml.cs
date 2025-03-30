using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using mahjong.Models;
using mahjong.Views;
using System;
using System.IO;

namespace mahjong.Views;

public partial class LevelSelectView : UserControl
{
    public LevelSelectView(MainWindow mainWindow)
    {
        InitializeComponent();
        var stackPanel = this.FindControl<StackPanel>("LevelsPanel");
        string path = Path.Combine(Directory.GetCurrentDirectory(), "levels");
        string[] filePaths = Directory.GetFiles(path, "*.json");
        string[] fileNamesWithoutExtension = new string[filePaths.Length];

        for (int i = 0; i < filePaths.Length; i++)
        {
            fileNamesWithoutExtension[i] = Path.GetFileNameWithoutExtension(filePaths[i]);
        }

        foreach (var level in fileNamesWithoutExtension)
        {
            var button = new Button { Content = $"Poziom {level}" };
            button.Click += (s, e) => mainWindow.NavigateTo(new GameView(mainWindow, int.Parse(level)));
            stackPanel.Children.Add(button);
        }

    }
}