using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("References")]
    public GameObject projectilePrefab;
    public Transform player;

    [Header("Activation")]
    public float detectionRange = 6f;
    private bool isActive = false;

    [Header("Boundaries (Empty Objects)")]
    public Transform leftPoint;
    public Transform rightPoint;
    public Transform skyPoint;

    [Header("Jump Settings")]
    public float jumpDurationMin = 0.5f;
    public float jumpDurationMax = 0.8f;
    public float jumpPeakMin = 2f;
    public float jumpPeakMax = 3f;
    public float groundWaitTime = 0.3f;

    [Header("Cycle")]
    public int jumpsBeforeSkyAttackMin = 8;
    public int jumpsBeforeSkyAttackMax = 12;

    [Header("Sky Attack")]
    public int bulletsMin = 8;
    public int bulletsMax = 15;

    [Header("SLAM Attack")]
    public float slamHoverHeight = 2f;
    public float slamHoverTime = 0.25f;
    public float slamFallSpeed = 18f;
    public int slamMin = 1;
    public int slamMax = 3;
    public float slamChancePerJump = 0.25f;   // 25% chance after each jump

    private int jumpCounter = 0;
    private int jumpsThisCycle;

    void Update()
    {
        if (!isActive)
        {
            if (Vector2.Distance(transform.position, player.position) <= detectionRange)
            {
                isActive = true;
                StartCoroutine(MainLoop());
            }
        }
    }

    IEnumerator MainLoop()
    {
        while (true)
        {
            jumpCounter = 0;
            jumpsThisCycle = Random.Range(jumpsBeforeSkyAttackMin, jumpsBeforeSkyAttackMax + 1);

            float minX = Mathf.Min(leftPoint.position.x, rightPoint.position.x);
            float maxX = Mathf.Max(leftPoint.position.x, rightPoint.position.x);
            float groundY = leftPoint.position.y;

            // --- Ground Phase ---
            while (jumpCounter < jumpsThisCycle)
            {
                // Possibly trigger SLAM instead of a normal jump
                if (Random.value < slamChancePerJump)
                {
                    int slamRepeats = Random.Range(slamMin, slamMax + 1);
                    yield return StartCoroutine(SlamAttack(slamRepeats));
                    jumpCounter++;
                    continue;
                }

                // Normal random jump
                Vector3 randomTarget = new Vector3(
                    Random.Range(minX, maxX),
                    groundY,
                    transform.position.z
                );

                float duration = Random.Range(jumpDurationMin, jumpDurationMax);
                float peak = Random.Range(jumpPeakMin, jumpPeakMax);

                yield return StartCoroutine(JumpTo(randomTarget, duration, peak));
                yield return new WaitForSeconds(groundWaitTime);

                jumpCounter++;
            }

            // ---- SKY ATTACK ----
            float skyDuration = Random.Range(jumpDurationMin, jumpDurationMax);
            float skyPeak = Random.Range(jumpPeakMin, jumpPeakMax);
            yield return StartCoroutine(JumpTo(skyPoint.position, skyDuration, skyPeak));

            int bulletsThisCycle = Random.Range(bulletsMin, bulletsMax + 1);
            ShootDownwardBullets(bulletsThisCycle);

            yield return new WaitForSeconds(1f);
        }
    }

    // ------------------------------------------------------------
    // NORMAL JUMP
    // ------------------------------------------------------------
    IEnumerator JumpTo(Vector3 target, float duration, float peak)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float height = Mathf.Sin(t * Mathf.PI) * peak;
            Vector3 pos = Vector3.Lerp(start, target, t);
            pos.y += height;

            transform.position = pos;
            yield return null;
        }

        transform.position = target;
    }

    // ------------------------------------------------------------
    // SLAM ATTACK
    // ------------------------------------------------------------
    IEnumerator SlamAttack(int repeatCount)
    {
        for (int i = 0; i < repeatCount; i++)
        {
            // Go above player
            Vector3 hoverPoint = new Vector3(
                player.position.x,
                leftPoint.position.y + slamHoverHeight,
                transform.position.z
            );

            // Move up fast
            yield return StartCoroutine(JumpTo(hoverPoint, 0.3f, 1.5f));

            // Hover and telegraph
            yield return new WaitForSeconds(slamHoverTime);

            // Slam STRAIGHT DOWN
            Vector3 groundPoint = new Vector3(
                hoverPoint.x,
                leftPoint.position.y,
                transform.position.z
            );

            while (transform.position.y > groundPoint.y)
            {
                transform.position += Vector3.down * slamFallSpeed * Time.deltaTime;
                yield return null;
            }

            transform.position = groundPoint;

            // Small recovery
            yield return new WaitForSeconds(0.2f);
        }
    }

    // ------------------------------------------------------------
    // SKY ATTACK BULLETS
    // ------------------------------------------------------------
    void ShootDownwardBullets(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            Vector2 dir = new Vector2(
                Random.Range(-1f, 1f),
                Random.Range(-1f, -0.3f)
            ).normalized;

            Projectile p = proj.GetComponent<Projectile>();
            if (p != null)
                p.direction = dir;
        }
    }
}
