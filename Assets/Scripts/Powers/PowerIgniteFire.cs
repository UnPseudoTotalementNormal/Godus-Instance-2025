using System.Collections.Generic;
using Powers;
using TileSystemSpace;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

public class PowerIgniteFire : Power
{
    private Vector2 mouseScreenPosition;
        private UnityEngine.Camera mainCamera;
        
        private bool isIgnitingTiles = false;
        
        private Vector2Int lastIgnitedTilePosition = new(int.MinValue, int.MinValue);
        
        private int tilesLeftToIgnite = 0;
        [SerializeField] private int maxTilesToIgnite = 30;
        
        private void Start()
        {
            mainCamera = UnityEngine.Camera.main;
            InputManager.instance.onMousePosition += GetMousePos;
        }

        public override void Activate()
        {
            base.Activate();
            InputManager.instance.onLeftMouseButtonPressStarted += TryStartIgnitingTiles;
            InputManager.instance.onLeftMouseButtonPressCanceled += StopIgnitingTiles;
            tilesLeftToIgnite = maxTilesToIgnite;
        }

        private void OnGUI()
        {
            if (!isPowerActive)
            {
                return;
            }

            GUIUtils.DisplayTextUnderMouse(mouseScreenPosition, tilesLeftToIgnite.ToString());
        }

        public override void Update()
        {
            base.Update();

            if (!isPowerActive)
            {
                return;
            }
            
            if (!isIgnitingTiles)
            {
                return;
            }
            
            int _tilesIgnited = TryIgniteTileAtMousePosition();
            if (_tilesIgnited > 0)
            {
                tilesLeftToIgnite -= _tilesIgnited;
                if (tilesLeftToIgnite <= 0)
                {
                    Deactivate();
                }
            }
        }

        private void TryStartIgnitingTiles()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            isIgnitingTiles = true;
        }

        private void StopIgnitingTiles()
        {
            isIgnitingTiles = false;
            lastIgnitedTilePosition = new Vector2Int(int.MinValue, int.MinValue);
        }

        private void GetMousePos(Vector2 _position)
        {
            mouseScreenPosition = _position;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            StopIgnitingTiles();
            InputManager.instance.onLeftMouseButtonPressStarted -= TryStartIgnitingTiles;
            InputManager.instance.onLeftMouseButtonPressCanceled -= StopIgnitingTiles;
        }

        private int TryIgniteTileAtMousePosition()
        {
            Vector2 _mouseWorldPosition = Vector2Int.RoundToInt(mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y))) 
                                          + GameValues.GRID_OFFSET;
            
            Vector2Int _mouseWorldPositionInt = new Vector2Int((int)_mouseWorldPosition.x, (int)_mouseWorldPosition.y);
            
            if (_mouseWorldPositionInt == lastIgnitedTilePosition)
            {
                return 0;
            }
            
            lastIgnitedTilePosition = _mouseWorldPositionInt;
            
            Dictionary<Tile, Vector2Int> _tilesInRadius = TileSystem.instance.GetAllTilesAtPointWithRadius(_mouseWorldPositionInt, tileRadius, radiusMode);
            
            int _tilesIgnitedCount = 0;
            foreach (KeyValuePair<Tile, Vector2Int> _tileEntry in _tilesInRadius)
            {
                if (FireSystem.FireManager.instance.IsTileOnFire(_tileEntry.Value))
                {
                    continue;
                }
                
                FireSystem.FireManager.instance.IgniteTile(_tileEntry.Value);
                _tilesIgnitedCount++;
                
                if (tilesLeftToIgnite - _tilesIgnitedCount <= 0)
                {
                    break;
                }
            }

            return _tilesIgnitedCount;
        }
        
        public override bool ShouldStartCooldownOnDeactivate()
        {
            return tilesLeftToIgnite != maxTilesToIgnite;
        }
}
