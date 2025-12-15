using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TileSystemSpace;
using UnityEngine;
using UnityEngine.Serialization;

public class Pathfinding
{
   [FormerlySerializedAs("_debug")] [Header("Debug")]
   public bool debug= false; //Probably replace this with some global debug variable, because the visualizer is pretty neat
   
   Cell[,] grid;
   HashSet<Cell> openSet;
   HashSet<Cell> closedSet;
   List<Cell> openSetList; 
   Cell[] neighboursBuffer;

   public Action<List<Cell>> callback; // I found this for now, but there may be a better way to send the path back to the caller

   bool searchingForPath = false;
   public Pathfinding()
   {
      grid = new Cell[TileSystem.instance.GetGridSize().x, TileSystem.instance.GetGridSize().y];
      for (int _x = 0; _x < TileSystem.instance.GetGridSize().x; _x++)
      {
         for (int _y = 0; _y < TileSystem.instance.GetGridSize().y; _y++)
         {
            grid[_x, _y] = new Cell(new Vector2Int(_x, _y));
         }
      }
      
      openSet = new HashSet<Cell>();
      closedSet = new HashSet<Cell>();
      openSetList = new List<Cell>();
      neighboursBuffer = new Cell[8]; // 8 neighbor maximum in a grid (including diagonals)
   }
   public void FindPath(Vector2Int _startPos, Vector2Int _endPos, int _step = 1)
   {
      if (searchingForPath)
         return;
      searchingForPath = true;
      
      foreach (Cell _cell in grid)
      {
         _cell.gCost = 0;
         _cell.hCost = 0;
         _cell.fCost = 0;
         _cell.cameFrom = null;
      }
      
      _endPos = new Vector2Int(Mathf.Clamp(_endPos.x,0,grid.GetLength(0)-1),Mathf.Clamp(_endPos.y,0,grid.GetLength(1)-1));
      //Debug.Log("has requested to find path");
      
      openSet.Clear();
      closedSet.Clear();
      openSetList.Clear();
      
      Cell _startCell = grid[_startPos.x, _startPos.y];
      _startCell.CalcHeuristic(_endPos);
      openSet.Add(_startCell);
      openSetList.Add(_startCell);
      CellIterator(_startPos, _endPos, _step);
   }

   void CellIterator(Vector2Int _startPos, Vector2Int _endPos, int _step = 1)
   {
      Cell _currentCell = null;
      while (openSet.Count != 0)
      {
         int _lowestCost = int.MaxValue;
         int _lowestIndex = 0;
         for (int i = 0; i < openSetList.Count; i++)
         {
            Cell _cell = openSetList[i];
            if (_cell.fCost < _lowestCost || (_cell.fCost == _lowestCost && _cell.hCost < openSetList[_lowestIndex].hCost))
            {
               _lowestCost = _cell.fCost;
               _lowestIndex = i;
            }
         }
         _currentCell = openSetList[_lowestIndex];
         
         if (_currentCell.position == _endPos)
         {
            //Debug.Log("Path found");
            PathConstructor(_currentCell);
            return;
         }
         
         openSet.Remove(_currentCell);
         openSetList.RemoveAt(_lowestIndex);
         closedSet.Add(_currentCell);

         int _neighbourCount = GetNeighbours(_currentCell.position, _step);
         for (int i = 0; i < _neighbourCount; i++)
         {
            Cell _neighbor = neighboursBuffer[i];
            
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
                  openSetList.Add(_neighbor);
               }
            }
         }
      }
      Debug.LogWarning("Could not find valid path for" + _endPos);
      callback?.Invoke(new List<Cell> {});
      //PathConstructor(_currentCell); //Uncomment if you still want the AI to find a path, however note that it will probably bring them to the other side of the map
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
      searchingForPath = false;
      callback.Invoke(_path);
   }

   int GetNeighbours(Vector2Int _pos, int _step)
   {
      Tile _currentTile = TileSystem.instance.GetTile(_pos);
      int _count = 0;
      
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
            
            Tile _neighboringTile = TileSystem.instance.GetTile(_pos+(new Vector2Int(_x, _y)));
            if (_neighboringTile != null)
            {
               // Use this whole nested statement to implement rules, an example is the line right below, which checks if the agent can step on the tile, but also if the tile is not water
               if (Mathf.Abs(_neighboringTile.level - _currentTile.level) > _step || _neighboringTile.tileType == TileType.Water)
                  continue;
               neighboursBuffer[_count] = grid[_pos.x + _x, _pos.y + _y];
               _count++;
            }
         }
      }

      return _count;
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
   
   // Optimisation HashSet : override GetHashCode et Equals
   public override int GetHashCode()
   {
      return position.GetHashCode();
   }
   
   public override bool Equals(object obj)
   {
      if (obj is Cell other)
      {
         return position.Equals(other.position);
      }
      return false;
   }
}
