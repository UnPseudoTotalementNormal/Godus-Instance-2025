using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace TileSystemSpace.Tilemap
{
    public class TilemapVisualizer : MonoBehaviour
    {
        [Header("Components")] 
        private UnityEngine.Tilemaps.Tilemap tilemap;

        private TileSystem tileSystem;
        [SerializeField] private SerializedDictionary<TileType, CustomRuleTile> tileTypeToRuleTileMap;

        [SerializeField] private SerializedDictionary<int, CustomRuleTile> waterDistanceRuleTiles;
        
        
        private void Awake()
        {
            tilemap = GetComponent<UnityEngine.Tilemaps.Tilemap>();
            Assert.IsNotNull(tilemap, "Tilemap is null");
        }

        private void Start()
        {
            tileSystem = TileSystem.instance;
            tileSystem.onAnyTileChanged += AnyTileChanged;

            for (int x = 0; x < tileSystem.GetSize().x; x++)
            {
                for (int y = 0; y < tileSystem.GetSize().y; y++)
                {
                    AnyTileChanged(tileSystem.GetTile(x, y), new Vector2Int(x, y));
                }
            }
        }
        
        private void AnyTileChanged(Tile _tile, Vector2Int _newPosition)
        {
            Vector3Int _targetPos = new Vector3Int(_newPosition.x, _newPosition.y, 0);
            CustomRuleTile _tileTypeToRuleTile = tileTypeToRuleTileMap[_tile.tileType];
            if (_tile.tileType == TileType.Water)
            {
                _tileTypeToRuleTile = GetWaterRuleTile(_newPosition);
            }
            tilemap.SetTile(_targetPos, _tileTypeToRuleTile);
            
            tilemap.RefreshTile(new Vector3Int(_newPosition.x, _newPosition.y));
            int _updateDistance = 2;
            for (int x = -_updateDistance; x <= _updateDistance; x++)
            {
                for (int y = -_updateDistance; y <= _updateDistance; y++)
                {
                    Vector2Int _neighbourPos = _newPosition + new Vector2Int(x, y);
                    Vector3Int _neighbourPosVector3 = new Vector3Int(_neighbourPos.x, _neighbourPos.y);
                    Tile _neighbourTile = tileSystem.GetTile(_neighbourPos);
                    if (_neighbourTile != null && _neighbourTile.tileType == TileType.Water)
                    {
                        tilemap.SetTile(_neighbourPosVector3, GetWaterRuleTile(_neighbourPos));
                    }
                    tilemap.RefreshTile(_neighbourPosVector3);
                }
            }
        }

        private CustomRuleTile GetWaterRuleTile(Vector2Int _waterPosition)
        {
            int _distanceToNonWater = 0;
            int _maxDistance = waterDistanceRuleTiles.Keys.Max(_k => _k);
            
            for (int _distance = 1; _distance <= _maxDistance; _distance++)
            {
                bool _foundNonWater = false;
                for (int x = -_distance; x <= _distance; x++)
                {
                    for (int y = -_distance; y <= _distance; y++)
                    {
                        if (Mathf.Abs(x) != _distance && Mathf.Abs(y) != _distance)
                        {
                            continue;
                        }

                        Vector2Int _checkPos = _waterPosition + new Vector2Int(x, y);
                        Tile _checkTile = tileSystem.GetTile(_checkPos);
                        if (_checkTile != null && _checkTile.tileType != TileType.Water)
                        {
                            _foundNonWater = true;
                            break;
                        }
                    }
                    if (_foundNonWater)
                    {
                        break;
                    }
                }

                if (_foundNonWater)
                {
                    _distanceToNonWater = _distance;
                    break;
                }
            }

            if (!waterDistanceRuleTiles.ContainsKey(_distanceToNonWater))
            {
                return tileTypeToRuleTileMap[TileType.Water];
            }

            return waterDistanceRuleTiles[_distanceToNonWater];
        }
    }
}