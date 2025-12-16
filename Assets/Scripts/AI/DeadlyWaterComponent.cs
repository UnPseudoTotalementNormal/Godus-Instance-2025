using System;
using TileSystemSpace;
using UnityEngine;

public class DeadlyWaterComponent : MonoBehaviour
{
    private float timeSinceLastUpdate = 0f;
    [SerializeField] private float updateInterval = 0.15f;
    private Vector2Int _tilePos;
    public event Action onDeath;

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
        TouchWater(_tile);

    }


    public void TouchWater(Tile _tile)
    {
        if (_tile.tileType == TileType.Water) Die();
    }

    private void Die()
    {
        onDeath?.Invoke();
        Destroy(gameObject);
    }
}
