using System.Collections;
using System.Collections.Generic;
using TRIA.Core;
using TRIA.Core.Constants;
using UnityEngine;

namespace TRIA.Enemies
{
    public class Boss1 : Enemy // Assumes you have a base Enemy class
    {
        [Header("Boss State")]
        [SerializeField]
        private bool isActive = false;

        [Header("Jump Settings")]
        [SerializeField]
        private Transform leftPoint;

        [SerializeField]
        private Transform rightPoint;

        [SerializeField]
        private float jumpDuration = 0.8f;

        [SerializeField]
        private float jumpHeight = 3f;

        [SerializeField]
        private float jumpCooldown = 1.5f;

        [Header("Spawning")]
        [SerializeField]
        private GameObject flyingEnemyPrefab;

        [SerializeField]
        private Transform spawnPoint;

        [SerializeField]
        private int enemiesPerSpawn = 2;

        [SerializeField]
        private float spawnCooldown = 5f;

        private Vector3 _startPosition;
        private List<GameObject> _spawnedEnemies = new List<GameObject>();
        private bool _jumpToLeft = true;
        private Transform _playerTransform;

        protected override void Start()
        {
            base.Start();
            _startPosition = transform.position;
        }

        public void ActivateBoss()
        {
            if (isActive || isDead)
                return;

            isActive = true;
            // Find player via PlayerManager for safety
            if (PlayerManager.Instance != null && PlayerManager.Instance.CurrentPlayer != null)
            {
                _playerTransform = PlayerManager.Instance.CurrentPlayer.transform;
            }

            StartCoroutine(BossLoop());
            StartCoroutine(SpawnLoop());
            Debug.Log("<color=red><b>Boss 1 Activated!</b></color>");
        }

        public void ResetBoss()
        {
            isActive = false;
            StopAllCoroutines();
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
            float timer = 0;

            while (timer < jumpDuration)
            {
                timer += Time.deltaTime;
                float t = timer / jumpDuration;

                // Parabolic Jump Calculation
                Vector3 currentPos = Vector3.Lerp(start, target, t);
                currentPos.y += Mathf.Sin(t * Mathf.PI) * jumpHeight;

                transform.position = currentPos;
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

                // If the flying enemy needs a player target
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

        protected override void Die()
        {
            isActive = false;
            StopAllCoroutines();
            ClearSpawnedEnemies();
            base.Die();
            Debug.Log("<color=black>Boss 1 Defeated!</color>");
        }
    }
}
