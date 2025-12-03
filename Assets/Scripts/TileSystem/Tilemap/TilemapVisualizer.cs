using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace TileSystemSpace.Tilemap
{
    public class TilemapVisualizer : MonoBehaviour
    {
        [Header("Components")] 
        private UnityEngine.Tilemaps.Tilemap tilemap;

        private TileSystem tileSystem;
        [SerializeField] private CustomRuleTile _newTileTest;
        private Dictionary<RuleTile, Dictionary<int, RuleTile>> ruleTiles;
        
        
        private void Awake()
        {
            tilemap = GetComponent<UnityEngine.Tilemaps.Tilemap>();
            Assert.IsNotNull(tilemap, "Tilemap is null");
            
            SetupRuleTiles();
        }

        private void SetupRuleTiles()
        {
            ruleTiles = new Dictionary<RuleTile, Dictionary<int, RuleTile>>();
            
            for (int i = 0; i < GameValues.MAX_TILE_HEIGHT; i++)
            {
                if (!ruleTiles.ContainsKey(_newTileTest))
                {
                    ruleTiles[_newTileTest] = new Dictionary<int, RuleTile>();
                }
                
                CustomRuleTile _tileVariant = Instantiate(_newTileTest);
                _tileVariant.heightLevel = i;
                ruleTiles[_newTileTest][i] = _tileVariant;
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
            tilemap.SetTile(_targetPos, ruleTiles[_newTileTest][_tile.level]);
        }
    }
}