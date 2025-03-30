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


        public Tile(int id, TileType type, int value = 0)
        {
            this.Id = id;
            this.Type = type;
            this.Value = value;
        }

        public Tile(int id, TileType type, DragonType dragon) : this(id, type)
        {
            if (type == TileType.Dragon)
            {
                this.Dragon = dragon;
            }
        }

        public Tile(int id, TileType type, FlowerType flower) : this(id, type)
        {
            if (type == TileType.Flower)
            {
                this.Flower = flower;
            }
        }

        public Tile(int id, TileType type, SeasonType season) : this(id, type)
        {
            if (type == TileType.Season)
            {
                this.Season = season;
            }
        }

        public Tile(int id, TileType type, WindType wind) : this(id, type)
        {
            if (type == TileType.Wind)
            {
                this.Wind = wind;
            }
        }

        public string tileInfo()
        {
            return $"Tile: {Type}, Id: {Id}, Value: {Value}, D: {Dragon.ToString()}, F: {Flower.ToString()},S: {Season.ToString()},W: {Wind.ToString()} \n";
        }
    }
}
