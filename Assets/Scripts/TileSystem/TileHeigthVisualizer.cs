using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace TileSystemSpace
{
    public class TileHeigthVisualizer : MonoBehaviour
    {
        private bool heightShow;
        private Dictionary<Vector2Int, TextMeshPro> tileHeightVisualizersDictionary = new();
        [SerializeField] private Transform transformParentToInstantiate;

        private Vector2 tempPosition;


        private void Start()
        {
            InputManager.instance.onShowCoordinatePressStarted += ToggleShowTileHeight;
            TileSystem.instance.onAnyTileChanged += OnAnyTileChanged;
            GetTileHeight();
            ToggleShowTileHeight();
        }

        private void OnAnyTileChanged(Tile tile, Vector2Int position)
        {
            if (tileHeightVisualizersDictionary.TryGetValue(position, out TextMeshPro textMesh))
            {
                textMesh.text = tile.level.ToString();
            }
        }
        private void GetTileHeight()
        {
            Vector2Int gridSize = TileSystem.instance.GetSize();
            
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Tile tile = TileSystem.instance.GetTile(x, y);
                    GameObject go = new GameObject($"LevelText_{x}_{y}");
                    TextMeshPro tileLevelText = go.AddComponent<TextMeshPro>();
                    go.transform.SetParent(transformParentToInstantiate, false);
                    tempPosition.x = x + 9.9f;
                    tempPosition.y = y - 2.2f;
                    tileLevelText.transform.position = tempPosition;
                    tileLevelText.fontSize = 6;
                    tileLevelText.text = tile.level.ToString();
                    tileHeightVisualizersDictionary[new Vector2Int(x, y)] = tileLevelText;

                }
            }
        }
        private void ToggleShowTileHeight()
        {
            heightShow = !heightShow;
            transformParentToInstantiate.position = heightShow
                ? new Vector3(-100000, -100000)
                : Vector3.zero;
        }

        private void OnDestroy()
        {
            if (InputManager.instance != null)
            {
                InputManager.instance.onShowCoordinatePressStarted -= ToggleShowTileHeight;
            }
            if (TileSystem.instance != null)
            {
                TileSystem.instance.onAnyTileChanged -= OnAnyTileChanged;
            }
        }
    }
}