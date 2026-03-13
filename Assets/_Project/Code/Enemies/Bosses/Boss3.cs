using System.Collections;
using TRIA.Enemies;
using TRIA.Utilities;
using UnityEngine;

namespace TRIA.Enemies
{
    [RequireComponent(typeof(Collider2D))]
    public class Boss3 : Enemy
    {
        #region Variables

        [Header("Arena Lock")]
        [SerializeField]
        private GameObject arenaDoor;

        [Header("Boundaries")]
        [SerializeField]
        private Transform leftPoint;

        [SerializeField]
        private Transform rightPoint;

        [SerializeField]
        private Transform skyPoint;

        [Header("Jump Settings")]
        [SerializeField]
        private FloatRange jumpDuration = new FloatRange { min = 0.5f, max = 0.8f };

        [SerializeField]
        private FloatRange jumpPeak = new FloatRange { min = 2f, max = 3f };

        [SerializeField]
        private float jumpCooldown = 0.3f;

        [Header("Sky Attack")]
        [SerializeField]
        private GameObject projectilePrefab;

        [SerializeField]
        private IntRange bulletCount = new IntRange { min = 8, max = 15 };

        [Header("Slam Attack")]
        [SerializeField]
        private float slamFallSpeed = 18f;

        private bool isActive = false;

        #endregion

        protected override void Start()
        {
            base.Start(); // This initializes 'health' to 'maxHealth'
        }

        public void ActivateBoss()
        {
            if (isActive || isDead)
                return;

            isActive = true;
            if (arenaDoor != null)
                arenaDoor.SetActive(true);

            StartCoroutine(BossLoop());
        }

        public void ResetBoss()
        {
            isActive = false;
            StopAllCoroutines();

            if (arenaDoor != null)
                arenaDoor.SetActive(false);

            // FIX: Changed healthPoints to health
            // and maxHealthPoints to maxHealth
            health = maxHealth;

            transform.position = leftPoint.position; // Move back to start
        }

        private IEnumerator BossLoop()
        {
            while (isActive && !isDead)
            {
                // Pattern: Jump Left -> Sky Attack -> Jump Right -> Slam
                yield return StartCoroutine(
                    JumpTo(leftPoint.position, jumpDuration.RandomValue, jumpPeak.RandomValue)
                );
                yield return new WaitForSeconds(jumpCooldown);

                yield return StartCoroutine(SkyAttack());
                yield return new WaitForSeconds(jumpCooldown);

                yield return StartCoroutine(
                    JumpTo(rightPoint.position, jumpDuration.RandomValue, jumpPeak.RandomValue)
                );
                yield return new WaitForSeconds(jumpCooldown);
            }
        }

        private IEnumerator JumpTo(Vector3 target, float duration, float height)
        {
            Vector3 start = transform.position;
            float elapsed = 0;

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

        private IEnumerator SkyAttack()
        {
            // Simple logic: fire projectiles downwards
            for (int i = 0; i < bulletCount.RandomValue; i++)
            {
                if (isDead)
                    break;

                GameObject proj = Instantiate(
                    projectilePrefab,
                    transform.position,
                    Quaternion.identity
                );
                // proj.GetComponent<Projectile>()?.SetDirection(Vector2.down);

                yield return new WaitForSeconds(0.1f);
            }
        }

        protected override void Die()
        {
            isActive = false;
            if (arenaDoor != null)
                arenaDoor.SetActive(false);

            base.Die(); // Handles destruction and points
        }

        // Methods for the BossTrigger to call
        public void ActivateFromRoom() => ActivateBoss();

        public void ResetFromRoom() => ResetBoss();
    }
}
