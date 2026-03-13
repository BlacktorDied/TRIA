using TRIA.Core;
using UnityEngine;

namespace TRIA.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField]
        private float damage = 1f;

        [SerializeField]
        private float attackDelay = 0.3f;

        [Header("Hitbox")]
        [SerializeField]
        private Vector2 attackOffset = new Vector2(1.2f, 0f);

        [SerializeField]
        private Vector2 attackSize = new Vector2(1.5f, 1.5f);

        [SerializeField]
        private LayerMask attackableLayer;

        private Animator _anim;
        private SpriteRenderer _sprite;
        private PlayerAudio _audio;
        private float _timeSinceAttack;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _sprite = GetComponent<SpriteRenderer>();
            _audio = GetComponent<PlayerAudio>();

            if (GameManager.Instance != null)
                damage = GameManager.Instance.playerAttack;
        }

        private void Update() => _timeSinceAttack += Time.deltaTime;

        public void OnAttack()
        {
            if (
                GameManager.Instance != null
                && GameManager.Instance.CurrentState != GameState.Gameplay
            )
                return;
            if (_timeSinceAttack < attackDelay)
                return;

            PerformAttack();
        }

        private void PerformAttack()
        {
            _timeSinceAttack = 0f;
            _anim?.SetTrigger("Attacking");
            _audio?.PlayAttack();

            float direction = _sprite.flipX ? -1f : 1f;
            Vector2 attackCenter =
                (Vector2)transform.position
                + new Vector2(attackOffset.x * direction, attackOffset.y);

            Collider2D[] hits = Physics2D.OverlapBoxAll(
                attackCenter,
                attackSize,
                0f,
                attackableLayer
            );

            foreach (Collider2D hit in hits)
            {
                // Look for the interface in Core
                IDamageable target = hit.GetComponent<IDamageable>();
                if (target != null)
                {
                    target.TakeDamage(damage);
                }
            }
        }

        public void IncreaseAttack(float amount) => damage += amount;

        private void OnDrawGizmosSelected()
        {
            if (_sprite == null)
                _sprite = GetComponent<SpriteRenderer>();
            float dir = (_sprite != null && _sprite.flipX) ? -1f : 1f;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                transform.position + new Vector3(attackOffset.x * dir, attackOffset.y, 0),
                attackSize
            );
        }
    }
}
