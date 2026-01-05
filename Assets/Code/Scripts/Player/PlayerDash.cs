using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.15f;
    [SerializeField] private float dashCooldown = 0.6f;

    private Rigidbody2D rb;
    private PlayerInputHandler input;
    
    public bool IsDashing => isDashing;
    private bool isDashing;
    private float lastDashTime;

    private float normalGravity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler>();
        normalGravity = rb.gravityScale;
    }

    private void Update()
    {
        if (isDashing) return;  

        if (input.DashPressed && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        lastDashTime = Time.time;

        float xInput = input.MoveInput.x;

        if (Mathf.Abs(xInput) < 0.01f)
            xInput = transform.localScale.x >= 0 ? 1f : -1f;

        Vector2 dir = new Vector2(xInput, 0f).normalized;

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        float t = 0f;
        while (t < dashTime)
        {
            rb.linearVelocity = dir * dashSpeed;
            t += Time.deltaTime;
            yield return null;
        }

        rb.gravityScale = normalGravity;
        isDashing = false;
    }
}
