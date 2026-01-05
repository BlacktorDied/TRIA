using UnityEngine;

public class BasicEnemy : Enemy
{
    #region Variables
    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private Transform[] points;

    private int i = 0;
    private SpriteRenderer spriteRenderer;
    #endregion

    #region Unity Methods

    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (points.Length == 0) Debug.LogWarning("No patrol points assigned for BasicEnemy!");
    }

    protected override void Update()
    {
        base.Update();

        if (points.Length == 0) return;

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
        spriteRenderer.flipX = (points[i].position.x - transform.position.x) < 0;

        if (Vector2.Distance(transform.position, points[i].position) <= 0.25f)
        {
            i++;
            if (i >= points.Length) i = 0;
        }
    }

    #endregion
}
