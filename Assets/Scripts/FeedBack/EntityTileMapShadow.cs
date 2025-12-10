using System;
using AI;
using TileSystemSpace;
using UnityEngine;

namespace Feedback
{
    public class EntityTileMapShadow : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public EntityHeightComponent heightComponent;
        private Material _materialInstance;
        
        private void Start()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            _materialInstance = spriteRenderer.material = Instantiate(spriteRenderer.material);
        }

        private void Update()
        {
            _materialInstance.SetFloat("_CurrentHeightLevel", heightComponent.heightLevel);
        }
    }
}