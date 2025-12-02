using System.Collections.Generic;
using UnityEngine;

public class TileSystem : MonoBehaviour
{
    [Header("Map size")]
    [SerializeField] int xSize = 50;
    [SerializeField] int ySize = 50;
    
    List<Tile> tiles = new();


    [Header("Island")] 
    [SerializeField] int yCentre = 25;
    [SerializeField] int xCentre = 25;
    [SerializeField] int radius = 20;
    List<int> edgePoints = new();
    
    void Awake()
    {
        for (int i = 0; i < 360; i++) //  Generates a circle as an island
        {
            float angle = i * Mathf.Deg2Rad;
            float x1 = xCentre + radius * Mathf.Cos(angle);
            float y1 = yCentre + radius * Mathf.Sin(angle);
            int calcIndex = Mathf.RoundToInt(y1) * xSize + Mathf.RoundToInt(x1);
            edgePoints.Add(calcIndex);
        }
        
        for (int x = 0; x < xSize * ySize; x++)
        {
            
            float dx = GetPositionFromIndex(x).x- xCentre;
            float dy = GetPositionFromIndex(x).y- yCentre;
            
            if (edgePoints.Contains(x))
                tiles.Add(new Tile(3));
            else if (dx * dx + dy * dy <= radius * radius) // Tries to see if current tile is in the generated circle to fill the island
            {
                tiles.Add(new Tile(3));
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
    
    Tile GetTile(Vector2Int tilePos)
    {
        int calcIndex = tilePos.y * xSize + tilePos.x;
        return tiles[calcIndex];
    }

    Vector2Int GetPositionFromIndex(int index)
    {
        return new Vector2Int(index % xSize, index / xSize);
    }

    void OnDrawGizmos() // Full debug to see what the generated map looks like even without any assets
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            Gizmos.color = Color.yellow;
            if (tiles[i].level == 3)
                Gizmos.color = Color.red;
            
            Vector2Int tilePos = GetPositionFromIndex(i);
            
            Gizmos.DrawCube(new Vector3(tilePos.x, 0, tilePos.y), Vector3.one);
        }  
    }
}

[System.Serializable]
public class Tile
{
    public int level = 0;

    public Tile( int level=0)
    {
        this.level = level;
    }
    
}