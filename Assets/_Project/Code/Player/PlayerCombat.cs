using UnityEngine;

namespace TRIA.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField]
        private float damage = 1f;

        [SerializeField]
        private float attackDelay = 0.2f;

        [Header("Hitboxes")]
        [SerializeField]
        private Transform upAttackPoint;

        [SerializeField]
        private Transform sideAttackPoint;

        [SerializeField]
        private Transform downAttackPoint;

        [SerializeField]
        private Vector2 upAttackArea,
            sideAttackArea,
            downAttackArea;

        [SerializeField]
        private LayerMask attackableLayer;

        private PlayerAudio _playerAudio;
        private Animator _anim;
        private PlayerMovement _movement;

        private Vector2 _moveInput;
        private float _timeSinceAttack;

        private void Awake()
        {
            _playerAudio = GetComponent<PlayerAudio>();
            _anim = GetComponent<Animator>();
            _movement = GetComponent<PlayerMovement>();
        }

        public void SetMoveInput(Vector2 input) => _moveInput = input;

        private void Update()
        {
            _timeSinceAttack += Time.deltaTime;
        }

        public void AttemptAttack()
        {
            if (_timeSinceAttack < attackDelay)
                return;

            _timeSinceAttack = 0f;
            _playerAudio?.PlayAttack();

            if (_moveInput.y > 0.5f)
            {
                Hit(upAttackPoint, upAttackArea);
                // _anim.SetTrigger("AttackUp");
            }
            else if (_moveInput.y < -0.5f && !_movement.IsGrounded)
            {
                Hit(downAttackPoint, downAttackArea);
                // _anim.SetTrigger("AttackDown");
            }
            else
            {
                Hit(sideAttackPoint, sideAttackArea);
                _anim?.SetTrigger("Attacking");
            }
        }

        private void Hit(Transform attackTransform, Vector2 attackArea)
        {
            Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(
                attackTransform.position,
                attackArea,
                0f,
                attackableLayer
            );

            foreach (Collider2D enemyCollider in objectsToHit)
            {
                // Note: Assuming your Enemy script has a namespace or is globally accessible
                //enemyCollider.GetComponent<Enemy>()?.EnemyHit(damage);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if (upAttackPoint != null)
                Gizmos.DrawWireCube(upAttackPoint.position, upAttackArea);
            if (sideAttackPoint != null)
                Gizmos.DrawWireCube(sideAttackPoint.position, sideAttackArea);
            if (downAttackPoint != null)
                Gizmos.DrawWireCube(downAttackPoint.position, downAttackArea);
        }
    }
}
