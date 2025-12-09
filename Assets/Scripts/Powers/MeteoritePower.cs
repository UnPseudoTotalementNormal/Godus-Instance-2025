using System;
using System.Collections;
using System.Collections.Generic;
using TileSystemSpace;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Powers
{
    public class MeteoritePower : Power
    {
        [SerializeField] private Meteorite meteoritePrefab;
        
        [SerializeField] private int maxMeteorites = 1;
        
        [SerializeField] private float damageAmount = 50;

        [SerializeField] private int minTileDigLevel = 1;
        [SerializeField] private int maxTileDigLevel = 2;
        
        [SerializeField] private float screenShakeForce = 2;
        [SerializeField] private float screenShakeDuration = 2;
        
        private int meteoriteLeft = 1;
        
        private Vector2 mouseScreenPosition;
        private UnityEngine.Camera mainCamera;
        
        private void Start()
        {
            mainCamera = UnityEngine.Camera.main;
            InputManager.instance.onMousePosition += OnGetMousePosition;
        }

        private void OnGetMousePosition(Vector2 _mousePos)
        {
            mouseScreenPosition = _mousePos;
        }

        private void OnGUI()
        {
            if (!isPowerActive)
            {
                return;
            }
            GUIUtils.DisplayTextUnderMouse(mouseScreenPosition, meteoriteLeft.ToString());
        }

        public override void Activate()
        {
            base.Activate();
            meteoriteLeft = maxMeteorites;
            InputManager.instance.onLeftMouseButtonPressStarted += TryDropMeteorite;
        }


        public override void Deactivate()
        {
            base.Deactivate();
            InputManager.instance.onLeftMouseButtonPressStarted -= TryDropMeteorite;
        }

        private void TryDropMeteorite()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            
            Vector2 _mouseWorldPosition = Vector2Int.RoundToInt(mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y))) 
                                          + GameValues.GRID_OFFSET;
            Vector2Int _mouseWorldPositionInt = new Vector2Int((int)_mouseWorldPosition.x, (int)_mouseWorldPosition.y);

            var _newMeteorite = Instantiate(meteoritePrefab, new Vector3(_mouseWorldPositionInt.x, _mouseWorldPositionInt.y, 0), Quaternion.identity);
            _newMeteorite.explosionRadius = tileRadius;
            _newMeteorite.damageAmount = damageAmount;
            _newMeteorite.minTileDigLevel = minTileDigLevel;
            _newMeteorite.maxTileDigLevel = maxTileDigLevel;
            _newMeteorite.impulseForce = screenShakeForce;
            _newMeteorite.impulseDuration = screenShakeDuration;
            
            _newMeteorite.LandAtPosition(_mouseWorldPositionInt);
            
            meteoriteLeft--;
            if (meteoriteLeft <= 0)
            {
                Deactivate();
            }
        }
        
        public override bool ShouldStartCooldownOnDeactivate()
        {
            return meteoriteLeft != maxMeteorites;
        }
    }
}
