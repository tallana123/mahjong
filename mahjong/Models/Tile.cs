using mahjong.Enums;
using System;
using System.Diagnostics;

namespace mahjong.Models
{
    public class Tile
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public TileType Type { get; set; }
        public DragonType? Dragon { get; set; }
        public FlowerType? Flower { get; set; }
        public SeasonType? Season { get; set; }
        public WindType? Wind { get; set; }
        public string ImagePath { get; set; }
        public Tile(int id, TileType type, int value = 0, string imagePath = "") 
        {
            this.Id = id;
            this.Type = type;
            this.Value = value;
            this.ImagePath = imagePath; 
        }
        public Tile(int id, TileType type, DragonType dragon, string imagePath = "") : this(id, type, 0, imagePath) 
        {
            if (type == TileType.Dragon)
            {
                this.Dragon = dragon;
            }
        }

        public Tile(int id, TileType type, FlowerType flower, string imagePath = "") : this(id, type, 0, imagePath) 
        {
            if (type == TileType.Flower)
            {
                this.Flower = flower;
            }
        }

        public Tile(int id, TileType type, SeasonType season, string imagePath = "") : this(id, type, 0, imagePath) 
        {
            if (type == TileType.Season)
            {
                this.Season = season;
            }
        }

        public Tile(int id, TileType type, WindType wind, string imagePath = "") : this(id, type, 0, imagePath) 
        {
            if (type == TileType.Wind)
            {
                this.Wind = wind;
            }
        }

        public string tileInfo()
        {
            return $"Tile: {Type}, Id: {Id}, Value: {Value}, D: {Dragon}, F: {Flower}, S: {Season}, W: {Wind}, Img: {ImagePath}\n";
        }
    }
}