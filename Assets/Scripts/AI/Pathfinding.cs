using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Pathfinding : MonoBehaviour
{
   [FormerlySerializedAs("debug")] [Header("Debug")]
   public bool _debug= false; //Probably replace this with some global debug variable, because the visualizer is pretty neat
   
   Cell[,] grid;
   List<Cell> openSet;
   List<Cell> closedSet;

   public Action<List<Cell>> callback; // I found this for now, but there may be a better way to send the path back to the caller

   void Awake()
   {
      grid = new Cell[50, 50];
      for (int _x = 0; _x < 50; _x++)
      {
         for (int _y = 0; _y < 50; _y++)
         {
            grid[_x, _y] = new Cell(new Vector2Int(_x, _y));
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
      Cell _currentCell = openSet[0];
      while (openSet.Count != 0)
      {
         int _lowestCost = int.MaxValue;
         foreach (Cell _cell in openSet)
         {
            if (_cell.fCost <= _lowestCost)
            {
               if (_cell.fCost == _lowestCost && _cell.hCost < _currentCell.hCost){
                  _currentCell = _cell;
                  _lowestCost = _cell.fCost;
                  continue;
               }
               _currentCell = _cell;
               _lowestCost = _cell.fCost;
            }
         }
         
         if (_currentCell.position == endPos)
         {
            Debug.Log("Path found");
            PathConstructor(_currentCell);
            yield break;
         }
         openSet.Remove(_currentCell);
         closedSet.Add(_currentCell);

         foreach (Cell _neighbor in GetNeighbours(_currentCell.position, step))
         {
            if (closedSet.Contains(_neighbor))
               continue;

            int _tentativeGCost = _currentCell.gCost + 1;
            
            if (!openSet.Contains(_neighbor) || _tentativeGCost < _neighbor.gCost)
            {
               _neighbor.cameFrom = _currentCell;
               _neighbor.gCost = _tentativeGCost;
               _neighbor.CalcHeuristic(endPos);

               if (!openSet.Contains(_neighbor))
               {
                  openSet.Add(_neighbor);
               }
            }
         }

         if (_debug)
         {
            PathConstructor(_currentCell);
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
      List<Cell> _path = new List<Cell>();
      _path.Add(current);
      while (current.cameFrom != null)
      {
         current = current.cameFrom;
         _path.Add(current);
         yield return null;
      }
      _path.Reverse();
      callback.Invoke(_path);
   }

   List<Cell> GetNeighbours(Vector2Int pos, int step)
   {
      Tile _currentTile = TileSystem.Instance.GetTile(pos);
      List<Cell> _neighbours = new List<Cell>();
      for (int _x = -1; _x <= 1; _x++)
      {
         for (int _y = -1; _y <= 1; _y++)
         {
            if (_y == 0 && _x == 0)
            {
               continue;
            }
            
            int _checkX = pos.x + _x;
            int _checkY = pos.y + _y;
            
            if (_checkX < 0 || _checkY < 0 || _checkX >= grid.GetLength(0) || _checkY >= grid.GetLength(1))
               continue;
            
            Tile _neighboringTile = TileSystem.Instance.GetTile(pos+(new Vector2Int(_x, _y)));
            if (_neighboringTile != null)
            {
               if (Mathf.Abs(_neighboringTile.level - _currentTile.level) > step)
                  continue;
               
               _neighbours.Add(grid[pos.x + _x, pos.y + _y]);
            }
         }
      }

      return _neighbours;
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
      int _dx = Mathf.Abs(position.x - endPos.x);
      int _dy = Mathf.Abs(position.y - endPos.y);
      
      hCost = 14 * Mathf.Min(_dx, _dy) + 10 * Mathf.Abs(_dx - _dy);
      
      fCost = gCost + hCost;
   }
}
