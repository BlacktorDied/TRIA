using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class MotherEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float fleeDistance = 5f;             // Distance to start fleeing
    public Vector2 randomOffsetRange = new Vector2(1f, 3f); // Random jitter amplitude
    public float directionChangeInterval = 1.2f; // How often to change random movement

    [Header("Minion Spawning")]
    public GameObject minionPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 2f;
    public float activationDistance = 8f; // Only spawn if player is this close

    [Header("Detection")]
    public Transform player;

    private Rigidbody2D rb;
    private Vector2 randomOffset;
    private float directionTimer;
    private bool isSpawning = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.gravityScale = 0;

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        randomOffset = Vector2.zero;
        directionTimer = directionChangeInterval;
    }

    void FixedUpdate()
    {
        if (!player) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Start or stop spawning based on activation distance
        if (distance <= activationDistance && !isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnMinions());
        }
        else if (distance > activationDistance && isSpawning)
        {
            isSpawning = false;
        }

        Vector2 direction = Vector2.zero;

        // Flee if player is too close
        if (distance < fleeDistance)
        {
            direction = (Vector2)transform.position - (Vector2)player.position;
        }
        else
        {
            // Slight random hovering if player far
            direction = Random.insideUnitCircle;
        }

        // Add unpredictable jitter
        directionTimer -= Time.fixedDeltaTime;
        if (directionTimer <= 0f)
        {
            randomOffset = new Vector2(
                Random.Range(-randomOffsetRange.x, randomOffsetRange.x),
                Random.Range(-randomOffsetRange.y, randomOffsetRange.y)
            );
            directionTimer = directionChangeInterval;
        }

        direction += randomOffset;

        // Move
        direction.Normalize();
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        // Optional: rotate toward movement
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    IEnumerator SpawnMinions()
    {
        while (isSpawning)
        {
            if (minionPrefab && spawnPoint)
            {
                GameObject minion = Instantiate(minionPrefab, spawnPoint.position, Quaternion.identity);

                // Ensure the minion's movement script is active
                var movement = minion.GetComponent<MonoBehaviour>();
                if (movement)
                    movement.enabled = true;
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
