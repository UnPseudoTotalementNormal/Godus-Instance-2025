using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Powers;
using TileSystemSpace;
using UnityEngine.Serialization;

public class ResourcesDrop : Power
{
    [Header("Spawn Settings")]
    [SerializeField] private int minToSpawn = 5;
    [SerializeField] private int maxToSpawn = 10;
    [SerializeField] private GameObject prefabToSpawn;

    [Header("Tile Restriction")]
    [SerializeField] private List<TileType> allowedTileTypes;
    [SerializeField] private List<TileType> prohibitedTileTypes;

    private UnityEngine.Camera mainCamera;
    private Vector2 mousePos;
    private bool hasSpawnedAtLeastOne = false;

    private void Start()
    {
        mainCamera = UnityEngine.Camera.main;
        InputManager.instance.onMousePosition += _pos => mousePos = _pos;
    }

    public override void Activate()
    {
        base.Activate();
        hasSpawnedAtLeastOne = false;
        InputManager.instance.onLeftMouseButtonPressStarted += TrySpawn;
    }

    private void TrySpawn()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 _worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        Vector2Int _gridPos = Vector2Int.RoundToInt(_worldPos);

        var _tiles = TileSystem.instance.GetAllTilesAtPointWithRadius(_gridPos, tileRadius, radiusMode);
        List<Vector2Int> _validTiles = new();

        foreach (var _entry in _tiles)
        {
            Tile _tile = _entry.Key;
            Vector2Int _pos = _entry.Value;

            if (allowedTileTypes.Count > 0 && !allowedTileTypes.Contains(_tile.tileType))
                continue;

            if (prohibitedTileTypes.Contains(_tile.tileType))
                continue;

            _validTiles.Add(_pos);
        }

        int _toSpawn = Random.Range(minToSpawn, maxToSpawn + 1);
        _toSpawn = Mathf.Min(_toSpawn, _validTiles.Count);

        if (_toSpawn == 0)
        {
            hasSpawnedAtLeastOne = false;
            Deactivate();
            return;
        }

        hasSpawnedAtLeastOne = true;

        for (int _i = 0; _i < _toSpawn; _i++)
        {
            int _idx = Random.Range(0, _validTiles.Count);
            Vector2Int _pos = _validTiles[_idx];
            _validTiles.RemoveAt(_idx);

            Instantiate(prefabToSpawn, (Vector2)_pos, Quaternion.identity);
        }

        Deactivate();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        InputManager.instance.onLeftMouseButtonPressStarted -= TrySpawn;
    }

    public override bool ShouldStartCooldownOnDeactivate()
    {
        return hasSpawnedAtLeastOne;
    }
}
