using System;
using System.Collections.Generic;
using AudioSystem;
using Camera;
using FMOD.Studio;
using FMODUnity;
using TileSystemSpace;
using UnityEngine;

public class CameraAmbianceSounds : MonoBehaviour
{
    [SerializeField] private UnityEngine.Camera targetCamera;
    [SerializeField] private CameraZoom cameraZoom;
    
    [SerializeField] private float checkTilesInterval = 1f;
    private float checkTilesTimer = 0f;

    [SerializeField] private EventReference ambianceEvent;
    
    private const string EVENT_INSTANCE_KEY = "CameraAmbiance_EventInstance";
    
    private const string PARAM_WATER_PERCENTAGE = "isSea";
    private const string PARAM_LAND_PERCENTAGE = "isGrass";
    private const string PARAM_ZOOMED_IN_PERCENTAGE = "isZoomedIn";
    
    private EventInstance ambianceEventInstance;
    
    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = UnityEngine.Camera.main;
        }

        ambianceEventInstance = GameAudioManager.instance.PlayEventInstance(ambianceEvent, EVENT_INSTANCE_KEY).Value;
        ambianceEventInstance.setParameterByName(PARAM_WATER_PERCENTAGE, 0);
        ambianceEventInstance.setParameterByName(PARAM_LAND_PERCENTAGE, 0);
        ambianceEventInstance.setParameterByName(PARAM_ZOOMED_IN_PERCENTAGE, 0);
    }

    private void OnDestroy()
    {
        GameAudioManager.instance.StopEventInstance(EVENT_INSTANCE_KEY);
    }

    private void Update()
    {
        CheckTiles();
        
        float _zoomedInPercentage = cameraZoom ? cameraZoom.GetZoomedInPercentage() : 1f;
        ambianceEventInstance.setParameterByName(PARAM_ZOOMED_IN_PERCENTAGE, _zoomedInPercentage);
    }

    private void CheckTiles()
    {
        checkTilesTimer -= Time.deltaTime;
        if (checkTilesTimer > 0f)
        {
            return;
        }
        
        checkTilesTimer = checkTilesInterval;

        Dictionary<Tile, Vector2Int> _tilesInView = GetTilesInCameraView();
        
        if (_tilesInView.Count == 0)
        {
            return;
        }
        
        int _waterTileCount = 0;
        int _landTileCount = 0;
        
        foreach (KeyValuePair<Tile, Vector2Int> _tileInView in _tilesInView)
        {
            if (_tileInView.Key.tileType == TileType.Water)
            {
                _waterTileCount++;
            }
            else
            {
                _landTileCount++;
            }
        }
        
        float _waterPercentage = (_waterTileCount / (float)_tilesInView.Count);
        float _landPercentage = (_landTileCount / (float)_tilesInView.Count);
        
        ambianceEventInstance.setParameterByName(PARAM_WATER_PERCENTAGE, _waterPercentage);
        ambianceEventInstance.setParameterByName(PARAM_LAND_PERCENTAGE, _landPercentage);
    }
    
    /// <summary>
    /// Gets all tiles that are visible in the camera's field of view
    /// </summary>
    /// <returns>Dictionary of tiles with their positions that are in camera view</returns>
    private Dictionary<Tile, Vector2Int> GetTilesInCameraView()
    {
        Dictionary<Tile, Vector2Int> _tilesInView = new Dictionary<Tile, Vector2Int>();
        
        if (TileSystem.instance == null)
        {
            return _tilesInView;
        }
        
        float _cameraHeight = targetCamera.orthographicSize * 2f;
        float _cameraWidth = _cameraHeight * targetCamera.aspect;
        
        Vector3 _cameraPos = targetCamera.transform.position;
        
        Vector2 _minBounds = new Vector2(_cameraPos.x - _cameraWidth / 2f, _cameraPos.y - _cameraHeight / 2f);
        Vector2 _maxBounds = new Vector2(_cameraPos.x + _cameraWidth / 2f, _cameraPos.y + _cameraHeight / 2f);
        
        Vector2Int _minTilePos = new Vector2Int(Mathf.FloorToInt(_minBounds.x), Mathf.FloorToInt(_minBounds.y));
        Vector2Int _maxTilePos = new Vector2Int(Mathf.CeilToInt(_maxBounds.x), Mathf.CeilToInt(_maxBounds.y));
        
        Vector2Int _gridSize = TileSystem.instance.GetGridSize();
        _minTilePos.x = Mathf.Max(0, _minTilePos.x);
        _minTilePos.y = Mathf.Max(0, _minTilePos.y);
        _maxTilePos.x = Mathf.Min(_gridSize.x - 1, _maxTilePos.x);
        _maxTilePos.y = Mathf.Min(_gridSize.y - 1, _maxTilePos.y);
        
        for (int _x = _minTilePos.x; _x <= _maxTilePos.x; _x++)
        {
            for (int _y = _minTilePos.y; _y <= _maxTilePos.y; _y++)
            {
                Vector2Int _tilePos = new Vector2Int(_x, _y);
                Tile _tile = TileSystem.instance.GetTile(_tilePos);
                
                if (_tile != null)
                {
                    _tilesInView[_tile] = _tilePos;
                }
            }
        }
        
        return _tilesInView;
    }
}
