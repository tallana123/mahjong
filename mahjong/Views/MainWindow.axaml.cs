using Avalonia.Controls;

namespace mahjong.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ContentControl = this.FindControl<ContentControl>("MainContent");
        NavigateTo(new Views.MainMenuView(this));
    }

    public ContentControl ContentControl { get; }

    public void NavigateTo(UserControl page)
    {
        ContentControl.Content = page;
    }
}
