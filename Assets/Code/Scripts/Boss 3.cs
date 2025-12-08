using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Boss : MonoBehaviour
{
    #region Variables

    [Header("Player Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 12f;

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
    [SerializeField] private IntRange jumpsBeforeSkyAttack;
    [SerializeField] private IntRange bulletCount;

    [Header("Slam Attack")]
    [SerializeField] private float slamHoverHeight = 3f;
    [SerializeField] private float slamHoverTime = 0.25f;
    [SerializeField] private float slamFallSpeed = 18f;
    [SerializeField] private IntRange slamCount = new IntRange { min = 1, max = 3 };
    [SerializeField] private float slamChancePerJump = 0.25f;   // 25% chance after each jump

    private bool isActive = false;
    private int jumpCounter = 0;
    private float minX;
    private float maxX;

    #endregion

    #region Unity Methods

    void Start()
    {
        minX = leftPoint.position.x;
        maxX = rightPoint.position.x;
    }

    void Update()
    {
        if (!player) return;

        if (!isActive && Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            isActive = true;
            StartCoroutine(MainLoop());
        }
    }

    #endregion

    IEnumerator MainLoop()
    {
        while (true)
        {
            jumpCounter = 0;

            while (jumpCounter < jumpsBeforeSkyAttack.RandomValue)
            {
                if (Random.value < slamChancePerJump)
                {
                    yield return SlamAttack(slamCount.RandomValue);
                }
                else
                {
                    Vector3 randomTarget = new Vector3(
                        Random.Range(minX, maxX),
                        GetGroundHeightAt(player.position.x),
                        transform.position.z
                    );

                    yield return JumpTo(randomTarget, jumpDuration.RandomValue, jumpPeak.RandomValue);
                }

                yield return new WaitForSeconds(jumpCooldown);

                jumpCounter++;
            }

            yield return SkyAttack();
        }
    }

    #region Jump Logic

    float GetGroundHeightAt(float xPos)
    {
        var col = GetComponent<Collider2D>();

        RaycastHit2D hit = Physics2D.BoxCast(
            new Vector2(xPos, skyPoint.position.y),
            new Vector2(3f, 0.1f),
            0f,
            Vector2.down,
            50f,
            LayerMask.GetMask("Ground")
        );

        if (hit.collider != null) return hit.point.y + col.bounds.extents.y;

        return transform.position.y;
    }

    IEnumerator JumpTo(Vector3 target, float duration, float peak)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float height = Mathf.Sin(t * Mathf.PI) * peak;
            Vector3 pos = Vector3.Lerp(transform.position, target, t);
            pos.y += height;

            transform.position = pos;
            yield return null;
        }

        transform.position = target;
    }

    #endregion

    #region Slam Attack

    IEnumerator SlamAttack(int repeatCount)
    {
        for (int i = 0; i < repeatCount; i++)
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

            while (transform.position.y > groundPoint.y)
            {
                transform.position += Vector3.down * slamFallSpeed * Time.deltaTime;
                yield return null;
            }

            transform.position = groundPoint;

            yield return new WaitForSeconds(0.2f);
        }
    }

    #endregion

    #region Sky Attack

    IEnumerator SkyAttack()
    {
        yield return JumpTo(skyPoint.position, jumpDuration.RandomValue, jumpPeak.RandomValue);
        ShootDownwardBullets(bulletCount.RandomValue);
        yield return new WaitForSeconds(1f);
    }

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
            if (p != null) p.direction = dir;
        }
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

}
