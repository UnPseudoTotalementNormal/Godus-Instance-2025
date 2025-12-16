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