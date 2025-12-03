using System.Collections.Generic;
using UnityEngine;

public class Test_PathfindingCaller : MonoBehaviour
{
    Pathfinding pathfinder;
    [SerializeField] Vector2Int startPos = new Vector2Int(0, 0);
    [SerializeField] Vector2Int endPos = new Vector2Int(0, 0);
    [SerializeField] List<Cell> newPath;

    void Start()
    {
        pathfinder = new Pathfinding();
        pathfinder.callback += SetNewPath;
        pathfinder.FindPath(startPos, endPos);
    }

    void SetNewPath(List<Cell> newPath)
    {
        this.newPath = newPath;
    }

    [ContextMenu("FindPath")]
    void FindNewPath()
    {
        pathfinder.FindPath(startPos, endPos);
    }

    void OnDrawGizmos()
    {
        foreach (Cell cell in newPath)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(new Vector3(cell.position.x,cell.position.y, 2), new Vector3(1, 1, 1));
        }
    }
}
