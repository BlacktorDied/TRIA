using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class BigEnemy : MonoBehaviour
{
    [Header("Movement & Detection")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;
    public float detectionRange = 6f;
    public float attackDistance = 1.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckDistance = 0.5f;
    public LayerMask groundLayer;

    [Header("Melee Hitboxes")]
    public GameObject swordHitboxFront;
    public GameObject swordHitboxTop;
    public float meleeCooldown = 1.5f;
    public float attackDuration = 0.3f;

    Rigidbody2D rb;
    Transform player;

    Vector2 patrolTarget;
    float meleeTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        patrolTarget = pointA.position;

        if (swordHitboxFront) swordHitboxFront.SetActive(false);
        if (swordHitboxTop) swordHitboxTop.SetActive(false);

        meleeTimer = 0f;
    }

    void FixedUpdate()
    {
        if (!player) return;

        meleeTimer -= Time.fixedDeltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        bool groundAhead = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        // Flip to face player
        bool playerIsLeft = player.position.x < transform.position.x;
        transform.localScale = new Vector3(playerIsLeft ? -1f : 1f, 1f, 1f);

        // -------------------- LOGIC --------------------

        if (distanceToPlayer > detectionRange)
        {
            Patrol(groundAhead);
        }
        else
        {
            if (distanceToPlayer <= attackDistance && meleeTimer <= 0f)
            {
                StartCoroutine(ActivateSwordHitboxes());
                meleeTimer = meleeCooldown;
            }
            else if (groundAhead)
            {
                Vector2 dir = (player.position - transform.position).normalized;
                rb.MovePosition(rb.position + dir * patrolSpeed * Time.fixedDeltaTime);
            }
        }
    }

    // -------------------- PATROL --------------------

    void Patrol(bool canMove)
    {
        if (!canMove) return;

        Vector2 dir = (patrolTarget - rb.position).normalized;
        rb.MovePosition(rb.position + dir * patrolSpeed * Time.fixedDeltaTime);

        if (Vector2.Distance(rb.position, patrolTarget) < 0.2f)
        {
            patrolTarget = patrolTarget == (Vector2)pointA.position
                ? pointB.position
                : pointA.position;
        }
    }

    // -------------------- MELEE --------------------

    IEnumerator ActivateSwordHitboxes()
    {
        if (swordHitboxFront)
        {
            swordHitboxFront.SetActive(true);
            StartCoroutine(FlashHitbox(swordHitboxFront));
        }

        if (swordHitboxTop)
        {
            swordHitboxTop.SetActive(true);
            StartCoroutine(FlashHitbox(swordHitboxTop));
        }

        yield return new WaitForSeconds(attackDuration);

        if (swordHitboxFront) swordHitboxFront.SetActive(false);
        if (swordHitboxTop) swordHitboxTop.SetActive(false);
    }

    IEnumerator FlashHitbox(GameObject hitbox)
    {
        SpriteRenderer sr = hitbox.GetComponent<SpriteRenderer>();
        if (!sr) yield break;

        Color original = sr.color;
        sr.color = Color.red;

        yield return new WaitForSeconds(attackDuration);

        sr.color = original;
    }

    // -------------------- DEBUG --------------------

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        if (groundCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                groundCheck.position,
                groundCheck.position + Vector3.down * groundCheckDistance
            );
        }
    }
}
