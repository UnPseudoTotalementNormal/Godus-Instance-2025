using System;
using TileSystemSpace;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FireSystem
{
    public class Fire : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [field:SerializeField] public float damagePerSecond { get; private set; }
        [field: SerializeField] public float spreadChancePerSecond { get; private set; } = 0;
        [field:SerializeField] public float minLifetime { get; private set; } = 5f;
        [field:SerializeField] public float maxLifetime { get; private set; } = 6f;
        [field:SerializeField] public float chanceToDamageTile { get; private set; } = 0.4f;

        private float lastDamageTime = -Mathf.Infinity;

        public float lifeTime { get; private set; }
        private float lifeTimer;

        private Tile tile;
        
        public event Action onFireExtinguished; 

        private void Awake()
        {
            lastDamageTime = Time.time;
            lifeTime = UnityEngine.Random.Range(minLifetime, maxLifetime);
            tile = TileSystem.instance.GetTile(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        }

        private void Start()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            animator.Play(stateInfo.fullPathHash, 0, Random.Range(0f, 1f));
        }

        private void Update()
        {
            lifeTimer += Time.deltaTime;
            
            if (Time.time >= lastDamageTime + 1f)
            {
                ApplyFireDamage();
                lastDamageTime = Time.time;
            }
            if (lifeTimer >= lifeTime)
            {
                OnFullyBurntOut();
                ExtinguishFire();
            }
        }

        private void OnFullyBurntOut()
        {
            float _rand = UnityEngine.Random.Range(0f, 1f);
            if (_rand <= chanceToDamageTile)
            {
                tile.tileType = TileType.DamagedDirt;
            }
        }

        public void ExtinguishFire()
        {
            onFireExtinguished?.Invoke();
            Destroy(gameObject);
        }

        private void ApplyFireDamage()
        {
            Collider2D[] _colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            foreach (Collider2D _collider in _colliders)
            {
                HealthComponent _healthComponent = _collider.GetComponentInParent<HealthComponent>();
                if (_healthComponent != null)
                {
                    _healthComponent.TakeDamage(damagePerSecond);
                }
            }
        }
    }
}