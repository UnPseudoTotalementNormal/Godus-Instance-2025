using System;
using System.Collections.Generic;
using UnityEngine;

namespace TileSystemSpace.Tilemap
{
    [RequireComponent(typeof(UnityEngine.Tilemaps.Tilemap))]
    public class HeightMapGenerator : MonoBehaviour
    {
        [SerializeField] private Material tilemapMaterial;
        [SerializeField] private Material entityMaterial;
        [SerializeField] private string heightTexPropertyName = "_HeightTex";
        [SerializeField] private int pixelsPerTile = 4;

        private TileSystem tileSystem;
        private Texture2D heightTex;

        private bool hasUpdatedTexture = false;

        private void Start()
        {
            tileSystem = TileSystem.instance;

            GenerateHeightMapTexture();
            UpdateMaterial();

            tileSystem.onAnyTileChanged += OnAnyTileChanged;
        }

        private void OnDestroy()
        {
            if (tileSystem != null)
                tileSystem.onAnyTileChanged -= OnAnyTileChanged;
        }

        private void LateUpdate()
        {
            if (hasUpdatedTexture)
            {
                heightTex.Apply();
                UpdateMaterial();
                hasUpdatedTexture = false;
            }
        }

        private void OnAnyTileChanged(Tile _tile, Vector2Int _pos)
        {
            Color32 _c = GetHeightColor(_pos.x, _pos.y);
            int _startX = _pos.x * pixelsPerTile;
            int _startY = _pos.y * pixelsPerTile;

            for (int _px = 0; _px < pixelsPerTile; _px++)
            {
                for (int _py = 0; _py < pixelsPerTile; _py++)
                {
                    heightTex.SetPixel(_startX + _px, _startY + _py, _c);
                }
            }
            
            hasUpdatedTexture = true;
        }

        private void GenerateHeightMapTexture()
        {
            Vector2Int _size = tileSystem.GetSize();
            int _tileWidth = _size.x;
            int _tileHeight = _size.y;

            int _texWidth = _tileWidth * pixelsPerTile;
            int _texHeight = _tileHeight * pixelsPerTile;

            if (heightTex == null || heightTex.width != _texWidth || heightTex.height != _texHeight)
            {
                heightTex = new Texture2D(_texWidth, _texHeight, TextureFormat.R8, false);
                heightTex.filterMode = FilterMode.Point;
                heightTex.wrapMode = TextureWrapMode.Clamp;
            }

            for (int _px = 0; _px < _texWidth; _px++)
            {
                for (int _py = 0; _py < _texHeight; _py++)
                {
                    float _tileFX = (_px + 0.5f) / pixelsPerTile;
                    float _tileFy = (_py + 0.5f) / pixelsPerTile;

                    int _tileX = Mathf.FloorToInt(_tileFX);
                    int _tileY = Mathf.FloorToInt(_tileFy);

                    var _c = GetHeightColor(_tileX, _tileY);
                    heightTex.SetPixel(_px, _py, _c);
                }
            }

            heightTex.Apply();
        }

        private Color32 GetHeightColor(int _tileX, int _tileY)
        {
            Tile _t = tileSystem.GetTile(_tileX, _tileY);
            int _level = _t?.level ?? 0;

            byte _h = (byte)Mathf.Clamp(_level, 0, 255);
            Color32 _c = new Color32(_h, 0, 0, 255);
            return _c;
        }

        private void UpdateMaterial()
        {
            if (tilemapMaterial != null && heightTex != null)
            {
                Vector2Int size = tileSystem.GetSize();

                tilemapMaterial.SetTexture(heightTexPropertyName, heightTex);
                tilemapMaterial.SetFloat("_PixelsPerTile", pixelsPerTile);
                tilemapMaterial.SetVector("_TileCount", new Vector4(size.x, size.y, 0, 0));
                
                if (entityMaterial != null)
                {
                    entityMaterial.SetTexture(heightTexPropertyName, heightTex);
                    entityMaterial.SetFloat("_PixelsPerTile", pixelsPerTile);
                    entityMaterial.SetVector("_TileCount", new Vector4(size.x, size.y, 0, 0));
                    
                    //Copy all values
                    entityMaterial.SetVector("_ShadowsOffset", tilemapMaterial.GetVector("_ShadowsOffset"));
                    entityMaterial.SetVector("_LightDir", tilemapMaterial.GetVector("_LightDir"));
                    entityMaterial.SetFloat("_ShadowStrength", tilemapMaterial.GetFloat("_ShadowStrength"));
                    entityMaterial.SetFloat("_MaxShadowOpacity", tilemapMaterial.GetFloat("_MaxShadowOpacity"));
                    entityMaterial.SetFloat("_MinLightMarchStepSize", tilemapMaterial.GetFloat("_MinLightMarchStepSize"));
                    entityMaterial.SetFloat("_MaxLightMarchStepSize", tilemapMaterial.GetFloat("_MaxLightMarchStepSize"));
                    entityMaterial.SetFloat("_TileHeightPerLevelHeight", tilemapMaterial.GetFloat("_TileHeightPerLevelHeight"));
                }
            }
        }
    }
}
