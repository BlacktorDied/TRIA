using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class FollowEnemy : Enemy
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

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
    }

    protected override void Update()
    {
        base.Update();
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
