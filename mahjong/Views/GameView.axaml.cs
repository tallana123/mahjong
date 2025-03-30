using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using mahjong.Models;
using mahjong.ViewModels;
using mahjong.Views;

namespace mahjong.Views;

public partial class GameView : UserControl
{
    public GameView(MainWindow mainWindow, int level)
    {
        InitializeComponent();
        DataContext = new GameViewModel(level);
        this.FindControl<TextBlock>("LevelText").Text = $"Poziom {level}";
        InitializeGrid();
        renderTiles();
        //this.FindControl<Button>("BackButton").Click += (s, e) =>
        //    mainWindow.NavigateTo(new LevelSelectView(mainWindow));
    }

    private void InitializeGrid()
    {
        for (int i = 0; i < 20; i++)
        {
            GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            GameGrid.RowDefinitions.Add(new RowDefinition());
        }
    }

    private void renderTiles()
    {
        var data = this.DataContext as GameViewModel;
        for (int y = 0; y < 20; y++)
        {
            for (int x = 0; x < 20; x++)
            {
                if (data.Game.RenderState[y, x] != null)
                {
                    var tile = new Border
                    {
                        BorderThickness = new Thickness(1),
                        Width = 80,
                        Height = 80,
                        BorderBrush = Brushes.White,
                        Background = Brushes.Black,  // Dodano t³o, ¿eby elementy by³y widoczniejsze
                        Padding = new Thickness(10),
                        Margin = new Thickness(2),
                        Child = new TextBlock
                        {
                            Text = $"{data.Game.RenderState[y, x].Type}",
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                        }
                    };
                    Grid.SetRow(tile, y);
                    Grid.SetColumn(tile, x);
                    GameGrid.Children.Add(tile);
                }
            }
        }
    }
}