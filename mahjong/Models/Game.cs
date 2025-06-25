using mahjong.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public enum GameMoveResult
{
    NoMatch,
    Match,
    LevelWon,
    NoMovesLeft
}

namespace mahjong.Models
{
    public class Game
    {
        public Level Level { get; set; }
        public Tile[,,] LevelState { get; set; }
        public Tile[,] RenderState { get; set; }
        public Tile? SelectedTile { get; private set; }
        public bool IsTileSelected => SelectedTile != null;
        public int ShufflesRemaining { get; private set; } = 3;

        public Game(int level)
        {
            Level = new Level(level);
            LevelState = (Tile[,,])Level.Map.Clone();
            RenderState = buildRenderState();
        }

        public GameMoveResult SelectTile(Tile tile)
        {
            if (tile == null) return GameMoveResult.NoMatch;

            if (!IsTileFree(tile))
            {
                SelectedTile = null;
                return GameMoveResult.NoMatch;
            }

            if (!IsTileSelected)
            {
                SelectedTile = tile;
                return GameMoveResult.NoMatch;
            }
            else
            {
                if (SelectedTile.Id == tile.Id)
                {
                    SelectedTile = null;
                    return GameMoveResult.NoMatch;
                }

                if (AreTilesMatching(SelectedTile, tile))
                {
                    RemoveTileFromState(SelectedTile);
                    RemoveTileFromState(tile);
                    SelectedTile = null;
                    RenderState = buildRenderState();

                    if (IsGameWon())
                    {
                        return GameMoveResult.LevelWon;
                    }

                    if (!HasAnyValidMoves())
                    {
                        return GameMoveResult.NoMovesLeft;
                    }

                    return GameMoveResult.Match;
                }
                else
                {
                    SelectedTile = tile;
                    return GameMoveResult.NoMatch;
                }
            }
        }

        public bool ShuffleRemainingTiles()
        {
            if (ShufflesRemaining <= 0) return false;

            ShufflesRemaining--;

            var remainingTiles = new List<Tile>();
            var originalPositions = new List<(int l, int r, int t)>();

            for (int l = 0; l < LevelState.GetLength(0); l++)
                for (int r = 0; r < LevelState.GetLength(1); r++)
                    for (int t = 0; t < LevelState.GetLength(2); t++)
                    {
                        if (LevelState[l, r, t] != null)
                        {
                            remainingTiles.Add(LevelState[l, r, t]!);
                            originalPositions.Add((l, r, t));
                        }
                    }

            int maxAttempts = 100;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                var rnd = new Random();
                var shuffledTiles = remainingTiles.OrderBy(x => rnd.Next()).ToList();

                var tempState = new Tile[20, 20, 20];
                for (int i = 0; i < originalPositions.Count; i++)
                {
                    var pos = originalPositions[i];
                    tempState[pos.l, pos.r, pos.t] = shuffledTiles[i];
                }

                if (HasAnyValidMovesInState(tempState))
                {
                    LevelState = tempState;
                    RenderState = buildRenderState();
                    SelectedTile = null;
                    return true;
                }
            }

            ShufflesRemaining++;
            return false;
        }

        public bool HasAnyValidMoves()
        {
            return HasAnyValidMovesInState(this.LevelState);
        }

        public bool IsGameWon()
        {
            foreach (var tile in LevelState)
            {
                if (tile != null) return false;
            }
            return true;
        }

        private bool AreTilesMatching(Tile t1, Tile t2)
        {
            if (t1.Type == TileType.Season && t2.Type == TileType.Season) return true;
            if (t1.Type == TileType.Flower && t2.Type == TileType.Flower) return true;
            return t1.Type == t2.Type && t1.Value == t2.Value && t1.Dragon == t2.Dragon && t1.Wind == t2.Wind;
        }

        public bool IsTileFree(Tile tile)
        {
            return IsTileFreeInState(tile, this.LevelState);
        }

        private bool HasAnyValidMovesInState(Tile[,,] state)
        {
            var freeTiles = new List<Tile>();
            for (int l = 0; l < state.GetLength(0); l++)
                for (int r = 0; r < state.GetLength(1); r++)
                    for (int t = 0; t < state.GetLength(2); t++)
                    {
                        var tile = state[l, r, t];
                        if (tile != null && IsTileFreeInState(tile, state))
                        {
                            freeTiles.Add(tile);
                        }
                    }

            if (freeTiles.Count < 2) return false;

            for (int i = 0; i < freeTiles.Count; i++)
                for (int j = i + 1; j < freeTiles.Count; j++)
                {
                    if (AreTilesMatching(freeTiles[i], freeTiles[j]))
                    {
                        return true;
                    }
                }
            return false;
        }

        private bool IsTileFreeInState(Tile tile, Tile[,,] state)
        {
            var (l, r, t) = FindTileCoordinatesInState(tile, state);
            if (l == -1) return false;

            int layerAbove = l + 1;
            if (layerAbove < state.GetLength(0) && state[layerAbove, r, t] != null) return false;

            int columnLeft = t - 1;
            int columnRight = t + 1;
            bool isBlockedOnLeft = (columnLeft >= 0) && (state[l, r, columnLeft] != null);
            bool isBlockedOnRight = (columnRight < state.GetLength(2)) && (state[l, r, columnRight] != null);

            return !(isBlockedOnLeft && isBlockedOnRight);
        }

        private (int, int, int) FindTileCoordinates(Tile tile)
        {
            return FindTileCoordinatesInState(tile, this.LevelState);
        }

        private (int, int, int) FindTileCoordinatesInState(Tile tile, Tile[,,] state)
        {
            for (int l = 0; l < state.GetLength(0); l++)
                for (int r = 0; r < state.GetLength(1); r++)
                    for (int t = 0; t < state.GetLength(2); t++)
                        if (state[l, r, t]?.Id == tile.Id) return (l, r, t);
            return (-1, -1, -1);
        }

        private void RemoveTileFromState(Tile tile)
        {
            var (l, r, t) = FindTileCoordinates(tile);
            if (l != -1) LevelState[l, r, t] = null;
        }

        public Tile[,] buildRenderState()
        {
            Tile[,] renderState = new Tile[20, 20];
            for (int l = LevelState.GetLength(0) - 1; l >= 0; l--)
            {
                for (int r = 0; r < LevelState.GetLength(1); r++)
                {
                    for (int t = 0; t < LevelState.GetLength(2); t++)
                    {
                        if (renderState[r, t] == null && LevelState[l, r, t] != null)
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