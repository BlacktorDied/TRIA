using UnityEngine;

public class JumpingEnemy : Enemy
{
    #region Variables

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float horizontalPower = 1.2f;
    [SerializeField] private float jumpCooldown = 0.5f;
    public Transform[] points;

    private int i = 0;
    private float cooldownTimer;
    private SpriteRenderer spriteRenderer;

    #endregion

    #region Unity Methods

    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();

        if (points.Length == 0) return;

        cooldownTimer -= Time.deltaTime;
        Vector2 target = points[i].position;

        // Flip sprite
        spriteRenderer.flipX = (target.x - transform.position.x) < 0;

        bool grounded = Mathf.Abs(rb.linearVelocity.y) < 0.05f;

        // Jump ONLY when grounded and cooldown ready
        if (grounded && cooldownTimer <= 0f)
        {
            JumpToward(target);
            cooldownTimer = jumpCooldown;
        }

        // Switch point ONLY when VERY close
        if (grounded && Mathf.Abs(transform.position.x - target.x) < 0.25f)
        {
            i++;
            if (i >= points.Length) i = 0;
        }
    }

    #endregion

    #region Jump Logic  

    void JumpToward(Vector2 target)
    {
        float distX = target.x - transform.position.x;

        // Reduce force when close so it doesn't overshoot
        float forceMultiplier = Mathf.Clamp(Mathf.Abs(distX), 0.2f, 1f);

        float vx = Mathf.Clamp(distX * horizontalPower * forceMultiplier, -3f, 3f);

        rb.linearVelocity = new Vector2(vx, jumpHeight);
    }

    #endregion
}
