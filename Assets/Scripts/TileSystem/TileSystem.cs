using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TileSystemSpace
{
    public class TileSystem : MonoBehaviour
    {
         public static TileSystem instance;    
        
        [Header("Map size")]
        [SerializeField] int xSize = 50;
        [SerializeField] int ySize = 50;
        
        List<Tile> tiles = new();

        
        [Header("Island")] 
        [SerializeField] int yCenter = 25;
        [SerializeField] int xCenter = 25;
        [SerializeField] int radius = 20;
        List<int> edgePoints = new();
        public Vector2Int GetSize() => new(xSize, ySize);
        
        public event Action<Tile, Vector2Int> onAnyTileChanged;
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            for (int _i = 0; _i < 360; _i++) //  Generates a circle as an island
            {
                float _angle = _i * Mathf.Deg2Rad;
                float _x1 = xCenter + radius * Mathf.Cos(_angle);
                float _y1 = yCenter + radius * Mathf.Sin(_angle);
                int _calcIndex = Mathf.RoundToInt(_y1) * xSize + Mathf.RoundToInt(_x1);
                edgePoints.Add(_calcIndex);
            }
            
            for (int _x = 0; _x < xSize * ySize; _x++)
            {
                
                float _dx = GetPositionFromIndex(_x).x- xCenter;
                float _dy = GetPositionFromIndex(_x).y- yCenter;

                Tile _newTile;
                if (edgePoints.Contains(_x))
                {
                    _newTile = new Tile(1);
                    tiles.Add(_newTile);
                }
                else if (_dx * _dx + _dy * _dy <= radius * radius) // Tries to see if current tile is in the generated circle to fill the island
                {
                    var range = Random.Range(1, 4);
                    _newTile = new Tile(range);
                    tiles.Add(_newTile);
                }
                else
                {
                    _newTile = new Tile(0, TileType.Water);
                    tiles.Add(_newTile);
                }
                
                var _x1 = _x;
                _newTile.onTileChanged += () =>
                {
                    onAnyTileChanged?.Invoke(_newTile, GetPositionFromIndex(_x1));
                };
            }
        }
        
        public Tile GetTile(Vector2Int _tilePos)
        {
            if(_tilePos.x <0 || _tilePos.y < 0)
            {
                return null;
            }
            int _calcIndex = _tilePos.y * xSize + _tilePos.x;
            return tiles.ElementAtOrDefault(_calcIndex);
        }

        public Vector2Int GetPositionFromIndex(int _index)
        {
            return new Vector2Int(_index % xSize, _index / xSize);
        }

        public Vector2Int GetGridSize()
        {
            return new Vector2Int(xSize, ySize);
        }

        void OnDrawGizmos() // Full debug to see what the generated map looks like even without any assets
        {
            for (int _i = 0; _i < tiles.Count; _i++)
            {
                Gizmos.color = Color.lightBlue;
                if (tiles[_i].level == 1)
                    Gizmos.color = Color.yellow;
                if (tiles[_i].level == 2)
                    Gizmos.color = Color.green;
                if (tiles[_i].level == 3)
                    Gizmos.color = Color.brown;
                
                Vector2Int _tilePos = GetPositionFromIndex(_i);
                
                Gizmos.DrawCube(new Vector3(_tilePos.x, _tilePos.y), Vector3.one);
            }  
        }

        public Tile GetTile(int x, int y)
        {
            return GetTile(new Vector2Int(x, y));
        }
    }
}