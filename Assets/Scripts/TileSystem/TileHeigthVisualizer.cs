using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace TileSystemSpace

{
    public class TileHeigthVisualizer : MonoBehaviour
    {
        private bool HeigthShow = false;
        Dictionary<Vector2Int, TextMeshPro> _tileHeightVisualizers = new();
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
            _tileHeightVisualizers[arg2].text = TileSystem.instance.GetTile(arg2).level.ToString();
        }
        private void GetTileHeight()
        {
            TileSystem.instance.GetSize();
            for (int x = 0; x < TileSystem.instance.GetSize().x; x++)
            {
                for (int y = 0; y < TileSystem.instance.GetSize().y; y++)
                {
                    TileSystem.instance.GetTile(x,y);
                    GameObject go = new GameObject("test");
                    TextMeshPro _testText = go.AddComponent<TextMeshPro>();
                    go.transform.SetParent(transformParentToInstantiate);
                    _testText.transform.position = new Vector2(x+9.9f, y-2.2f);
                    _testText.fontSize = 6;
                    _testText.text = TileSystem.instance.GetTile(x,y).level.ToString();
                    _tileHeightVisualizers[new Vector2Int(x, y)] = _testText;

                }
            }
        }
        void ToggleShowTileHeight()
        {
            if (HeigthShow == false)
            {
                transformParentToInstantiate.position = new Vector3(-100000, -100000);
                //transformParentToInstantiate.gameObject.SetActive(true);
                HeigthShow = true;
            }
            else if (HeigthShow == true)
            {
                transformParentToInstantiate.position = new Vector3(0, 0);
                //transformParentToInstantiate.gameObject.SetActive(false);
                HeigthShow = false;
            }
        }
        
    }
}