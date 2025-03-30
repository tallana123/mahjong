﻿using mahjong.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace mahjong.Models
{
    public class Level
    {
        public int Id { get; set; }
        public Tile[,,] Map {  get; set; }
        public Level(int level) {
            Id = level;
            Map = loadLevel(level);
        }

        public Tile[,,] loadLevel(int level)
        {
            string json = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "levels", $"{level}.json"));
            JsonElement levelData = JsonSerializer.Deserialize<JsonElement>(json);
            Tile[,,] localMap = new Tile[20, 20, 20];
            int layersCounter = 0;
            int tileIdCounter = 0;
            foreach (var layers in levelData.GetProperty("level").EnumerateArray()) {
                int rowsCounter = 0;
                int tilesCounter = 0;
                foreach (var rows in layers.EnumerateArray())
                {
                    tilesCounter = 0;
                    foreach (var tile in rows.EnumerateArray())
                    {
                        
                        if (!tile.TryGetProperty("type", out JsonElement j))
                        {
                            tilesCounter++;
                            continue;
                        }

                        TileType tileType = (TileType) Enum.Parse(typeof(TileType), tile.GetProperty("type").GetString());

                        if (tileType == TileType.Dot || tileType == TileType.Bamboo || tileType == TileType.Symbol)
                        {
                            localMap[layersCounter, rowsCounter, tilesCounter] = new(tileIdCounter, tileType, tile.GetProperty("value").GetInt16());
                        }
                        if (tileType == TileType.Dragon)
                        {
                            DragonType dragonType = (DragonType)Enum.Parse(typeof(DragonType), tile.GetProperty("value").GetString());
                            localMap[layersCounter, rowsCounter, tilesCounter] = new(tileIdCounter, tileType, dragonType);
                        }
                        if (tileType == TileType.Flower)
                        {
                            FlowerType flowerType = (FlowerType)Enum.Parse(typeof(FlowerType), tile.GetProperty("value").GetString());
                            localMap[layersCounter, rowsCounter, tilesCounter] = new(tileIdCounter, tileType, flowerType);
                        }
                        if (tileType == TileType.Season)
                        {
                            SeasonType seasonType = (SeasonType)Enum.Parse(typeof(SeasonType), tile.GetProperty("value").GetString());
                            localMap[layersCounter, rowsCounter, tilesCounter] = new(tileIdCounter, tileType, seasonType);
                        }
                        if (tileType == TileType.Wind)
                        {
                            WindType windType = (WindType)Enum.Parse(typeof(WindType), tile.GetProperty("value").GetString());
                            localMap[layersCounter, rowsCounter, tilesCounter] = new(tileIdCounter, tileType, windType);
                        }
                        tileIdCounter++;
                        tilesCounter++;
                    }
                    rowsCounter++;
                }
                layersCounter++;
            }
            return localMap;
        }
    }
}
