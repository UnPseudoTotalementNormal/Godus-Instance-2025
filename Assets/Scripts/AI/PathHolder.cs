using System.Collections.Generic;
using UnityEngine;
using Action = System.Action;

namespace AI
{
    public class PathHolder : MonoBehaviour
    {
        public List<Vector2Int> waypoints { get; private set; } = new();
        
        public event Action onPathChanged;
        
        public void SetPath(List<Vector2Int> _waypoints)
        {
            waypoints = new List<Vector2Int>(_waypoints);
            onPathChanged?.Invoke();
        }
    }
}