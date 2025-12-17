using System;
using TileSystemSpace;
using UnityEngine;

namespace Powers
{
    public class PowerSizeAndShapeModifier : MonoBehaviour
    {
        public Power power;
        private int sizeIncreased = 0;
        private bool isPowerActive = false;

        [SerializeField] private int maxBrushIncrease = 3;
        
        private void Start()
        {
            power.onPowerDeactivated += onDeactivated;
            power.onPowerActivated += onActivated;
            InputManager.instance.onBrushIncreasedPressStarted += onBrushIncreasedPressStarted;
            InputManager.instance.onBrushDecreasedPressStarted += onBrushDecreasedPressStarted;
            InputManager.instance.onBrushShapeChangedPressStarted += onBrushShapeChanged;
        }

        private void OnDestroy()
        {
            InputManager.instance.onBrushIncreasedPressStarted -= onBrushIncreasedPressStarted;
            InputManager.instance.onBrushDecreasedPressStarted -= onBrushDecreasedPressStarted;
            InputManager.instance.onBrushShapeChangedPressStarted -= onBrushShapeChanged;
        }

        private void OnGUI()
        {
            if (power.isPowerActive)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontSize = 16;
                style.normal.textColor = Color.white;
                string shape = power.radiusMode == TileSystem.RadiusMode.Circle ? "Circle" : "Diamond";
                GUI.Label(new Rect(10, 10, 300, 20), "Power Brush Size: " + power.tileRadius.ToString() + " Shape: " + shape, style);
            }
        }


        private void onDeactivated()
        {
            isPowerActive = false;
        }

        private void onActivated()
        {
            power.tileRadius = 0;
            power.radiusMode = TileSystem.RadiusMode.Circle;
            Debug.Log(sizeIncreased);
            isPowerActive = true;
        }

        private void onBrushIncreasedPressStarted()
        {
            if (power.tileRadius < maxBrushIncrease && isPowerActive)
            {
                power.tileRadius++;
            }
        }

        private void onBrushDecreasedPressStarted()
        {
            if (power.tileRadius > 0 && isPowerActive)
            {
                power.tileRadius--;
            }
        }
        
        private void onBrushShapeChanged()
        {
            if (power.radiusMode == TileSystem.RadiusMode.Circle)
            {
                power.radiusMode = TileSystem.RadiusMode.Diamond;
                return;  
            }
            power.radiusMode = TileSystem.RadiusMode.Circle;
        }
    }
}