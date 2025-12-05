using System;
using System.Collections.Generic;
using TileSystemSpace;
using UnityEngine;

namespace Powers
{
    public class DropTilesPower : Power
    {
        [SerializeField] private TileType tileTypeToDrop;
        
        private Vector2 mouseScreenPosition;
        private UnityEngine.Camera mainCamera;
        
        private void Start()
        {
            mainCamera = UnityEngine.Camera.main;
        }

        public override void Activate()
        {
            base.Activate();
            InputManager.instance.onLeftMouseButtonPressStarted += DropTileAtMousePosition;
            InputManager.instance.onMousePosition += GetMousePos;
        }

        private void GetMousePos(Vector2 _position)
        {
            mouseScreenPosition = _position;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            InputManager.instance.onLeftMouseButtonPressStarted -= DropTileAtMousePosition;
        }

        private void DropTileAtMousePosition()
        {
            Vector2 _mouseWorldPosition = Vector2Int.RoundToInt(mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y))) 
                                          + GameValues.GRID_OFFSET;
            
            Vector2Int _mouseWorldPositionInt = new Vector2Int((int)_mouseWorldPosition.x, (int)_mouseWorldPosition.y);
            
            Dictionary<Tile, Vector2Int> _tilesInRadius = TileSystem.instance.GetAllTilesAtPointWithRadius(_mouseWorldPositionInt, tileRadius, TileSystem.RadiusMode.Circle);
            
            foreach (KeyValuePair<Tile, Vector2Int> _tileEntry in _tilesInRadius)
            {
                Tile _tile = _tileEntry.Key;
                
                int _currentHeight = _tile.level;
                _tile.tileType = tileTypeToDrop;
                if (_currentHeight < GameValues.MAX_TILE_HEIGHT - 1)
                {
                    _tile.level += 1;
                }
            }
        }
    }
}
