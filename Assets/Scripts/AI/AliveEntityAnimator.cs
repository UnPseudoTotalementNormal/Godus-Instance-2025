using System;
using UnityEngine;

namespace AI
{
    public class AliveEntityAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [SerializeField] private AttackComponent attackComponent;
        [SerializeField] private HealthComponent healthComponent;
        
        private Vector2 lastPosition = Vector2.zero;

        private readonly int isWalkingKey = Animator.StringToHash("isWalking");
        private readonly int isAttacking = Animator.StringToHash("isAttacking");

        private void Awake()
        {
            if (attackComponent)
            {
                attackComponent.onAttack += OnAttack;
            }
        }

        private void LateUpdate()
        {
            Vector2 _currentPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 _movementDelta = _currentPosition - lastPosition;
            lastPosition = _currentPosition;
            bool _isWalking = _movementDelta.magnitude > 0.01f;
            animator.SetBool(isWalkingKey, _isWalking);
            
            if (!_isWalking)
            {
                return;
            }
            
            if (Math.Sign(_movementDelta.x) > 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
            
        }

        private void OnAttack()
        {
            animator.SetTrigger(isAttacking);
        }
    }
}
