using System;
using System.Collections.Generic;
using System.Linq;
using TileSystemSpace;
using Unity.Cinemachine;
using UnityEngine;

namespace Powers
{
    public class Meteorite : MonoBehaviour
    {
        private Vector2Int targetPosition;

        [SerializeField] private float minTargetDistanceAtSpawn;
        public float fallAcceleration = 10f;
        private float currentFallSpeed;
        
        [SerializeField] float objectSize = 1f;
        
        public int explosionRadius = 3;
        public TileSystem.RadiusMode radiusMode = TileSystem.RadiusMode.Circle;
        
        public float damageAmount = 50;

        public int minTileDigLevel = 1;
        public int maxTileDigLevel = 2;
        
        private UnityEngine.Camera mainCamera;
        
        [SerializeField] private CinemachineImpulseSource impulseSource;
        public float impulseForce = 2;
        public float impulseDuration = 2;

        public event Action onExplode;

        private void Awake()
        {
            mainCamera = UnityEngine.Camera.main;
        }

        private void SpawnMeteorite()
        {
            float _cameraHeight = mainCamera.orthographicSize * 2f;
            float _cameraWidth = _cameraHeight * mainCamera.aspect;
            
            float _randomX = UnityEngine.Random.Range(-1f, 1f);
            
            Vector3 _cameraPos = mainCamera.transform.position;
            float _spawnX = _cameraPos.x + (_randomX * _cameraWidth * 0.5f);
            float _spawnY = _cameraPos.y + (_cameraHeight * 0.5f) + objectSize;

            Vector2 _spawnVector = new Vector2(_spawnX, _spawnY);
            
            if (Vector2.Distance(_spawnVector, targetPosition) <= minTargetDistanceAtSpawn)
            {
                Vector2 _direction = (_spawnVector - targetPosition).normalized;
                _spawnVector += _direction * minTargetDistanceAtSpawn;
            }
            
            transform.position = _spawnVector;
        }

        private void Update()
        {
            currentFallSpeed += fallAcceleration * Time.deltaTime;
            
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentFallSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                OnExplode();
                onExplode?.Invoke();
                Destroy(gameObject);
            }
        }
        
        public void LandAtPosition(Vector2Int _worldPos)
        {
            targetPosition = _worldPos;
            
            SpawnMeteorite();
        }

        private void OnExplode()
        {
            Dictionary<Tile, Vector2Int> _tilesInRadius = TileSystem.instance.GetAllTilesAtPointWithRadius(targetPosition, explosionRadius, radiusMode);

            foreach (KeyValuePair<Tile, Vector2Int> _tile in _tilesInRadius)
            {
                if (_tile.Key.level <= 0)
                {
                    continue;
                }
                _tile.Key.level = 
                    Mathf.Clamp(_tile.Key.level - UnityEngine.Random.Range(minTileDigLevel, maxTileDigLevel + 1), 0, GameValues.MAX_TILE_HEIGHT - 1);
            }
            
            Collider2D[] _hitColliders = Physics2D.OverlapCircleAll(targetPosition, explosionRadius);
            List<HealthComponent> _damagedEntities = _hitColliders
                .Select(_h => _h.GetComponentInParent<HealthComponent>())
                .Where(_hc => _hc != null)
                .Distinct().ToList();
            foreach (HealthComponent _entity in _damagedEntities)
            {
                _entity.TakeDamage(damageAmount);
            }
            
            impulseSource.ImpulseDefinition.ImpulseDuration = impulseDuration;
            impulseSource.GenerateImpulseWithForce(impulseForce);
        }
    }
}