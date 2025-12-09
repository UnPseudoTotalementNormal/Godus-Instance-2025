using System;
using System.Collections.Generic;
using TileSystemSpace;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Powers
{
    public class DropTilesPower : Power
    {
        [SerializeField] private TileType tileTypeToDrop;
        
        private Vector2 mouseScreenPosition;
        private UnityEngine.Camera mainCamera;
        
        private bool isDroppingTiles = false;
        
        private Vector2Int lastDroppedTilePosition = new(int.MinValue, int.MinValue);
        
        private int tilesLeftToDrop = 0;
        [SerializeField] private int maxTilesToDrop = 30;
        
        private void Start()
        {
            mainCamera = UnityEngine.Camera.main;
            InputManager.instance.onMousePosition += GetMousePos;
        }

        public override void Activate()
        {
            base.Activate();
            InputManager.instance.onLeftMouseButtonPressStarted += TryStartDroppingTiles;
            InputManager.instance.onLeftMouseButtonPressCanceled += StopDroppingTiles;
            tilesLeftToDrop = maxTilesToDrop;
        }

        private void OnGUI()
        {
            if (!isPowerActive)
            {
                return;
            }

            GUIUtils.DisplayTextUnderMouse(mouseScreenPosition, tilesLeftToDrop.ToString());
        }

        public override void Update()
        {
            base.Update();

            if (!isPowerActive)
            {
                return;
            }
            
            if (!isDroppingTiles)
            {
                return;
            }
            
            int _tilesDropped = TryDropTileAtMousePosition();
            if (_tilesDropped > 0)
            {
                tilesLeftToDrop -= _tilesDropped;
                if (tilesLeftToDrop <= 0)
                {
                    Deactivate();
                }
            }
        }

        private void TryStartDroppingTiles()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            isDroppingTiles = true;
        }

        private void StopDroppingTiles()
        {
            isDroppingTiles = false;
            lastDroppedTilePosition = new Vector2Int(int.MinValue, int.MinValue);
        }

        private void GetMousePos(Vector2 _position)
        {
            mouseScreenPosition = _position;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            StopDroppingTiles();
            InputManager.instance.onLeftMouseButtonPressStarted -= TryStartDroppingTiles;
            InputManager.instance.onLeftMouseButtonPressCanceled -= StopDroppingTiles;
        }

        private int TryDropTileAtMousePosition()
        {
            Vector2 _mouseWorldPosition = Vector2Int.RoundToInt(mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y))) 
                                          + GameValues.GRID_OFFSET;
            
            Vector2Int _mouseWorldPositionInt = new Vector2Int((int)_mouseWorldPosition.x, (int)_mouseWorldPosition.y);
            
            if (_mouseWorldPositionInt == lastDroppedTilePosition)
            {
                return 0;
            }
            
            lastDroppedTilePosition = _mouseWorldPositionInt;
            
            Dictionary<Tile, Vector2Int> _tilesInRadius = TileSystem.instance.GetAllTilesAtPointWithRadius(_mouseWorldPositionInt, tileRadius, radiusMode);
            
            int _tilesDroppedCount = 0;
            foreach (KeyValuePair<Tile, Vector2Int> _tileEntry in _tilesInRadius)
            {
                Tile _tile = _tileEntry.Key;
                
                int _currentHeight = _tile.level;
                _tile.tileType = tileTypeToDrop;
                if (_currentHeight < GameValues.MAX_TILE_HEIGHT - 1)
                {
                    _tilesDroppedCount++;
                    _tile.level += 1;
                }
                
                if (tilesLeftToDrop - _tilesDroppedCount <= 0)
                {
                    break;
                }
            }

            return _tilesDroppedCount;
        }
        
        public override bool ShouldStartCooldownOnDeactivate()
        {
            return tilesLeftToDrop != maxTilesToDrop;
        }
    }
}
