using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace TileSystemSpace

{
    public class TileHeigthVisualizer : MonoBehaviour
    {
        private bool heightShow = false;
        Dictionary<Vector2Int, TextMeshPro> tileHeightVisualizersDictionary = new();
        [SerializeField] private Transform transformParentToInstantiate;


        private void Start()
        {
            InputManager.instance.onShowCoordinatePressStarted += ToggleShowTileHeight;
            TileSystem.instance.onAnyTileChanged += OnAnyTileChanged;
            GetTileHeight();
            ToggleShowTileHeight();
        }

        private void OnAnyTileChanged(Tile arg1, Vector2Int arg2)
        {
            tileHeightVisualizersDictionary[arg2].text = TileSystem.instance.GetTile(arg2).level.ToString();
        }
        private void GetTileHeight()
        {
            TileSystem.instance.GetSize();
            for (int x = 0; x < TileSystem.instance.GetSize().x; x++)
            {
                for (int y = 0; y < TileSystem.instance.GetSize().y; y++)
                {
                    TileSystem.instance.GetTile(x,y);
                    GameObject go = new GameObject("LevelText");
                    TextMeshPro tileLevelText = go.AddComponent<TextMeshPro>();
                    go.transform.SetParent(transformParentToInstantiate);
                    tileLevelText.transform.position = new Vector2(x+9.9f, y-2.2f);
                    tileLevelText.fontSize = 6;
                    tileLevelText.text = TileSystem.instance.GetTile(x,y).level.ToString();
                    tileHeightVisualizersDictionary[new Vector2Int(x, y)] = tileLevelText;

                }
            }
        }
        void ToggleShowTileHeight()
        {
            if (heightShow == false)
            {
                transformParentToInstantiate.position = new Vector3(-100000, -100000);
                //transformParentToInstantiate.gameObject.SetActive(true);
                heightShow = true;
            }
            else if (heightShow == true)
            {
                transformParentToInstantiate.position = new Vector3(0, 0);
                //transformParentToInstantiate.gameObject.SetActive(false);
                heightShow = false;
            }
        }
        
    }
}