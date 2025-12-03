using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileSystemSpace.Tilemap
{
    [RequireComponent(typeof(UnityEngine.Tilemaps.Tilemap))]
    public class HeightMapDebugger : MonoBehaviour
    {
        [SerializeField] private Material tilemapMaterial;
        private TileSystem tileSystem;
        private UnityEngine.Tilemaps.Tilemap tilemap;

        private void Start()
        {
            tileSystem = TileSystem.instance;
            tilemap = GetComponent<UnityEngine.Tilemaps.Tilemap>();
            
            DebugInfo();
        }

        private void DebugInfo()
        {
            if (tileSystem == null)
            {
                Debug.LogError("TileSystem is null!");
                return;
            }

            Vector2Int size = tileSystem.GetSize();
            Debug.Log($"=== HEIGHTMAP DEBUG INFO ===");
            Debug.Log($"TileSystem Size: {size.x} x {size.y}");
            
            // Test quelques tuiles
            Debug.Log($"\n--- Tile Heights ---");
            for (int x = 0; x < Mathf.Min(5, size.x); x++)
            {
                for (int y = 0; y < Mathf.Min(5, size.y); y++)
                {
                    Tile t = tileSystem.GetTile(x, y);
                    int level = t != null ? t.level : 0;
                    Debug.Log($"Tile ({x},{y}): level={level}");
                }
            }

            // Test les coordonnées monde de la Tilemap
            Debug.Log($"\n--- Tilemap World Coordinates ---");
            Debug.Log($"Tilemap Transform Position: {tilemap.transform.position}");
            Debug.Log($"Tilemap LocalToWorld Matrix: {tilemap.transform.localToWorldMatrix}");
            
            Vector3Int testCell = new Vector3Int(0, 0, 0);
            Vector3 worldPos = tilemap.CellToWorld(testCell);
            Debug.Log($"Cell (0,0) -> World: {worldPos}");
            
            testCell = new Vector3Int(1, 1, 0);
            worldPos = tilemap.CellToWorld(testCell);
            Debug.Log($"Cell (1,1) -> World: {worldPos}");

            // Test material properties
            if (tilemapMaterial != null)
            {
                Debug.Log($"\n--- Material Properties ---");
                if (tilemapMaterial.HasProperty("_TileCount"))
                {
                    Vector4 tileCount = tilemapMaterial.GetVector("_TileCount");
                    Debug.Log($"_TileCount: {tileCount}");
                }
                if (tilemapMaterial.HasProperty("_PixelsPerTile"))
                {
                    float ppt = tilemapMaterial.GetFloat("_PixelsPerTile");
                    Debug.Log($"_PixelsPerTile: {ppt}");
                }
                if (tilemapMaterial.HasProperty("_LightDir"))
                {
                    Vector4 lightDir = tilemapMaterial.GetVector("_LightDir");
                    Debug.Log($"_LightDir: {lightDir}");
                }
                if (tilemapMaterial.HasProperty("_ShadowStrength"))
                {
                    float strength = tilemapMaterial.GetFloat("_ShadowStrength");
                    Debug.Log($"_ShadowStrength: {strength}");
                }
                if (tilemapMaterial.HasProperty("_HeightTex"))
                {
                    Texture heightTex = tilemapMaterial.GetTexture("_HeightTex");
                    if (heightTex != null)
                    {
                        Debug.Log($"_HeightTex: {heightTex.width}x{heightTex.height}");
                    }
                    else
                    {
                        Debug.LogWarning("_HeightTex is NULL!");
                    }
                }
            }
            else
            {
                Debug.LogError("Material is NULL!");
            }
        }

        private void OnDrawGizmos()
        {
            if (tileSystem == null || tilemap == null) return;

            // Dessiner les coordonnées monde pour quelques tuiles
            Vector2Int size = tileSystem.GetSize();
            for (int x = 0; x < Mathf.Min(5, size.x); x++)
            {
                for (int y = 0; y < Mathf.Min(5, size.y); y++)
                {
                    Vector3Int cellPos = new Vector3Int(x, y, 0);
                    Vector3 worldPos = tilemap.CellToWorld(cellPos);
                    
                    Tile t = tileSystem.GetTile(x, y);
                    if (t != null)
                    {
                        // Couleur basée sur la hauteur
                        Gizmos.color = Color.Lerp(Color.blue, Color.red, t.level / 10f);
                        Gizmos.DrawWireCube(worldPos + new Vector3(0.5f, 0.5f, 0), new Vector3(0.9f, 0.9f, 0.1f));
                    }
                }
            }
        }
    }
}

