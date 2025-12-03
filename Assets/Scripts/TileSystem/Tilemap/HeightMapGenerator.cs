using UnityEngine;

namespace TileSystemSpace.Tilemap
{
    [RequireComponent(typeof(UnityEngine.Tilemaps.Tilemap))]
    public class HeightMapGenerator : MonoBehaviour
    {
        [SerializeField] private Material tilemapMaterial; // le material utilisé par ta Tilemap
        [SerializeField] private string heightTexPropertyName = "_HeightTex";

        private TileSystem tileSystem;
        private Texture2D heightTex;

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

        private void OnAnyTileChanged(Tile tile, Vector2Int pos)
        {
            // Version simple : régénérer toute la heightmap.
            // Tu pourras optimiser plus tard si besoin.
            GenerateHeightMapTexture();
            UpdateMaterial();
        }

        private void GenerateHeightMapTexture()
        {
            Vector2Int size = tileSystem.GetSize();
            int width = size.x;
            int height = size.y;

            if (heightTex == null || heightTex.width != width || heightTex.height != height)
            {
                heightTex = new Texture2D(width, height, TextureFormat.R8, false);
                heightTex.filterMode = FilterMode.Point;
                heightTex.wrapMode = TextureWrapMode.Clamp;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Tile t = tileSystem.GetTile(x, y);
                    int level = t != null ? t.level : 0;

                    byte h = (byte)Mathf.Clamp(level, 0, 255);
                    Color32 c = new Color32(h, 0, 0, 255);
                    heightTex.SetPixel(x, y, c);
                }
            }

            heightTex.Apply();
        }

        private void UpdateMaterial()
        {
            if (tilemapMaterial != null && heightTex != null)
            {
                tilemapMaterial.SetTexture(heightTexPropertyName, heightTex);
            }
        }
    }
}
