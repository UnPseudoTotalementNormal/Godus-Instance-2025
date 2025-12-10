using System;
using System.Collections.Generic;
using System.Linq;
using TileSystemSpace;
using UnityEngine;

public class WaterTileSystem : MonoBehaviour
{
    private TileSystem tileSystem;
    
    private HashSet<WaterTileInfo> activeTiles = new();

    private Vector2Int[] tileNeighborsOffset =
    {
        new(-1, 0),
        new(1, 0),
        new(0, -1),
        new(0, 1)
    };
    
    [SerializeField] private float tickTimeInterval = 1f;
    private float timeSinceLastTick = 0f;

    private void Start()
    {
        tileSystem = TileSystem.instance;
        
        tileSystem.onAnyTileChanged += OnAnyTileChanged;
    }

    private void Update()
    {
        timeSinceLastTick += Time.deltaTime;
        if (timeSinceLastTick >= tickTimeInterval)
        {
            timeSinceLastTick = 0f;
            OnTick();
        }
    }

    private void OnTick()
    {
        List<WaterTileInfo> _oldActiveTiles = activeTiles.ToList();
        activeTiles.Clear();
        foreach (WaterTileInfo _currentWaterTileInfo in _oldActiveTiles)
        {
            Tile _currentWaterTile = _currentWaterTileInfo.tile;
            if (_currentWaterTile.tileType != TileType.Water)
            {
                continue;
            }

            foreach (Vector2Int _offset in tileNeighborsOffset)
            {
                Vector2Int _neighbourPosition = _currentWaterTileInfo.position + _offset;
                Tile _neighbourTile = tileSystem.GetTile(_neighbourPosition);
                if (_neighbourTile == null)
                {
                    continue;
                }
                
                if (_neighbourTile.level >= _currentWaterTile.level)
                {
                    continue;
                }

                if (_neighbourTile.tileType == TileType.Water)
                {
                    continue;
                }
                
                _neighbourTile.AddTileOnTop(TileType.Water);
                activeTiles.Add(new WaterTileInfo(_neighbourPosition, _neighbourTile));
            }
        }
    }

    private void OnAnyTileChanged(Tile _tile, Vector2Int _tilePos)
    {
        TrySetTileActive(_tilePos);
        foreach (Vector2Int _offset in tileNeighborsOffset)
        {
            TrySetTileActive(_tilePos + _offset);
        }
    }

    private void TrySetTileActive(Vector2Int _pos)
    {
        Tile _tile = tileSystem.GetTile(_pos);
        if (_tile == null) return;
        if (_tile.tileType != TileType.Water) return;

        activeTiles.Add(new WaterTileInfo(_pos, _tile));
    }

    [ContextMenu("Log water tiles count")]
    private void LogWaterTileCount()
    {
        Debug.Log(activeTiles.Count);
    }
    
    private class WaterTileInfo : IEquatable<WaterTileInfo>
    {
        public Vector2Int position;
        public Tile tile;
        
        public WaterTileInfo(Vector2Int _pos, Tile _tile)
        {
            position = _pos;
            tile = _tile;
        }

        public bool Equals(WaterTileInfo _other)
        {
            if (_other is null) return false;
            if (ReferenceEquals(this, _other)) return true;
            return position.Equals(_other.position) && Equals(tile, _other.tile);
        }

        public override bool Equals(object _obj)
        {
            if (_obj is null) return false;
            if (ReferenceEquals(this, _obj)) return true;
            if (_obj.GetType() != GetType()) return false;
            return Equals((WaterTileInfo)_obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(position, tile);
        }
    }
}
