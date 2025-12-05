using UnityEngine;

namespace TileSystemSpace.Tilemap
{
    [RequireComponent(typeof(UnityEngine.Tilemaps.Tilemap))]
    public class HeightMapGenerator : MonoBehaviour
    {
        [SerializeField] private Material tilemapMaterial;
        [SerializeField] private string heightTexPropertyName = "_HeightTex";
        [SerializeField] private int pixelsPerTile = 4;

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
            GenerateHeightMapTexture();
            UpdateMaterial();
        }

        private void GenerateHeightMapTexture()
        {
            Vector2Int size = tileSystem.GetSize();
            int tileWidth = size.x;
            int tileHeight = size.y;

            int texWidth = tileWidth * pixelsPerTile;
            int texHeight = tileHeight * pixelsPerTile;

            if (heightTex == null || heightTex.width != texWidth || heightTex.height != texHeight)
            {
                heightTex = new Texture2D(texWidth, texHeight, TextureFormat.R8, false);
                heightTex.filterMode = FilterMode.Bilinear;
                heightTex.wrapMode = TextureWrapMode.Clamp;
            }

            for (int px = 0; px < texWidth; px++)
            {
                for (int py = 0; py < texHeight; py++)
                {
                    float tileFX = (px + 0.5f) / pixelsPerTile;
                    float tileFY = (py + 0.5f) / pixelsPerTile;

                    int tileX = Mathf.FloorToInt(tileFX);
                    int tileY = Mathf.FloorToInt(tileFY);

                    Tile t = tileSystem.GetTile(tileX, tileY);
                    int level = t != null ? t.level : 0;

                    byte h = (byte)Mathf.Clamp(level, 0, 255);
                    Color32 c = new Color32(h, 0, 0, 255);
                    heightTex.SetPixel(px, py, c);
                }
            }

            heightTex.Apply();
        }

        private void UpdateMaterial()
        {
            if (tilemapMaterial != null && heightTex != null)
            {
                Vector2Int size = tileSystem.GetSize();

                tilemapMaterial.SetTexture(heightTexPropertyName, heightTex);
                tilemapMaterial.SetFloat("_PixelsPerTile", pixelsPerTile);
                tilemapMaterial.SetVector("_TileCount", new Vector4(size.x, size.y, 0, 0));
            }
        }
    }
}
