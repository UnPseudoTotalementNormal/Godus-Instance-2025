using System;

namespace TileSystemSpace
{
    [System.Serializable]
    public class Tile
    {
        public event Action onTileChanged;
        private int levelField;

        public int level
        {
            get => levelField;
            set { levelField = value; onTileChanged?.Invoke(); }
        }

        public Tile( int _level=0)
        {
            levelField = _level;
        }
    }
}