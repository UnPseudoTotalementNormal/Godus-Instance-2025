using System;
using TileSystemSpace;
using UnityEngine;

namespace Powers
{
    public class PowerSizeAndShapeModifier : MonoBehaviour
    {
        public Power power;
        private int sizeIncreased = 0;

        private void Start()
        {
            power.onPowerDeactivated += onDeactivated;
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
            power.tileRadius -= sizeIncreased;
            power.radiusMode = TileSystem.RadiusMode.Circle;
        }

        private void onBrushIncreasedPressStarted()
        {
            if (power.tileRadius < 5)
            {
                power.tileRadius++;
                sizeIncreased++;
            }
        }

        private void onBrushDecreasedPressStarted()
        {
            if (power.tileRadius > 0)
            {
                power.tileRadius--;
                sizeIncreased--;
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