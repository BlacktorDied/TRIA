using UnityEngine;
using System.Collections;

public class ShooterEnemy : MonoBehaviour
{
    [Header("Movement")]
    public Transform pointA;
    public Transform pointB;
    public float wanderSpeed = 3.5f;
    public float retreatSpeed = 2f;
    public float stopChance = 0.25f;
    public Vector2 stopTimeRange = new Vector2(0.5f, 1.5f);

    [Header("Combat")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float bulletSpeed = 12f;
    public float bulletLifetime = 3f;
    public float detectionRange = 6f;
    public float retreatDistance = 2.2f;

    [Header("Burst Settings")]
    public int bulletsPerBurst = 3;
    public float timeBetweenShots = 0.2f;
    public float timeBetweenBursts = 1.2f;

    [Header("Ground Check")]
    public Transform groundCheck;           // Position in front of feet
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayer;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Transform player;

    Transform targetPoint;
    bool isStopping;
    float stopTimer;

    bool isShootingBurst = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        targetPoint = pointA;
    }

    void Update()
    {
        if (!player) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Face the player
        bool playerIsLeft = player.position.x < transform.position.x;
        sr.flipX = playerIsLeft;

        // Mirror shootPoint so it stays in front
        if (shootPoint)
        {
            Vector3 spLocal = shootPoint.localPosition;
            spLocal.x = Mathf.Abs(spLocal.x) * (playerIsLeft ? -1 : 1);
            shootPoint.localPosition = spLocal;
        }

        // Start burst shooting if in detection range
        if (distance <= detectionRange && !isShootingBurst)
        {
            StartCoroutine(ShootBurst());
        }
    }

    void FixedUpdate()
    {
        if (!player) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= retreatDistance)
        {
            Retreat();
        }
        else if (distance <= detectionRange)
        {
            // Stand still while shooting
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (isStopping)
        {
            stopTimer -= Time.fixedDeltaTime;
            if (stopTimer <= 0f)
                isStopping = false;
            return;
        }

        float dir = Mathf.Sign(targetPoint.position.x - rb.position.x);

        // Check for ground in front before moving
        if (IsGroundAhead(dir))
        {
            rb.linearVelocity = new Vector2(dir * wanderSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // stop at edge
        }

        if (Mathf.Abs(rb.position.x - targetPoint.position.x) < 0.1f)
        {
            targetPoint = targetPoint == pointA ? pointB : pointA;

            if (Random.value < stopChance)
            {
                isStopping = true;
                stopTimer = Random.Range(stopTimeRange.x, stopTimeRange.y);
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
    }

    void Retreat()
    {
        float dir = Mathf.Sign(rb.position.x - player.position.x);

        // Stop if there's no ground ahead
        if (IsGroundAhead(dir))
            rb.linearVelocity = new Vector2(dir * retreatSpeed, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    bool IsGroundAhead(float direction)
    {
        if (!groundCheck) return true; // fallback
        Vector2 origin = groundCheck.position;
        Vector2 dir = Vector2.down;
        float checkDistance = groundCheckDistance;

        RaycastHit2D hit = Physics2D.Raycast(origin + Vector2.right * direction * 0.1f, dir, checkDistance, groundLayer);
        Debug.DrawRay(origin + Vector2.right * direction * 0.1f, dir * checkDistance, Color.red);
        return hit.collider != null;
    }

    IEnumerator ShootBurst()
    {
        isShootingBurst = true;

        for (int i = 0; i < bulletsPerBurst; i++)
        {
            Shoot();
            yield return new WaitForSeconds(timeBetweenShots);
        }

        yield return new WaitForSeconds(timeBetweenBursts);

        isShootingBurst = false;
    }

    void Shoot()
    {
        if (!bulletPrefab || !shootPoint || !player) return;

        GameObject bulletObj = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        Projectile proj = bulletObj.GetComponent<Projectile>();
        if (!proj) return;

        proj.direction = (player.position - shootPoint.position).normalized;
        proj.speed = bulletSpeed;

        Destroy(bulletObj, bulletLifetime);
    }
}
