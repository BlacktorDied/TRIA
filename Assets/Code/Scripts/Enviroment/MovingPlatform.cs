using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    public float speed = 2f;            // Movement speed
    public int startingPoint = 0;       // Index of the starting point
    public Transform[] points;          // Waypoints for the platform

    private int i;                       // Current target point index

    void Start()
    {
        if (points.Length == 0) return;

        // Set platform at the starting point
        transform.position = points[startingPoint].position;
        i = startingPoint;
    }

    void Update()
    {
        if (points.Length == 0) return;

        // Move to the next point if close enough
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++;
            if (i >= points.Length)
                i = 0; // Loop back to first point
        }

        // Move platform
        transform.position = Vector2.MoveTowards(
            transform.position,
            points[i].position,
            speed * Time.deltaTime
        );
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Parent player to platform
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Start unparenting from the PLAYER object to avoid inactive platform error
        if (collision.collider.CompareTag("Player"))
        {
            var playerMono = collision.collider.GetComponent<MonoBehaviour>();
            if (playerMono != null)
                playerMono.StartCoroutine(UnparentNextFrame(collision.collider.transform));
        }
    }

    System.Collections.IEnumerator UnparentNextFrame(Transform player)
    {
        yield return null; // wait one frame
        if (player != null)
            player.SetParent(null);
    }
}
