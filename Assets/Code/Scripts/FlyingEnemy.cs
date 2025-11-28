using UnityEngine;

public class FollowEnemy : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public float detectionRange = 8f;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float idleDriftAmount = 0.2f;

    private Vector3 velocity;

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            FollowPlayer();
        }
        else
        {
            IdleHover();
        }

        transform.position += velocity * Time.deltaTime;
    }

    void FollowPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        velocity = dir * moveSpeed;
    }

    void IdleHover()
    {
        float x = (Mathf.PerlinNoise(Time.time * 0.5f, 0f) - 0.5f) * idleDriftAmount;
        float y = (Mathf.PerlinNoise(0f, Time.time * 0.5f) - 0.5f) * idleDriftAmount;

        velocity = new Vector3(x, y, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        velocity *= 0.2f;
    }
}
