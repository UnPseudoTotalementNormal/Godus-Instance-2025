using System;

namespace TileSystemSpace
{
    [System.Serializable]
    public class Tile
    {
        public event Action onTileChanged;
        private int levelField;
        private TileType tileTypeField;

        public void AddTileOnTop(TileType _tileType)
        {
            levelField++;
            tileTypeField = _tileType;
            onTileChanged?.Invoke();
        }

        public int level
        {
            get => levelField;
            set { levelField = value; onTileChanged?.Invoke(); }
        }

        public TileType tileType
        {
            get => tileTypeField;
            set { tileTypeField = value; onTileChanged?.Invoke(); }
        }

        public Tile(int _level = 0, TileType _tileType = TileType.Grass)
        {
            levelField = _level;
            tileTypeField = _tileType;
        }
    }
}