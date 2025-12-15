using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace TileSystemSpace
{
    public class TileHeigthVisualizer : MonoBehaviour
    {
        private ObjectPool<TextMeshPro> textMeshPool;

        [SerializeField] private bool heightShow;
        private Dictionary<Vector2Int, TextMeshPro> tileHeightVisualizersDictionary = new();
        [SerializeField] private Transform transformParentToInstantiate;

        private Vector2 tempPosition;

        [SerializeField] private float cameraBoundsMargin = 2f;
        [SerializeField] private int defaultPoolCapacity = 100;
        [SerializeField] private int maxPoolSize = 1000;
        
        private UnityEngine.Camera mainCamera;

        [SerializeField] private float tickTime = 0.25f;
        private float tickTimer;

        private void Awake()
        {
            InitializePool();
        }

        private void Start()
        {
            mainCamera = UnityEngine.Camera.main;
            InputManager.instance.onShowCoordinatePressStarted += ToggleShowTileHeight;
            TileSystem.instance.onAnyTileChanged += OnAnyTileChanged;
            ToggleShowTileHeight();
        }

        private void Update()
        {
            tickTimer += Time.deltaTime;
            
            if (heightShow)
            {
                return;
            }
            
            if (tickTimer < tickTime)
            {
                return;
            }
            tickTimer = 0f;
            
            UpdateVisibleTiles();
        }
        
        private void UpdateVisibleTiles()
        {
            (Vector2Int _min, Vector2Int _max) _visibleTiles = GetVisibleTileBounds();

            Vector2Int _min = _visibleTiles._min;
            Vector2Int _max = _visibleTiles._max;
        
            List<Vector2Int> keysToRemove = new();
            foreach (var _kvp in tileHeightVisualizersDictionary)
            {
                if (_kvp.Key.x >= _min.x && _kvp.Key.x <= _max.x &&
                    _kvp.Key.y >= _min.y && _kvp.Key.y <= _max.y)
                {
                    continue;
                }
            
                keysToRemove.Add(_kvp.Key);
            }
        
            foreach (Vector2Int _key in keysToRemove)
            {
                textMeshPool.Release(tileHeightVisualizersDictionary[_key]);
                tileHeightVisualizersDictionary.Remove(_key);
            }
        
            for (int _x = _min.x; _x <= _max.x; _x++)
            {
                for (int _y = _min.y; _y <= _max.y; _y++)
                {
                    Vector2Int _position = new Vector2Int(_x, _y);
                    
                    if (!tileHeightVisualizersDictionary.TryGetValue(_position, out TextMeshPro _textMesh))
                    {
                        _textMesh = textMeshPool.Get();
                        tileHeightVisualizersDictionary.Add(_position, _textMesh);
                        
                        tempPosition.x = _x + 9.9f;
                        tempPosition.y = _y - 2.2f;
                        _textMesh.transform.localPosition = tempPosition;
                        _textMesh.fontSize = 6;
                    }
                
                    Tile _tile = TileSystem.instance.GetTile(_x, _y);
                    _textMesh.text = _tile.level.ToString();
                }
            }
        }

        private void OnAnyTileChanged(Tile _tile, Vector2Int _position)
        {
            if (tileHeightVisualizersDictionary.TryGetValue(_position, out TextMeshPro _textMesh))
            {
                _textMesh.text = _tile.level.ToString();
            }
        }
        
        private void ToggleShowTileHeight()
        {
            heightShow = !heightShow;
            transformParentToInstantiate.position = heightShow
                ? new Vector3(-100000, -100000)
                : Vector3.zero;
        }
        
        private (Vector2Int _min, Vector2Int _max) GetVisibleTileBounds()
        {
            if (mainCamera == null) return (Vector2Int.zero, Vector2Int.zero);

            Vector3 _bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
            Vector3 _topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

            Vector2Int _gridSize = TileSystem.instance.GetSize();
            
            int _minX = Mathf.Max(0, Mathf.FloorToInt(_bottomLeft.x - cameraBoundsMargin));
            int _minY = Mathf.Max(0, Mathf.FloorToInt(_bottomLeft.y - cameraBoundsMargin));
            int _maxX = Mathf.Min(_gridSize.x - 1, Mathf.CeilToInt(_topRight.x + cameraBoundsMargin));
            int _maxY = Mathf.Min(_gridSize.y - 1, Mathf.CeilToInt(_topRight.y + cameraBoundsMargin));

            return (new Vector2Int(_minX, _minY), new Vector2Int(_maxX, _maxY));
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
        
        private void InitializePool()
        {
            textMeshPool = new ObjectPool<TextMeshPro>(
                createFunc: CreateTextMesh,
                actionOnGet: OnGetFromPool,
                actionOnRelease: OnReleaseToPool,
                actionOnDestroy: OnDestroyPoolObject,
                collectionCheck: true,
                defaultCapacity: defaultPoolCapacity,
                maxSize: maxPoolSize
            );
        }
        
        private TextMeshPro CreateTextMesh()
        {
            var _go = new GameObject("LevelText");
            var _tmp = _go.AddComponent<TextMeshPro>();
            _go.transform.SetParent(transformParentToInstantiate, false);
            _go.transform.localPosition = Vector3.one * 999999;
            
            return _tmp;
        }
        
        private void OnGetFromPool(TextMeshPro _obj)
        {
            
        }
        
        private void OnReleaseToPool(TextMeshPro _obj)
        {
            _obj.transform.localPosition = Vector3.one * 999999;
        }
        
        private void OnDestroyPoolObject(TextMeshPro _obj)
        {
            Destroy(_obj);
        }
    }
}