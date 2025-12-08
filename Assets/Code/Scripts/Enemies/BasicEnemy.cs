using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    #region Variables

    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private Transform[] points; // Array of patrol points

    private int i = 0; // Current target point index
    private SpriteRenderer spriteRenderer;

    #endregion

    #region Unity Methods

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (points.Length == 0)
        {
            Debug.LogWarning("No patrol points assigned for BasicEnemy!");
        }
    }

    void Update()
    {
        if (points.Length == 0) return; // Safety check

        // Move towards the current target point
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);

        // Flip sprite depending on movement direction
        spriteRenderer.flipX = (points[i].position.x - transform.position.x) < 0;

        // Check if we reached the current point
        if (Vector2.Distance(transform.position, points[i].position) <= 0.25f)
        {
            i++; // Move to next point
            if (i >= points.Length) i = 0; // Loop back to first point
        }
    }

    #endregion
}
