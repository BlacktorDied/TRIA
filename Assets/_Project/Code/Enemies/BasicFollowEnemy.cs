using UnityEngine;

public class BasicFollowEnemy : Enemy
{
    #region Variables

    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private Transform[] points;

    [Header("Player Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 5f;

    private int i = 0;
    private Vector3 patrolTarget;
    private SpriteRenderer spriteRenderer;

    private bool isFollowingPlayer = false;
    private Vector3 returnTarget;

    #endregion

    #region Unity Methods

    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (points.Length == 0)
        {
            Debug.LogWarning("No patrol points assigned for FollowEnemy!");
        }
        else
        {
            patrolTarget = points[i].position;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (points.Length == 0 || !player) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Start following player
            isFollowingPlayer = true;
            returnTarget = transform.position; // Save current position as return point
            MoveTowards(player.position);
        }
        else
        {
            // Normal patrol behavior
            MoveTowards(patrolTarget);

            if (Vector2.Distance(transform.position, patrolTarget) < 0.25f)
            {
                i++;

                if (i >= points.Length) i = 0;

                patrolTarget = points[i].position;
            }
        }

        if (isFollowingPlayer)
        {
            if (Vector2.Distance(transform.position, returnTarget) < 0.1f)
            {
                isFollowingPlayer = false;
                patrolTarget = points[i].position;
            }
            else
            {
                MoveTowards(returnTarget);
            }
        }
    }

    #endregion

    void MoveTowards(Vector3 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        spriteRenderer.flipX = (target.x - transform.position.x) < 0;
    }
}
