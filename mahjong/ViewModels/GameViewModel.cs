using Avalonia.Controls;
using mahjong.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace mahjong.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        public Game Game { get; set; }
        public GameViewModel(int level) {
            Game = new Game(level);
        }

    }
}
