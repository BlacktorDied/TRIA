using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    #region Variables

    [Header("Attack Settings")]
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private LayerMask enemyLayer;

    private PlayerInputHandler input;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        PerformAttack();
    }

    #endregion

    #region Attack Logic
    private void PerformAttack()
    {
        if (input.AttackPressed)
        {
            Debug.Log("ATTACK!");

            // detect enemies
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                attackCheck.position,
                attackRadius,
                enemyLayer
            );

            foreach (var hit in hits)
            {
                Debug.Log("Hit: " + hit.name);

                // If enemy has health script:
                // hit.GetComponent<EnemyHealth>()?.TakeDamage(1);
            }
        }
    }
    #endregion
    private void OnDrawGizmosSelected()
    {
        if (attackCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackCheck.position, attackRadius);
    }
}
