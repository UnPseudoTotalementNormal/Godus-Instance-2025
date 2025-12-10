using System;
using TileSystemSpace;
using UnityEngine;

namespace AI
{
    public class EntityHeightComponent : MonoBehaviour
    {
        public int heightLevel { get; private set; }
        
        [SerializeField] private float updateInterval = 0.15f;
        private float timeSinceLastUpdate = 0f;
        
        private Vector2Int _tilePos;
        
        private TileSystem tileSystem;

        private void Start()
        {
            tileSystem = TileSystem.instance;
        }

        private void Update()
        {
            if (timeSinceLastUpdate < updateInterval)
            {
                timeSinceLastUpdate += Time.deltaTime;
                return;
            }
            timeSinceLastUpdate = 0f;
            _tilePos.x = Mathf.FloorToInt(transform.position.x + GameValues.GRID_OFFSET.x);
            _tilePos.y = Mathf.FloorToInt(transform.position.y + GameValues.GRID_OFFSET.y);
            var _tile = tileSystem.GetTile(_tilePos);
            if (_tile == null) return;
            heightLevel = _tile.level;
            
        }
    }
}