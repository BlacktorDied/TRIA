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

        [Header("Hitbox (No Child Object)")]
        [SerializeField]
        private Vector2 attackOffset = new Vector2(1f, 0f); // How far in front of player

        [SerializeField]
        private Vector2 attackSize = new Vector2(1.5f, 1.5f);

        [SerializeField]
        private LayerMask attackableLayer;

        private Animator _anim;
        private SpriteRenderer _sprite;
        private float _timeSinceAttack;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _sprite = GetComponent<SpriteRenderer>();

            if (GameManager.Instance != null)
                damage = GameManager.Instance.playerAttack;
        }

        private void Update() => _timeSinceAttack += Time.deltaTime;

        // Called by Player Input (SendMessages)
        public void OnAttack()
        {
            if (GameManager.Instance.CurrentState == GameState.Pause)
                return;
            if (_timeSinceAttack < attackDelay)
                return;

            _timeSinceAttack = 0f;
            _anim?.SetTrigger("Attacking");

            // CALCULATE HITBOX CENTER
            // We flip the X offset based on which way the sprite is facing
            float direction = _sprite.flipX ? -1f : 1f;
            Vector2 finalOffset = new Vector2(attackOffset.x * direction, attackOffset.y);
            Vector2 attackCenter = (Vector2)transform.position + finalOffset;

            // DETECT HITS
            Collider2D[] hits = Physics2D.OverlapBoxAll(
                attackCenter,
                attackSize,
                0f,
                attackableLayer
            );

            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == gameObject)
                    continue;

                // Example: hit.GetComponent<Enemy>()?.TakeDamage(damage);
                Debug.Log($"Hit {hit.name} for {damage} damage!");
            }
        }

        public void IncreaseAttack(float amount)
        {
            damage += amount;
        }

        // Draw the hitbox in the editor so you can see it without a child object
        private void OnDrawGizmosSelected()
        {
            if (_sprite == null)
                _sprite = GetComponent<SpriteRenderer>();
            float direction = (_sprite != null && _sprite.flipX) ? -1f : 1f;
            Vector3 pos =
                transform.position + new Vector3(attackOffset.x * direction, attackOffset.y, 0);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(pos, attackSize);
        }
    }
}
