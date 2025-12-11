using System.Collections.Generic;
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
            tilemap.SetTile(_targetPos, tileTypeToRuleTileMap[_tile.tileType]);
            
            tilemap.RefreshTile(new Vector3Int(_newPosition.x, _newPosition.y));
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    tilemap.RefreshTile(new Vector3Int(_newPosition.x, _newPosition.y) + new Vector3Int(x, y, 0));
                }
            }
            
        }
    }
}