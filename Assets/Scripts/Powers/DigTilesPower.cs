using System;
using System.Collections.Generic;
using TileSystemSpace;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Powers
{
    public class DigTilesPower : Power
    {
        private Vector2 mouseScreenPosition;
        private UnityEngine.Camera mainCamera;
        
        private bool isDiggingTiles = false;
        
        private Vector2Int lastDiggedTilePosition = new(int.MinValue, int.MinValue);
        
        private int tilesLeftToDig = 0;
        [SerializeField] private int maxTilesToDig = 30;
        
        private void Start()
        {
            mainCamera = UnityEngine.Camera.main;
            InputManager.instance.onMousePosition += GetMousePos;
        }

        public override void Activate()
        {
            base.Activate();
            InputManager.instance.onLeftMouseButtonPressStarted += TryStartDiggingTiles;
            InputManager.instance.onLeftMouseButtonPressCanceled += StopDiggingTiles;
            tilesLeftToDig = maxTilesToDig;
        }

        private void OnGUI()
        {
            if (!isPowerActive)
            {
                return;
            }
            GUIUtils.DisplayTextUnderMouse(mouseScreenPosition, tilesLeftToDig.ToString());
        }

        public override void Update()
        {
            base.Update();

            if (!isPowerActive)
            {
                return;
            }
            
            if (!isDiggingTiles)
            {
                return;
            }
            
            int _tilesDigged = TryDigTileAtMousePosition();
            if (_tilesDigged > 0)
            {
                tilesLeftToDig -= _tilesDigged;
                if (tilesLeftToDig <= 0)
                {
                    Deactivate();
                }
            }
        }

        private void TryStartDiggingTiles()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            isDiggingTiles = true;
        }

        private void StopDiggingTiles()
        {
            isDiggingTiles = false;
            lastDiggedTilePosition = new Vector2Int(int.MinValue, int.MinValue);
        }

        private void GetMousePos(Vector2 _position)
        {
            mouseScreenPosition = _position;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            StopDiggingTiles();
            InputManager.instance.onLeftMouseButtonPressStarted -= TryStartDiggingTiles;
            InputManager.instance.onLeftMouseButtonPressCanceled -= StopDiggingTiles;
        }

        private int TryDigTileAtMousePosition()
        {
            Vector2 _mouseWorldPosition = Vector2Int.RoundToInt(mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y))) 
                                          + GameValues.GRID_OFFSET;
            
            Vector2Int _mouseWorldPositionInt = new Vector2Int((int)_mouseWorldPosition.x, (int)_mouseWorldPosition.y);
            
            if (_mouseWorldPositionInt == lastDiggedTilePosition)
            {
                return 0;
            }
            
            lastDiggedTilePosition = _mouseWorldPositionInt;
            
            Dictionary<Tile, Vector2Int> _tilesInRadius = TileSystem.instance.GetAllTilesAtPointWithRadius(_mouseWorldPositionInt, tileRadius, radiusMode);
            
            int _tilesDiggedCount = 0;
            foreach (KeyValuePair<Tile, Vector2Int> _tileEntry in _tilesInRadius)
            {
                Tile _tile = _tileEntry.Key;
                
                int _currentHeight = _tile.level;
                if (_currentHeight > 0)
                {
                    _tilesDiggedCount++;
                    _tile.level -= 1;
                }
                
                if (tilesLeftToDig - _tilesDiggedCount <= 0)
                {
                    break;
                }
            }

            return _tilesDiggedCount;
        }
        
        public override bool ShouldStartCooldownOnDeactivate()
        {
            return tilesLeftToDig != maxTilesToDig;
        }
    }
}
