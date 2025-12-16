using System;
using System.Collections.Generic;
using TileSystemSpace;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Make path if necessary", story: "[Self] make path towards [Pathtarget] if necessary", category: "Action", id: "3a905384eeae0db203acc38d47d5febf")]
public partial class MakePathIfNecessaryAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Pathtarget;
    
    private Pathfinding pathfinder;
    private bool pathFound = false;
    private bool isPathComplete = false;

    protected override Status OnStart()
    {
        pathFound = false;
        if (pathfinder == null)
        {
            pathfinder = new Pathfinding();
            pathfinder.callback += PathFindingCallback;
        }
        
        Vector2Int _currentLocation = new Vector2Int((int)Self.Value.transform.position.x, (int)Self.Value.transform.position.y);
        Vector2Int _destination = new Vector2Int((int)Pathtarget.Value.transform.position.x, (int)Pathtarget.Value.transform.position.y);
        pathfinder.FindPath(_currentLocation, _destination);
        return Status.Running;
    }

    private void PathFindingCallback(List<Cell> _cells)
    {
        pathFound = true;
        isPathComplete = _cells.Count > 0;
    }

    protected override Status OnUpdate()
    {
        if (!pathFound)
        {
            return Status.Running;
        }

        pathFound = false;
        
        if (isPathComplete)
        {
            return Status.Success;
        }
        
        Vector2Int _currentLocation = new Vector2Int((int)Self.Value.transform.position.x, (int)Self.Value.transform.position.y);
        Vector2Int _destination = new Vector2Int((int)Pathtarget.Value.transform.position.x, (int)Pathtarget.Value.transform.position.y);
        
        Vector2Int _point = _currentLocation;

        int _setTileLevel = 3;
        TileType _setTileType = TileType.DamagedDirt;
        int _radiusAtEachStep = 1;
        
        while(_point != _destination)
        {
            Vector2Int _direction = (_destination - _point);
            
            if (_direction.x != 0)
            {
                _point.x += Math.Sign(_direction.x);
            }
            else if (_direction.y != 0)
            {
                _point.y += Math.Sign(_direction.y);
            }

            Dictionary<Tile, Vector2Int> _tiles = TileSystem.instance.GetAllTilesAtPointWithRadius(_point, _radiusAtEachStep);
            foreach (KeyValuePair<Tile, Vector2Int> _tile in _tiles)
            {
                if (_tile.Key == null)
                {
                    continue;
                }
                
                _tile.Key.level = _setTileLevel;
                _tile.Key.tileType = _setTileType;
            }
        }
        
        pathfinder.FindPath(_currentLocation, _destination);
        
        return Status.Running;
    }
}

