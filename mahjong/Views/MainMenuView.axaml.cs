using Avalonia.Controls;

namespace mahjong.Views;

public partial class MainMenuView : UserControl
{
    public MainMenuView(MainWindow mainWindow)
    {
        InitializeComponent();
        this.FindControl<Button>("SelectLevelButton").Click += (s, e) =>
            mainWindow.NavigateTo(new LevelSelectView(mainWindow));
    }
}