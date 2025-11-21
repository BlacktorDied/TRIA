using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FollowEnemy : MonoBehaviour
{
    #region Variables

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Player Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 8f;

    private Rigidbody2D rb;

    #endregion

    #region Unity Methods

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        if (Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        }
    }

    #endregion
}
