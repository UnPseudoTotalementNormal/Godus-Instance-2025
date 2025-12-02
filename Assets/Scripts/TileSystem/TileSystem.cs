using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class TileSystem : MonoBehaviour
{
     public static TileSystem Instance;    
    
    [Header("Map size")]
    [SerializeField] int xSize = 50;
    [SerializeField] int ySize = 50;
    
    List<Tile> tiles = new();

    
    [Header("Island")] 
    [SerializeField] int yCenter = 25;
    [SerializeField] int xCenter = 25;
    [SerializeField] int radius = 20;
    List<int> edgePoints = new();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        for (int _i = 0; _i < 360; _i++) //  Generates a circle as an island
        {
            float _angle = _i * Mathf.Deg2Rad;
            float _x1 = xCenter + radius * Mathf.Cos(_angle);
            float _y1 = yCenter + radius * Mathf.Sin(_angle);
            int _calcIndex = Mathf.RoundToInt(_y1) * xSize + Mathf.RoundToInt(_x1);
            edgePoints.Add(_calcIndex);
        }
        
        for (int _x = 0; _x < xSize * ySize; _x++)
        {
            
            float _dx = GetPositionFromIndex(_x).x- xCenter;
            float _dy = GetPositionFromIndex(_x).y- yCenter;
            
            if (edgePoints.Contains(_x))
                tiles.Add(new Tile(1));
            else if (_dx * _dx + _dy * _dy <= radius * radius) // Tries to see if current tile is in the generated circle to fill the island
            {
                tiles.Add(new Tile(Random.Range(1,4)));
            }
            else
                tiles.Add(new Tile());
        }

        GetTile(new Vector2Int(10, 15)).level = 3;
    }

    void Start()
    {
        Debug.Log(GetTile(new Vector2Int(10, 15)).level);
    }
    
    public Tile GetTile(Vector2Int _tilePos)
    {
        int _calcIndex = _tilePos.y * xSize + _tilePos.x;
        return tiles.ElementAtOrDefault(_calcIndex);
    }

    public Vector2Int GetPositionFromIndex(int _index)
    {
        return new Vector2Int(_index % xSize, _index / xSize);
    }

    void OnDrawGizmos() // Full debug to see what the generated map looks like even without any assets
    {
        for (int _i = 0; _i < tiles.Count; _i++)
        {
            Gizmos.color = Color.lightBlue;
            if (tiles[_i].level == 1)
                Gizmos.color = Color.yellow;
            if (tiles[_i].level == 2)
                Gizmos.color = Color.green;
            if (tiles[_i].level == 3)
                Gizmos.color = Color.brown;
            
            Vector2Int _tilePos = GetPositionFromIndex(_i);
            
            Gizmos.DrawCube(new Vector3(_tilePos.x, 0, _tilePos.y), Vector3.one);
        }  
    }
}

[System.Serializable]
public class Tile
{
    public int level = 0;

    public Tile( int _level=0)
    {
        this.level = _level;
    }
    
}