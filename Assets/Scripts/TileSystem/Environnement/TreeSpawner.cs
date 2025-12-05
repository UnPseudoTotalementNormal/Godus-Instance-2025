using System;
using System.Collections.Generic;
using UnityEngine;

namespace TileSystemSpace.Environnement
{
    public class TreeSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject treePrefab;

        [SerializeField] [Range(0, 100)] private float tileTreeSpawnChancePerTick;
        [SerializeField] private float tickTimeInterval = 1;
        
        private Dictionary<Tile, Vector2Int> grassTiles = new();
        
        private TileSystem tileSystem;

        private void Start()
        {
            tileSystem = TileSystem.instance;

            var _mapSize = tileSystem.GetSize();
            for (int x = 0; x < _mapSize.x; x++)
            {
                for (int y = 0; y < _mapSize.y; y++)
                {
                    var _tile = tileSystem.GetTile(new Vector2Int(x, y));
                    if (_tile != null && _tile.tileType == TileType.Grass)
                    {
                        grassTiles.Add(_tile, new Vector2Int(x, y));
                    }
                }
            }
            
            tileSystem.onAnyTileChanged += OnAnyTileChanged;
            
            InvokeRepeating(nameof(TickSpawnTree), 1f, tickTimeInterval);
        }

        private void TickSpawnTree()
        {
            if (grassTiles.Count == 0) return;

            float _expectedSpawns = grassTiles.Count * (tileTreeSpawnChancePerTick / 100f);
    
            int _guaranteedSpawns = Mathf.FloorToInt(_expectedSpawns);
            float _fractionalPart = _expectedSpawns - _guaranteedSpawns;
    
            int _totalSpawns = _guaranteedSpawns + (UnityEngine.Random.value < _fractionalPart ? 1 : 0);
    
            Debug.Log($"Expected tree spawns: {_expectedSpawns:F2}, Actual spawns: {_totalSpawns}");
    
            List<Tile> _tileList = new List<Tile>(grassTiles.Keys);
            for (int i = 0; i < _totalSpawns && _tileList.Count > 0; i++)
            {
                int _randomIndex = UnityEngine.Random.Range(0, _tileList.Count);
                Tile _selectedTile = _tileList[_randomIndex];
                Vector2Int _tilePosition = grassTiles[_selectedTile];
                
                Debug.Log("Spawning tree at: " + _tilePosition);
                Instantiate(treePrefab, 
                    new Vector3(_tilePosition.x, _tilePosition.y), 
                    Quaternion.identity);
        
                _tileList.RemoveAt(_randomIndex);
            }
        }


        private void OnAnyTileChanged(Tile _tile, Vector2Int _position)
        {
            if (!grassTiles.ContainsKey(_tile))
            {
                if (_tile.tileType == TileType.Grass)
                {
                    grassTiles.Add(_tile, _position);
                }
                return;
            }
            
            if (_tile.tileType != TileType.Grass)
            {
                grassTiles.Remove(_tile);
            }
        }
    }
}