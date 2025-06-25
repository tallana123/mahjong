using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using mahjong.Models;
using mahjong.ViewModels;
using System;
using System.Diagnostics;

namespace mahjong.Views;

public partial class GameView : UserControl
{
    private readonly MainWindow _mainWindow;

    public GameView(MainWindow mainWindow, int level)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        DataContext = new GameViewModel(level);

        var levelTextControl = this.FindControl<TextBlock>("LevelText");
        if (levelTextControl != null)
        {
            levelTextControl.Text = $"Poziom {level}";
        }

        InitializeGrid();
        renderTiles();
    }

    private void InitializeGrid()
    {
        var gameGrid = this.FindControl<Grid>("GameGrid");
        if (gameGrid != null && gameGrid.ColumnDefinitions.Count == 0)
        {
            for (int i = 0; i < 20; i++)
            {
                gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                gameGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
        }
    }

    private void renderTiles()
    {
        var data = this.DataContext as GameViewModel;
        var gameGrid = this.FindControl<Grid>("GameGrid");
        if (data == null || gameGrid == null) return;

        gameGrid.Children.Clear();

        for (int y = 0; y < 20; y++)
        {
            for (int x = 0; x < 20; x++)
            {
                Tile currentTile = data.Game.RenderState[y, x];
                if (currentTile != null)
                {
                    var tileControl = new Border
                    {
                        BorderThickness = new Thickness(1),
                        Width = 50,
                        Height = 70,
                        CornerRadius = new CornerRadius(5),
                        Margin = new Thickness(1),
                        BoxShadow = new BoxShadows(new BoxShadow() { Color = Colors.Black, OffsetX = 2, OffsetY = 2, Blur = 3 }),
                        Tag = currentTile
                    };

                    tileControl.PointerPressed += Tile_PointerPressed;

                    if (!string.IsNullOrEmpty(currentTile.ImagePath))
                    {
                        try
                        {
                            var uri = new Uri($"avares://mahjong{currentTile.ImagePath}");
                            using (var stream = AssetLoader.Open(uri))
                            {
                                tileControl.Background = new ImageBrush
                                {
                                    Source = new Bitmap(stream),
                                    Stretch = Stretch.Fill
                                };
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"B³¹d ³adowania obrazka: {currentTile.ImagePath}. Szczegó³y: {ex.Message}");
                            tileControl.Background = Brushes.Red;
                            tileControl.Child = new TextBlock { Text = "!", Foreground = Brushes.White, FontWeight = FontWeight.Bold, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                        }
                    }

                    Grid.SetRow(tileControl, y);
                    Grid.SetColumn(tileControl, x);
                    gameGrid.Children.Add(tileControl);
                }
            }
        }
        UpdateSelectionVisuals();
    }

    private void Tile_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var data = this.DataContext as GameViewModel;
        if (sender is not Border clickedBorder || data == null) return;

        if (clickedBorder.Tag is Tile clickedTile)
        {
            var result = data.Game.SelectTile(clickedTile);

            switch (result)
            {
                case GameMoveResult.Match:
                    renderTiles();
                    break;
                case GameMoveResult.LevelWon:
                    renderTiles();
                    ShowEndScreen("Zwyciêstwo! Gratulacje!");
                    break;
                case GameMoveResult.NoMovesLeft:
                    renderTiles();
                    ShowEndScreen("Brak Dalszych Ruchów. Koniec Gry.");
                    break;
                case GameMoveResult.NoMatch:
                default:
                    UpdateSelectionVisuals();
                    break;
            }
        }
    }

    private void UpdateSelectionVisuals()
    {
        var data = this.DataContext as GameViewModel;
        var gameGrid = this.FindControl<Grid>("GameGrid");
        if (data == null || gameGrid == null) return;

        foreach (var child in gameGrid.Children)
        {
            if (child is Border border && border.Tag is Tile tile)
            {
                bool isFree = data.Game.IsTileFree(tile);
                bool isSelected = data.Game.SelectedTile?.Id == tile.Id;

                border.BorderBrush = (isSelected && isFree) ? Brushes.Yellow : Brushes.DarkGray;
                border.BorderThickness = (isSelected && isFree) ? new Thickness(3) : new Thickness(1);
                border.Opacity = isFree ? 1.0 : 0.6;
            }
        }
    }

    private void ShowEndScreen(string message)
    {
        var gameGrid = this.FindControl<Grid>("GameGrid");
        if (gameGrid == null) return;

        var endOverlay = new Border
        {
            Background = new SolidColorBrush(Colors.Black, 0.7),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        var mainPanel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        var endText = new TextBlock
        {
            Text = message,
            FontSize = 32,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.White,
            Margin = new Thickness(0, 0, 0, 20)
        };

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var backButton = new Button
        {
            Content = "Wróæ do menu",
            Padding = new Thickness(20),
            FontSize = 24,
            Margin = new Thickness(10)
        };
        backButton.Click += (s, e) => _mainWindow.NavigateTo(new MainMenuView(_mainWindow));
        buttonPanel.Children.Add(backButton);

        if (message.Contains("Brak Dalszych Ruchów"))
        {
            var shuffleButton = new Button
            {
                Content = "Potasuj",
                Padding = new Thickness(20),
                FontSize = 24,
                Margin = new Thickness(10),
                Background = Brushes.CornflowerBlue
            };
            shuffleButton.Click += ShuffleButton_Click;
            buttonPanel.Children.Insert(0, shuffleButton);
        }

        mainPanel.Children.Add(endText);
        mainPanel.Children.Add(buttonPanel);
        endOverlay.Child = mainPanel;

        Grid.SetRowSpan(endOverlay, 20);
        Grid.SetColumnSpan(endOverlay, 20);
        gameGrid.Children.Add(endOverlay);
    }

    private void ShuffleButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var data = this.DataContext as GameViewModel;
        if (data == null) return;

        bool success = data.Game.ShuffleRemainingTiles();

        if (success)
        {
            renderTiles();
        }
        else
        {
            ShowEndScreen("Koniec Przetasowañ. Koniec Gry.");
        }
    }
}