using System;
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
        
        private void Awake()
        {
            tilemap = GetComponent<UnityEngine.Tilemaps.Tilemap>();
            Assert.IsNotNull(tilemap, "Tilemap is null");
        }

        private void Start()
        {
            tileSystem = TileSystem.instance;
            tileSystem.onAnyTileChanged += TileSystem_onAnyTileChanged;
        }
        
        private void TileSystem_onAnyTileChanged(Tile _tile, Vector2Int _position)
        {

        }
    }
}