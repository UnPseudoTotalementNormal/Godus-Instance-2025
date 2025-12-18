using System;
using System.Collections.Generic;
using TileSystemSpace;
using UnityEngine;
using Action = System.Action;

namespace AI
{
    public class PathHolder : MonoBehaviour
    {
        public List<Vector2Int> waypoints { get; private set; } = new();
        public GameObject targetObject;

        public bool askForRecalculation;
        public bool hasMapBeenChangedSinceLastPathCalculation { get; private set; } = true;
        
        public event Action onPathChanged;

        public PathHolder()
        {
            TileSystem.instance.onAnyTileChanged += (_, _) =>
            {
                hasMapBeenChangedSinceLastPathCalculation = true;
            };
        }
        
        public void SetPath(List<Vector2Int> _waypoints)
        {
            askForRecalculation = false;
            hasMapBeenChangedSinceLastPathCalculation = false;
            waypoints = new List<Vector2Int>(_waypoints);
            onPathChanged?.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < waypoints.Count; i++)
            {
                Vector3 _waypointPos = new Vector3(waypoints[i].x, waypoints[i].y, 0);
                Gizmos.DrawSphere(_waypointPos, 0.1f);
                
                if (i > 0)
                {
                    Vector3 _previousWaypointPos = new Vector3(waypoints[i - 1].x, waypoints[i - 1].y, 0);
                    Gizmos.DrawLine(_previousWaypointPos, _waypointPos);
                }
            }
        }
    }
}