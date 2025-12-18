using System;
using AudioSystem;
using FMODUnity;
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
        
        [SerializeField] private EventReference walkingSfx;
        private FMOD.Studio.EventInstance walkingSfxInstance;
        private string walkingSfxKey => $"WalkingSfx_{gameObject.GetInstanceID()}";

        private void Awake()
        {
            if (attackComponent)
            {
                attackComponent.onAttack += OnAttack;
            }
        }

        private void FixedUpdate()
        {
            Vector2 _currentPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 _movementDelta = _currentPosition - lastPosition;
            lastPosition = _currentPosition;
            bool _isWalking = _movementDelta.magnitude > 0.001f;
            animator.SetBool(isWalkingKey, _isWalking);
            
            if (!_isWalking)
            {
                if (walkingSfxInstance.isValid())
                {
                    GameAudioManager.instance.StopEventInstance(walkingSfxKey);
                }
                return;
            }

            if (!walkingSfx.IsNull)
            {
                if (walkingSfxInstance.isValid())
                {
                    walkingSfxInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
                }
                else
                {
                    walkingSfxInstance = GameAudioManager.instance.PlayEventInstance(walkingSfx, walkingSfxKey).Value;
                }
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
