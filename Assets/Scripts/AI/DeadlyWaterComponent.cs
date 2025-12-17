using System;
using TileSystemSpace;
using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class DeadlyWaterComponent : MonoBehaviour
{
    private float timeSinceLastUpdate = 0f;
    [SerializeField] private float updateInterval = 0.15f;
    private Vector2Int _tilePos;

    [SerializeField] private HealthComponent healthComponent;
        
    private bool isOnWater = false;
    
    private TileSystem tileSystem;

    private void Start()
    {
        tileSystem = TileSystem.instance;
    }

    private void Update()
    {
        if (isOnWater)
        {
            healthComponent.TakeDamage(100f * Time.deltaTime);
        }
        
        if (timeSinceLastUpdate < updateInterval)
        {
            timeSinceLastUpdate += Time.deltaTime;
            return;
        }
        timeSinceLastUpdate = 0f;
        _tilePos.x = Mathf.FloorToInt(transform.position.x + GameValues.GRID_OFFSET.x);
        _tilePos.y = Mathf.FloorToInt(transform.position.y + GameValues.GRID_OFFSET.y);
        var _tile = tileSystem.GetTile(_tilePos);
        CheckWater(_tile);

    }


    public void CheckWater(Tile _tile)
    {
        isOnWater = _tile.tileType == TileType.Water;
    }
}
