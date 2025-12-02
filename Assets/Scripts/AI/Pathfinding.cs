using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
   [Header("Debug")]
   public bool debug= false; //Probably replace this with some global debug variable, because the visualizer is pretty neat
   
   Cell[,] grid;
   List<Cell> openSet;
   List<Cell> closedSet;

   public Action<List<Cell>> callback; // I found this for now, but there may be a better way to send the path back to the caller

   void Awake()
   {
      grid = new Cell[50, 50];
      for (int x = 0; x < 50; x++)
      {
         for (int y = 0; y < 50; y++)
         {
            grid[x, y] = new Cell(new Vector2Int(x, y));
         }
      }
   }
   public void FindPath(Vector2Int startPos, Vector2Int endPos, int step = 1)
   {
      Debug.Log(name + "has requested to find path");
      openSet = new List<Cell>();
      closedSet = new List<Cell>();
      Cell _startCell = grid[startPos.x, startPos.y];
      _startCell.CalcHeuristic(endPos);
      openSet.Add(_startCell);
      StartCoroutine(CellIterator(startPos, endPos, step));
   }

   IEnumerator CellIterator(Vector2Int startPos, Vector2Int endPos, int step = 1)
   {
      Cell currentCell = openSet[0];
      while (openSet.Count != 0)
      {
         int lowestCost = int.MaxValue;
         foreach (Cell cell in openSet)
         {
            if (cell.fCost <= lowestCost)
            {
               if (cell.fCost == lowestCost && cell.hCost < currentCell.hCost){
                  currentCell = cell;
                  lowestCost = cell.fCost;
                  continue;
               }
               currentCell = cell;
               lowestCost = cell.fCost;
            }
         }
         
         if (currentCell.position == endPos)
         {
            Debug.Log("Path found");
            PathConstructor(currentCell);
            yield break;
         }
         openSet.Remove(currentCell);
         closedSet.Add(currentCell);

         foreach (Cell neighbor in GetNeighbours(currentCell.position, step))
         {
            if (closedSet.Contains(neighbor))
               continue;

            int tentativeGCost = currentCell.gCost + 1;
            
            if (!openSet.Contains(neighbor) || tentativeGCost < neighbor.gCost)
            {
               neighbor.cameFrom = currentCell;
               neighbor.gCost = tentativeGCost;
               neighbor.CalcHeuristic(endPos);

               if (!openSet.Contains(neighbor))
               {
                  openSet.Add(neighbor);
               }
            }
         }

         if (debug)
         {
            PathConstructor(currentCell);
            yield return new WaitForSeconds(0.1f);
         }
         else
         {
            yield return null;
         }
         
      }
      throw new Exception("Path not found");
   }

   void PathConstructor(Cell current)
   {
      StartCoroutine(ReconstructPath(current));
   }

   IEnumerator ReconstructPath(Cell current)
   {
      List<Cell> path = new List<Cell>();
      path.Add(current);
      while (current.cameFrom != null)
      {
         current = current.cameFrom;
         path.Add(current);
         yield return null;
      }
      path.Reverse();
      callback.Invoke(path);
   }

   List<Cell> GetNeighbours(Vector2Int pos, int step)
   {
      Tile currentTile = TileSystem.Instance.GetTile(pos);
      List<Cell> neighbours = new List<Cell>();
      for (int x = -1; x <= 1; x++)
      {
         for (int y = -1; y <= 1; y++)
         {
            if (y == 0 && x == 0)
            {
               continue;
            }
            
            int checkX = pos.x + x;
            int checkY = pos.y + y;
            
            if (checkX < 0 || checkY < 0 || checkX >= grid.GetLength(0) || checkY >= grid.GetLength(1))
               continue;
            
            Tile neighboringTile = TileSystem.Instance.GetTile(pos+(new Vector2Int(x, y)));
            if (neighboringTile != null)
            {
               if (Mathf.Abs(neighboringTile.level - currentTile.level) > step)
                  continue;
               
               neighbours.Add(grid[pos.x + x, pos.y + y]);
            }
         }
      }

      return neighbours;
   }
}

[Serializable]
public class Cell
{
   public Vector2Int position;
   public int gCost;
   public int hCost;
   public int fCost;
   public Cell cameFrom;
   
   public Cell(Vector2Int pos, Cell parent = null)
   {
     position = pos;
     gCost = 0;
     hCost = 0;
     fCost = 0;
     cameFrom = parent;
     
   }
   public void CalcHeuristic(Vector2Int endPos)
   {
      int dx = Mathf.Abs(position.x - endPos.x);
      int dy = Mathf.Abs(position.y - endPos.y);
      
      hCost = 14 * Mathf.Min(dx, dy) + 10 * Mathf.Abs(dx - dy);
      
      fCost = gCost + hCost;
   }
}
