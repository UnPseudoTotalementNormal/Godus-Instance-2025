using System.Collections.Generic;
using TileSystemSpace;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Powers
{
    public class DrainWaterPower : Power
    {
        private Vector2 mouseScreenPosition;
        private UnityEngine.Camera mainCamera;
        
        private bool isDrainingWater;
        
        private Vector2Int lastDrainedTilePosition = new(int.MinValue, int.MinValue);
        
        private int waterLayersLeftToDrain;
        [SerializeField] private int maxWaterLayersToDrain = 50;
        
        private void Start()
        {
            mainCamera = UnityEngine.Camera.main;
            InputManager.instance.onMousePosition += GetMousePos;
        }

        public override void Activate()
        {
            base.Activate();
            InputManager.instance.onLeftMouseButtonPressStarted += TryStartDrainingWater;
            InputManager.instance.onLeftMouseButtonPressCanceled += StopDrainingWater;
            waterLayersLeftToDrain = maxWaterLayersToDrain;
        }

        private void OnGUI()
        {
            if (!isPowerActive)
            {
                return;
            }
            GUIUtils.DisplayTextUnderMouse(mouseScreenPosition, waterLayersLeftToDrain.ToString());
        }

        public override void Update()
        {
            base.Update();

            if (!isPowerActive)
            {
                return;
            }
            
            if (!isDrainingWater)
            {
                return;
            }
            
            int _waterLayersDrained = TryDrainWaterAtMousePosition();
            if (_waterLayersDrained > 0)
            {
                waterLayersLeftToDrain -= _waterLayersDrained;
                if (waterLayersLeftToDrain <= 0)
                {
                    Deactivate();
                }
            }
        }

        private void TryStartDrainingWater()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            isDrainingWater = true;
        }

        private void StopDrainingWater()
        {
            isDrainingWater = false;
            lastDrainedTilePosition = new Vector2Int(int.MinValue, int.MinValue);
        }

        private void GetMousePos(Vector2 _position)
        {
            mouseScreenPosition = _position;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            StopDrainingWater();
            InputManager.instance.onLeftMouseButtonPressStarted -= TryStartDrainingWater;
            InputManager.instance.onLeftMouseButtonPressCanceled -= StopDrainingWater;
        }

        private int TryDrainWaterAtMousePosition()
        {
            Vector2 _mouseWorldPosition = Vector2Int.RoundToInt(mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y))) 
                                          + GameValues.GRID_OFFSET;
            
            Vector2Int _mouseWorldPositionInt = new Vector2Int((int)_mouseWorldPosition.x, (int)_mouseWorldPosition.y);
            
            if (_mouseWorldPositionInt == lastDrainedTilePosition)
            {
                return 0;
            }
            
            lastDrainedTilePosition = _mouseWorldPositionInt;
            
            Dictionary<Tile, Vector2Int> _tilesInRadius = TileSystem.instance.GetAllTilesAtPointWithRadius(_mouseWorldPositionInt, tileRadius, radiusMode);
            
            int _waterLayersDrainedCount = 0;
            foreach (KeyValuePair<Tile, Vector2Int> _tileEntry in _tilesInRadius)
            {
                Tile _tile = _tileEntry.Key;
                
                //Remove all water layers from the top
                int _layersRemoved = RemoveTopWaterLayers(_tile);
                _waterLayersDrainedCount += _layersRemoved;
                
                if (waterLayersLeftToDrain - _waterLayersDrainedCount <= 0)
                {
                    break;
                }
            }

            return _waterLayersDrainedCount;
        }
        
        private int RemoveTopWaterLayers(Tile _tile)
        {
            if (_tile.level == 0)
            {
                return 0;
            }

            int _waterLayersRemoved = 0;
            
            while (_tile.level > 0 && _tile.tileType == TileType.Water)
            {
                _tile.RemoveTileOnTop();
                _waterLayersRemoved++;
            }
            
            return _waterLayersRemoved;
        }
        
        public override bool ShouldStartCooldownOnDeactivate()
        {
            return waterLayersLeftToDrain != maxWaterLayersToDrain;
        }
    }
}

