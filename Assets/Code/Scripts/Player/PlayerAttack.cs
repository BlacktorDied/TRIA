using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    #region Variables

    [Header("Attack Settings")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float attackDelay = 0.2f;
    [SerializeField] private Transform upAttackPoint, sideAttackPoint, downAttackPoint;
    [SerializeField] private Vector2 upAttackArea, sideAttackArea, downAttackArea;
    [SerializeField] private LayerMask attackableLayer;

    private PlayerInputHandler input;
    private PlayerMovement movement;
    private Animator anim;

    private float timeSinceAttack;

    #endregion

    #region Unity Methods

    void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
        movement = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Attack();
    }

    #endregion

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;

        if (!input.AttackPressed) return;
        if (timeSinceAttack < attackDelay) return;

        timeSinceAttack = 0f;

        if (input.MoveInput.y > 0.5)
        {
            Hit(upAttackPoint, upAttackArea);
            Debug.Log("Up Attack!");
             //anim.SetTrigger("AttackUp");
        }
        else if (input.MoveInput.y < -0.5 && !movement.IsGrounded)
        {
            Hit(downAttackPoint, downAttackArea);
            Debug.Log("Down Attack!");
            // anim.SetTrigger("AttackDown");
        }
        else
        {
            Hit(sideAttackPoint, sideAttackArea);
            Debug.Log("Side Attack!");
             anim.SetTrigger("Attacking");
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0f, attackableLayer);
        
        foreach (Collider2D enemy in objectToHit)
        {
            Debug.Log("Hit " + enemy.name);

            enemy.GetComponent<Enemy>()?.EnemyHit(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (upAttackPoint != null) Gizmos.DrawWireCube(upAttackPoint.position, upAttackArea);
        if (sideAttackPoint != null) Gizmos.DrawWireCube(sideAttackPoint.position, sideAttackArea);
        if (downAttackPoint != null) Gizmos.DrawWireCube(downAttackPoint.position, downAttackArea);
    }
}
