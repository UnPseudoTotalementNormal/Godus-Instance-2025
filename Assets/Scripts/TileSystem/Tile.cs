using System;
using System.Collections.Generic;

namespace TileSystemSpace
{
    [Serializable]
    public class Tile
    {
        public event Action onTileChanged;
        private int levelField;
        private TileType bottomTileType;

        private Stack<TileType> tileTypeStack = new();
        
        public void AddTileOnTop(TileType _tileType, int _levelsToAdd = 1)
        {
            if (_levelsToAdd <= 0 || levelField >= GameValues.MAX_TILE_HEIGHT)
            {
                return;
            }
            
            _levelsToAdd = Math.Min(_levelsToAdd, GameValues.MAX_TILE_HEIGHT - levelField);
            
            for (int i = 0; i < _levelsToAdd; i++)
            {
                levelField++;
                tileTypeStack.Push(_tileType);
            }
            onTileChanged?.Invoke();
        }

        public void RemoveTileOnTop(int _levelsToRemove = 1)
        {
            if (_levelsToRemove <= 0 || levelField == 0)
            {
                return;
            }

            _levelsToRemove = Math.Min(_levelsToRemove, levelField);
            
            for (int i = 0; i < _levelsToRemove; i++)
            {
                tileTypeStack.Pop();
            }
            
            levelField -= _levelsToRemove;
            onTileChanged?.Invoke();
        }
        
        public TileType tileType
        {
            get => tileTypeStack.Count > 0 ? tileTypeStack.Peek() : bottomTileType;
            set
            {
                if (tileTypeStack.Count > 0)
                {
                    tileTypeStack.Pop();
                }
                tileTypeStack.Push(value);
                onTileChanged?.Invoke();
            }
        }

        public int level
        {
            get => levelField;
            set
            {
                if (value < 0) value = 0;
                if (value == levelField) return;
                
                if (value > levelField)
                {
                    int _levelsToAdd = value - levelField;
                    AddTileOnTop(tileType, _levelsToAdd);
                }
                else if (value < levelField)
                {
                    int _levelsToRemove = levelField - value;
                    RemoveTileOnTop(_levelsToRemove);
                }
            }
        }

        public Tile(int _level = 0, TileType _tileType = TileType.Grass, TileType _bottomTileType = TileType.Grass)
        {
            levelField = 0;
            bottomTileType = _bottomTileType;
            if (_level > 0)
            {
                AddTileOnTop(_tileType, _level);
            }
        }
    }
}