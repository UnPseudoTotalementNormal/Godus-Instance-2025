using System.Collections.Generic;
using UnityEngine;

namespace FireSystem
{
    public class FireManager : MonoBehaviour
    {
        public static FireManager instance { get; private set; }
        
        private Dictionary<Vector2Int, Fire> activeFires = new();

        [SerializeField] private Fire firePrefab;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        public void IgniteTile(Vector2Int _tilePosition)
        {
            if (IsTileOnFire(_tilePosition))
            {
                return;
            }
            
            Fire _newFire = Instantiate(firePrefab, 
                new Vector2(_tilePosition.x, _tilePosition.y), 
                Quaternion.identity);

            _newFire.onFireExtinguished += () =>
            {
                OnFireExtinguished(_tilePosition);
            };
            
            activeFires.Add(_tilePosition, _newFire);
        }
        
        public void ExtinguishFireAtTile(Vector2Int _tilePosition)
        {
            if (!activeFires.ContainsKey(_tilePosition))
            {
                return;
            }

            Fire _fire = activeFires[_tilePosition];
            _fire.ExtinguishFire();
        }

        private void OnFireExtinguished(Vector2Int _tilePosition)
        {
            if (!activeFires.ContainsKey(_tilePosition))
            {
                return;
            }
            
            activeFires.Remove(_tilePosition);
        }
        
        public bool IsTileOnFire(Vector2Int _tilePosition)
        {
            TileSystemSpace.Tile _tile = TileSystemSpace.TileSystem.instance.GetTile(_tilePosition);
            return _tile != null && activeFires.ContainsKey(_tilePosition);
        }
    }
}