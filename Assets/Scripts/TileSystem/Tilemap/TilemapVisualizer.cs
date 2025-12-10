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
        private Dictionary<RuleTile, Dictionary<int, CustomRuleTile>> ruleTiles;
        [SerializeField] private SerializedDictionary<TileType, CustomRuleTile> tileTypeToRuleTileMap;
        
        
        private void Awake()
        {
            tilemap = GetComponent<UnityEngine.Tilemaps.Tilemap>();
            Assert.IsNotNull(tilemap, "Tilemap is null");
            
            SetupRuleTiles();
        }

        private void SetupRuleTiles()
        {
            ruleTiles = new Dictionary<RuleTile, Dictionary<int, CustomRuleTile>>();
            
            foreach (TileType tileType in tileTypeToRuleTileMap.Keys)
            {
                CustomRuleTile _customRuleTile = tileTypeToRuleTileMap[tileType];
                
                for (int i = 0; i <= GameValues.MAX_TILE_HEIGHT; i++)
                {
                    if (!ruleTiles.ContainsKey(_customRuleTile))
                    {
                        ruleTiles[_customRuleTile] = new Dictionary<int, CustomRuleTile>();
                    }
                    
                    CustomRuleTile _tileVariant = Instantiate(_customRuleTile);
                    _tileVariant.heightLevel = i;
                    ruleTiles[_customRuleTile][i] = _tileVariant;
                }
            }
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
            tilemap.SetTile(_targetPos, ruleTiles[tileTypeToRuleTileMap[_tile.tileType]][_tile.level]);
        }
    }
}