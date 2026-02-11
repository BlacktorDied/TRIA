using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Boss3 : Enemy
{
    #region Variables

    [Header("Player Reference")]
    [SerializeField] private Transform player;

    [Header("Arena Lock")]
    [SerializeField] private GameObject arenaDoor;

    [Header("Boundaries")]
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private Transform skyPoint;

    [Header("Jump Settings")]
    [SerializeField] private FloatRange jumpDuration = new FloatRange { min = 0.5f, max = 0.8f };
    [SerializeField] private FloatRange jumpPeak = new FloatRange { min = 2f, max = 3f };
    [SerializeField] private float jumpCooldown = 0.3f;

    [Header("Sky Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private IntRange jumpsBeforeSkyAttack = new IntRange { min = 8, max = 12 };
    [SerializeField] private IntRange bulletCount = new IntRange { min = 8, max = 15 };

    [Header("Slam Attack")]
    [SerializeField] private float slamHoverHeight = 3f;
    [SerializeField] private float slamHoverTime = 0.25f;
    [SerializeField] private float slamFallSpeed = 18f;
    [SerializeField] private IntRange slamCount = new IntRange { min = 1, max = 3 };
    [SerializeField] private float slamChancePerJump = 0.25f;

    [Header("Reset Settings")]
    [SerializeField] private int resetHealth = 10;

    private bool isActive = false;
    private int jumpCounter = 0;
    private float minX;
    private float maxX;

    private Vector3 spawnPosition;
    private Coroutine mainLoopRoutine;

    #endregion

    #region Unity Methods

    protected override void Start()
    {
        base.Start();

        minX = leftPoint.position.x;
        maxX = rightPoint.position.x;

        spawnPosition = transform.position;

        // Ensure collider is trigger (square detection zone)
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    #endregion

    #region Room Trigger Detection (Square Area)

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Player") && !isActive)
        {
            ActivateBoss();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Player") && isActive)
        {
            ResetBoss();
        }
    }

    #endregion

    #region Boss Logic

    private void ActivateBoss()
    {
        isActive = true;

        if (arenaDoor != null)
        {
            arenaDoor.SetActive(true);

            Collider2D col = arenaDoor.GetComponentInChildren<Collider2D>(true);
            if (col != null)
            {
                col.enabled = true;
                col.isTrigger = false;
            }

            SpriteRenderer sr = arenaDoor.GetComponentInChildren<SpriteRenderer>(true);
            if (sr != null) sr.enabled = true;
        }

        mainLoopRoutine = StartCoroutine(MainLoop());
    }

    private void ResetBoss()
    {
        isActive = false;

        if (mainLoopRoutine != null)
            StopCoroutine(mainLoopRoutine);

        StopAllCoroutines();

        transform.position = spawnPosition;

        healthPoints = resetHealth;

        if (arenaDoor != null)
            arenaDoor.SetActive(false);
    }

    IEnumerator MainLoop()
    {
        while (!isDead && isActive)
        {
            jumpCounter = 0;

            while (jumpCounter < jumpsBeforeSkyAttack.RandomValue && !isDead && isActive)
            {
                if (Random.value < slamChancePerJump)
                {
                    yield return SlamAttack(slamCount.RandomValue);
                }
                else
                {
                    Vector3 randomTarget = new Vector3(
                        Random.Range(minX, maxX),
                        leftPoint.position.y,
                        transform.position.z
                    );

                    yield return JumpTo(
                        randomTarget,
                        jumpDuration.RandomValue,
                        jumpPeak.RandomValue
                    );
                }

                yield return new WaitForSeconds(jumpCooldown);
                jumpCounter++;
            }

            if (!isDead && isActive)
            {
                yield return SkyAttack();
                yield return new WaitForSeconds(1f);
            }
        }
    }

    IEnumerator JumpTo(Vector3 target, float duration, float peak)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < duration && !isDead && isActive)
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

    IEnumerator SlamAttack(int repeatCount)
    {
        for (int i = 0; i < repeatCount && !isDead && isActive; i++)
        {
            Vector3 hoverPoint = new Vector3(
                player.position.x,
                leftPoint.position.y + slamHoverHeight,
                transform.position.z
            );

            yield return JumpTo(hoverPoint, 0.3f, 1.5f);
            yield return new WaitForSeconds(slamHoverTime);

            Vector3 groundPoint = new Vector3(
                hoverPoint.x,
                leftPoint.position.y,
                transform.position.z
            );

            while (transform.position.y > groundPoint.y && !isDead && isActive)
            {
                transform.position += Vector3.down * slamFallSpeed * Time.deltaTime;
                yield return null;
            }

            transform.position = groundPoint;
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator SkyAttack()
    {
        yield return JumpTo(
            skyPoint.position,
            jumpDuration.RandomValue,
            jumpPeak.RandomValue
        );

        for (int i = 0; i < bulletCount.RandomValue && !isDead && isActive; i++)
        {
            GameObject proj = Instantiate(
                projectilePrefab,
                transform.position,
                Quaternion.identity
            );

            Vector2 dir = new Vector2(
                Random.Range(-1f, 1f),
                Random.Range(-1f, -0.3f)
            ).normalized;

            Projectile p = proj.GetComponent<Projectile>();
            if (p != null)
                p.direction = dir;
        }
    }

    #endregion

    #region Death

    protected override void Die()
    {
        if (isDead) return;

        isDead = true;

        if (arenaDoor != null)
            arenaDoor.SetActive(false);

        StopAllCoroutines();
        Destroy(gameObject);
    }

    public void ActivateFromRoom()
    {
        ActivateBoss();
    }

    public void ResetFromRoom()
    {
        ResetBoss();
    }

    #endregion
}