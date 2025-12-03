using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Pathfinding
{
   [FormerlySerializedAs("_debug")] [Header("Debug")]
   public bool debug= false; //Probably replace this with some global debug variable, because the visualizer is pretty neat
   
   Cell[,] grid;
   List<Cell> openSet;
   List<Cell> closedSet;

   public Action<List<Cell>> callback; // I found this for now, but there may be a better way to send the path back to the caller

   public Pathfinding()
   {
      grid = new Cell[TileSystem.Instance.GetGridSize().x, TileSystem.Instance.GetGridSize().y];
      for (int _x = 0; _x < TileSystem.Instance.GetGridSize().x; _x++)
      {
         for (int _y = 0; _y < TileSystem.Instance.GetGridSize().y; _y++)
         {
            grid[_x, _y] = new Cell(new Vector2Int(_x, _y));
         }
      }
   }
   public void FindPath(Vector2Int _startPos, Vector2Int _endPos, int _step = 1)
   {
      Debug.Log("has requested to find path");
      openSet = new List<Cell>();
      closedSet = new List<Cell>();
      Cell _startCell = grid[_startPos.x, _startPos.y];
      _startCell.CalcHeuristic(_endPos);
      openSet.Add(_startCell);
      CellIterator(_startPos, _endPos, _step);
   }

   void CellIterator(Vector2Int _startPos, Vector2Int _endPos, int _step = 1)
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
         
         if (_currentCell.position == _endPos)
         {
            Debug.Log("Path found");
            PathConstructor(_currentCell);
            return;
         }
         openSet.Remove(_currentCell);
         closedSet.Add(_currentCell);

         foreach (Cell _neighbor in GetNeighbours(_currentCell.position, _step))
         {
            if (closedSet.Contains(_neighbor))
               continue;

            int _tentativeGCost = _currentCell.gCost + 1;
            
            if (!openSet.Contains(_neighbor) || _tentativeGCost < _neighbor.gCost)
            {
               _neighbor.cameFrom = _currentCell;
               _neighbor.gCost = _tentativeGCost;
               _neighbor.CalcHeuristic(_endPos);

               if (!openSet.Contains(_neighbor))
               {
                  openSet.Add(_neighbor);
               }
            }
         }
      }
      throw new Exception("Path not found");
   }

   void PathConstructor(Cell _current) //This can be safely removed as it's legacy code from the old versions, just call directly ReconstructPath
   {
      ReconstructPath(_current);
   }

   void ReconstructPath(Cell _current)
   {
      List<Cell> _path = new List<Cell>();
      _path.Add(_current);
      while (_current.cameFrom != null)
      {
         _current = _current.cameFrom;
         _path.Add(_current);
      }
      _path.Reverse();
      callback.Invoke(_path);
   }

   List<Cell> GetNeighbours(Vector2Int _pos, int _step)
   {
      Tile _currentTile = TileSystem.Instance.GetTile(_pos);
      List<Cell> _neighbours = new List<Cell>();
      for (int _x = -1; _x <= 1; _x++)
      {
         for (int _y = -1; _y <= 1; _y++)
         {
            if (_y == 0 && _x == 0)
            {
               continue;
            }
            
            int _checkX = _pos.x + _x;
            int _checkY = _pos.y + _y;
            
            if (_checkX < 0 || _checkY < 0 || _checkX >= grid.GetLength(0) || _checkY >= grid.GetLength(1))
               continue;
            
            Tile _neighboringTile = TileSystem.Instance.GetTile(_pos+(new Vector2Int(_x, _y)));
            if (_neighboringTile != null)
            {
               if (Mathf.Abs(_neighboringTile.level - _currentTile.level) > _step)
                  continue;
               
               _neighbours.Add(grid[_pos.x + _x, _pos.y + _y]);
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
   
   public Cell(Vector2Int _pos, Cell _parent = null)
   {
     position = _pos;
     gCost = 0;
     hCost = 0;
     fCost = 0;
     cameFrom = _parent;
     
   }
   public void CalcHeuristic(Vector2Int _endPos)
   {
      int _dx = Mathf.Abs(position.x - _endPos.x);
      int _dy = Mathf.Abs(position.y - _endPos.y);
      
      hCost = 14 * Mathf.Min(_dx, _dy) + 10 * Mathf.Abs(_dx - _dy);
      
      fCost = gCost + hCost;
   }
}
