using System.Collections;
using UnityEngine;

namespace TRIA.Enemies
{
    [RequireComponent(typeof(Collider2D))]
    public class Boss3 : Enemy
    {
        [Header("Arena")]
        [SerializeField] private GameObject arenaDoor;
        [SerializeField] private Transform leftPoint;
        [SerializeField] private Transform rightPoint;
        [SerializeField] private Transform skyPoint;

        [Header("Jump Settings")]
        [SerializeField] private float jumpDuration = 0.8f;
        [SerializeField] private float jumpHeight = 3f;
        [SerializeField] private float jumpCooldown = 0.3f;
        [SerializeField] private float minJumpDistance = 1.5f;

        [Header("Sky Attack")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int minSkyShots = 5;
        [SerializeField] private int maxSkyShots = 12;
        [SerializeField] private float skyShotSpreadAngle = 60f;
        [SerializeField] private float projectileSpeed = 5f;

        [Header("Slam Attack")]
        [SerializeField] private float slamFallSpeed = 18f;
        [SerializeField] private int minSlams = 2;
        [SerializeField] private int maxSlams = 4;
        [SerializeField] private float slamHoverHeight = 3f;
        [SerializeField] private float slamHoverSpeed = 5f;

        [Header("Attack Chances")]
        [SerializeField, Range(0f, 1f)] private float slamChance = 0.3f;
        [SerializeField, Range(0f, 1f)] private float skyAttackChance = 0.2f;

        private bool isActive = false;
        private Vector3 spawnPosition;

        protected override void Start()
        {
            base.Start();
            spawnPosition = transform.position;

            if (arenaDoor != null)
                arenaDoor.SetActive(false);
        }

        public void ActivateFromRoom() => ActivateBoss();
        public void ResetFromRoom() => ResetBoss();

        public void ActivateBoss()
        {
            if (isActive || isDead)
                return;

            isActive = true;

            if (arenaDoor != null)
                arenaDoor.SetActive(true);

            StartCoroutine(BossRoutine());
        }

        public void ResetBoss()
        {
            isActive = false;

            StopAllCoroutines();

            if (arenaDoor != null)
                arenaDoor.SetActive(false);

            health = maxHealth;
            transform.position = spawnPosition;
        }

        private IEnumerator BossRoutine()
        {
            yield return new WaitForSeconds(2f);

            while (isActive && !isDead)
            {
                // --- Random Ground Jump (anywhere between points) ---
                Vector3 target;

                do
                {
                    float randomX = Random.Range(leftPoint.position.x, rightPoint.position.x);
                    target = new Vector3(randomX, leftPoint.position.y, transform.position.z);
                }
                while (Vector3.Distance(transform.position, target) < minJumpDistance);

                yield return StartCoroutine(JumpTo(target, jumpDuration, jumpHeight));
                yield return new WaitForSeconds(jumpCooldown);

                // --- Random Slam Attack ---
                if (Random.value < slamChance)
                {
                    int slamCount = Random.Range(minSlams, maxSlams + 1);

                    for (int i = 0; i < slamCount; i++)
                    {
                        yield return StartCoroutine(SlamAbovePlayer());
                        yield return new WaitForSeconds(0.5f);
                    }
                }

                // --- Random Sky Attack ---
                if (Random.value < skyAttackChance)
                {
                    yield return StartCoroutine(JumpTo(skyPoint.position, jumpDuration, jumpHeight));
                    yield return new WaitForSeconds(0.2f);
                    yield return StartCoroutine(SkyProjectileAttack());
                }
            }
        }

        private IEnumerator JumpTo(Vector3 target, float duration, float height)
        {
            Vector3 start = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                Vector3 pos = Vector3.Lerp(start, target, t);
                pos.y += Mathf.Sin(t * Mathf.PI) * height;

                transform.position = pos;

                yield return null;
            }

            transform.position = target;
        }

        private IEnumerator SlamAbovePlayer()
        {
            if (playerTransform == null)
                yield break;

            Vector3 startPos = transform.position;

            Vector3 hoverPos = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y + slamHoverHeight,
                transform.position.z
            );

            float distance = Vector3.Distance(startPos, hoverPos);
            float elapsed = 0f;

            while (elapsed < distance / slamHoverSpeed)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, hoverPos, elapsed * slamHoverSpeed / distance);
                yield return null;
            }

            transform.position = hoverPos;

            Vector3 groundPos = new Vector3(
                hoverPos.x,
                leftPoint.position.y,
                transform.position.z
            );

            float slamDistance = Vector3.Distance(hoverPos, groundPos);
            elapsed = 0f;

            while (elapsed < slamDistance / slamFallSpeed)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(hoverPos, groundPos, elapsed * slamFallSpeed / slamDistance);
                yield return null;
            }

            transform.position = groundPos;
        }

        private IEnumerator SkyProjectileAttack()
        {
            int shots = Random.Range(minSkyShots, maxSkyShots + 1);

            for (int i = 0; i < shots; i++)
            {
                if (isDead)
                    break;

                float angle = Random.Range(-skyShotSpreadAngle / 2f, skyShotSpreadAngle / 2f);
                Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.down;

                GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

                if (proj.TryGetComponent(out Rigidbody2D rb))
                {
                    rb.linearVelocity = dir * projectileSpeed;
                }
            }

            yield return null;
        }

        protected override void Die()
        {
            isActive = false;
            StopAllCoroutines();

            if (arenaDoor != null)
                arenaDoor.SetActive(false);

            base.Die();
        }
    }
}