using System.Collections;
using System.Collections.Generic;
using TRIA.Core;
using TRIA.Core.Constants;
using UnityEngine;

namespace TRIA.Enemies
{
    public class Boss1 : Enemy
    {
        [Header("Boss State")]
        [SerializeField] private bool isActive = false;

        [Header("Arena Doors")]
        [SerializeField] private List<GameObject> arenaDoors = new List<GameObject>();

        [Header("Jump Targets")]
        [SerializeField] private Transform leftPoint;
        [SerializeField] private Transform rightPoint;

        [Header("Vertical Jump Phase")]
        [SerializeField] private float verticalJumpHeight = 3f;
        [SerializeField] private float verticalJumpDuration = 0.25f;

        [Header("Arc Jump Phase")]
        [SerializeField] private float arcHeight = 1.5f;
        [SerializeField] private float arcDuration = 0.5f;

        [Header("Jump Timing")]
        [SerializeField] private float peakPause = 0.1f;
        [SerializeField] private float jumpCooldown = 1.5f;

        [Header("Spawning")]
        [SerializeField] private GameObject flyingEnemyPrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private int enemiesPerSpawn = 2;
        [SerializeField] private float spawnCooldown = 5f;

        private Vector3 _startPosition;
        private List<GameObject> _spawnedEnemies = new List<GameObject>();
        private bool _jumpToLeft = true;
        private Transform _playerTransform;

        protected override void Start()
        {
            base.Start();

            _startPosition = transform.position;

            SetDoors(false);
        }

        // Called by BossRoomTrigger
        public void ActivateFromRoom()
        {
            ActivateBoss();
        }

        public void ResetFromRoom()
        {
            ResetBoss();
        }

        public void ActivateBoss()
        {
            if (isActive || isDead)
                return;

            isActive = true;

            SetDoors(true);

            if (PlayerManager.Instance != null && PlayerManager.Instance.CurrentPlayer != null)
                _playerTransform = PlayerManager.Instance.CurrentPlayer.transform;

            StartCoroutine(BossLoop());
            StartCoroutine(SpawnLoop());

            Debug.Log("<color=red><b>Boss 1 Activated!</b></color>");
        }

        public void ResetBoss()
        {
            isActive = false;

            StopAllCoroutines();

            SetDoors(false);

            transform.position = _startPosition;

            ClearSpawnedEnemies();
        }

        private IEnumerator BossLoop()
        {
            while (isActive && !isDead)
            {
                yield return new WaitForSeconds(jumpCooldown);

                Transform target = _jumpToLeft ? leftPoint : rightPoint;

                yield return StartCoroutine(JumpRoutine(target.position));

                _jumpToLeft = !_jumpToLeft;
            }
        }

        private IEnumerator JumpRoutine(Vector3 target)
        {
            Vector3 start = transform.position;
            float timer = 0f;

            // PHASE 1 — straight up
            while (timer < verticalJumpDuration)
            {
                timer += Time.deltaTime;
                float t = timer / verticalJumpDuration;

                float y = Mathf.Lerp(start.y, start.y + verticalJumpHeight, t);

                transform.position = new Vector3(start.x, y, transform.position.z);

                yield return null;
            }

            Vector3 peak = transform.position;

            if (peakPause > 0)
                yield return new WaitForSeconds(peakPause);

            timer = 0f;

            // PHASE 2 — arc toward target
            while (timer < arcDuration)
            {
                timer += Time.deltaTime;
                float t = timer / arcDuration;

                float x = Mathf.Lerp(peak.x, target.x, t);

                float y = Mathf.Lerp(peak.y, target.y, t)
                          + Mathf.Sin(t * Mathf.PI) * arcHeight;

                transform.position = new Vector3(x, y, transform.position.z);

                yield return null;
            }

            transform.position = target;
        }

        private IEnumerator SpawnLoop()
        {
            while (isActive && !isDead)
            {
                yield return new WaitForSeconds(spawnCooldown);

                SpawnEnemies();
            }
        }

        private void SpawnEnemies()
        {
            if (flyingEnemyPrefab == null || spawnPoint == null)
                return;

            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                GameObject enemy = Instantiate(
                    flyingEnemyPrefab,
                    spawnPoint.position,
                    Quaternion.identity
                );

                _spawnedEnemies.Add(enemy);

                // Optional player targeting
                // enemy.GetComponent<BasicFlyingEnemy>()?.SetPlayer(_playerTransform);
            }
        }

        private void ClearSpawnedEnemies()
        {
            foreach (var enemy in _spawnedEnemies)
            {
                if (enemy != null)
                    Destroy(enemy);
            }

            _spawnedEnemies.Clear();
        }

        private void SetDoors(bool state)
        {
            foreach (GameObject door in arenaDoors)
            {
                if (door != null)
                    door.SetActive(state);
            }
        }

        protected override void Die()
        {
            isActive = false;

            StopAllCoroutines();

            ClearSpawnedEnemies();

            SetDoors(false);

            base.Die();

            Debug.Log("<color=black>Boss 1 Defeated!</color>");
        }
    }
}
