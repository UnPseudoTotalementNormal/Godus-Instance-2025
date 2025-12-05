using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using TileSystemSpace;
using UnityEditor.Rendering;
using UnityEngine;

public class PowerRangeFeedback : MonoBehaviour
{
    bool isPowerActivated = false;

    private Vector2 mousePosition;
    
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.05f;
    [SerializeField] private Color lineColor = Color.red;
    
    private List<GameObject> activeLines = new();

    private UnityEngine.Camera mainCamera;
    
    private void Start()
    {
        mainCamera = UnityEngine.Camera.main;
        InputManager.instance.onMousePosition += GetMousePos;
    }

    private void LateUpdate()
    {
        DrawPowerEdges();
    }


    private void GetMousePos(Vector2 _mousePosition)
    {
        mousePosition = _mousePosition;
    }

    private void DrawPowerEdges()
    {
        ClearLines();
        
        Vector2Int _newMousePosInt = Vector2Int.RoundToInt(mousePosition);
        
        if (!mainCamera)
        {
            return;
        }
        
        Vector3 _posMouse = mainCamera
            .ScreenToWorldPoint(new Vector3(_newMousePosInt.x, _newMousePosInt.y));
        _posMouse += Vector3.one * 0.5f;
        
        Dictionary<Tile, Vector2Int> _tilesInRange = TileSystem.instance.GetAllTilesAtPointWithRadius(
            new Vector2Int ((int)_posMouse.x,(int)_posMouse.y),
            2, TileSystem.RadiusMode.Circle);
        
        foreach (KeyValuePair<Tile, Vector2Int> _tile in _tilesInRange)
        {
            DrawEdges(_tile, _tilesInRange);
        }
    }
    
    private void ClearLines()
    {
        foreach (GameObject _line in activeLines)
        {
            if (_line)
            {
                Destroy(_line);
            }
        }
        activeLines.Clear();
    }

    private void DrawEdges(KeyValuePair<Tile, Vector2Int> _tile, Dictionary<Tile, Vector2Int> _tilesInRange)
    {
        Vector3 _tileWorldPos = new Vector3(_tile.Value.x, _tile.Value.y, 0f);
        
        Vector3 _topLeft = new Vector3(_tileWorldPos.x - 0.5f, _tileWorldPos.y + 0.5f, 0f);
        Vector3 _topRight = new Vector3(_tileWorldPos.x + 0.5f, _tileWorldPos.y + 0.5f, 0f);
        Vector3 _bottomLeft = new Vector3(_tileWorldPos.x - 0.5f, _tileWorldPos.y - 0.5f, 0f);
        Vector3 _bottomRight = new Vector3(_tileWorldPos.x + 0.5f, _tileWorldPos.y - 0.5f, 0f);

        if (!_tilesInRange.ContainsValue(new Vector2Int(_tile.Value.x, _tile.Value.y + 1)))
        {
            CreateLine(_topLeft, _topRight);
        }

        if (!_tilesInRange.ContainsValue(new Vector2Int(_tile.Value.x, _tile.Value.y - 1)))
        {
            CreateLine(_bottomLeft, _bottomRight);
        }

        if (!_tilesInRange.ContainsValue(new Vector2Int(_tile.Value.x - 1, _tile.Value.y)))
        {
            CreateLine(_topLeft, _bottomLeft);
        }

        if (!_tilesInRange.ContainsValue(new Vector2Int(_tile.Value.x + 1, _tile.Value.y)))
        {
            CreateLine(_topRight, _bottomRight);
        }
    }
    
    private void CreateLine(Vector3 _start, Vector3 _end)
    {
        GameObject _lineObj = new GameObject("EdgeLine");
        _lineObj.transform.SetParent(transform);
        
        LineRenderer _lineRenderer = _lineObj.AddComponent<LineRenderer>();
        
        _lineRenderer.material = lineMaterial;
        _lineRenderer.startColor = lineColor;
        _lineRenderer.endColor = lineColor;
        _lineRenderer.startWidth = lineWidth;
        _lineRenderer.endWidth = lineWidth;
        _lineRenderer.positionCount = 2;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.sortingOrder = 100;
        
        _lineRenderer.SetPosition(0, _start);
        _lineRenderer.SetPosition(1, _end);
        
        activeLines.Add(_lineObj);
    }
}

