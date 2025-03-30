using System.Diagnostics;

namespace mahjong.Models
{
    public class Game
    {
        public Level Level { get; set; }
        public Tile[,,] LevelState { get; set; }
        public Tile[,] RenderState { get; set; }

        public Game(int level) {
            Level = new Level(level);
            LevelState = Level.Map;
            RenderState = buildRenderState();
        }

        public Tile[,] buildRenderState()
        {
            Tile[,] renderState = new Tile[20,20];
            for (int l = 19; l > LevelState.GetLength(0)-21; l--)
            {
                for (int r = 0; r < LevelState.GetLength(1); r++)
                {
                    for (int t = 0; t < LevelState.GetLength(2); t++)
                    {
                        if (renderState[r,t] == null && LevelState[l, r, t] != null)
                        {
                            renderState[r, t] = LevelState[l, r, t];
                        }
                    }
                }
            }
            return renderState;
        }
    }
}