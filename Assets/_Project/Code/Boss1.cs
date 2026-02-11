using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : Enemy
{
    [Header("Boss Room Trigger")]
    [SerializeField] private Collider2D roomTrigger;

    [Header("Jump Points")]
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;

    [Header("Jump Settings")]
    [SerializeField] private float jumpDuration = 0.8f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float jumpCooldown = 1f;

    [Header("Enemy Spawning")]
    [SerializeField] private GameObject flyingEnemyPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int enemiesPerSpawn = 2;
    [SerializeField] private float spawnCooldown = 5f;

    private bool isActive = false;
    private bool jumpLeft = false;

    private Vector3 spawnPosition;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    protected override void Start()
    {
        base.Start();

        spawnPosition = transform.position;

        if (roomTrigger != null)
            roomTrigger.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!isActive)
        {
            Debug.Log("Boss activated");
            ActivateBoss();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (isActive)
        {
            Debug.Log("Boss reset");
            ResetBoss();
        }
    }

    public void ActivateBoss()
    {
        if (isActive || isDead) return;

        isActive = true;

        StartCoroutine(JumpLoop());
        StartCoroutine(SpawnLoop());
    }

    public void ResetBoss()
    {
        isActive = false;

        StopAllCoroutines();

        transform.position = spawnPosition;

        healthPoints = maxHealthPoints;

        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }

        spawnedEnemies.Clear();
    }

    IEnumerator JumpLoop()
    {
        while (isActive && !isDead)
        {
            Transform target = jumpLeft ? leftPoint : rightPoint;

            yield return JumpTo(target.position);

            jumpLeft = !jumpLeft;

            yield return new WaitForSeconds(jumpCooldown);
        }
    }

    IEnumerator JumpTo(Vector3 target)
    {
        Vector3 start = transform.position;

        float elapsed = 0f;

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / jumpDuration;

            float height = Mathf.Sin(t * Mathf.PI) * jumpHeight;

            Vector3 pos = Vector3.Lerp(start, target, t);
            pos.y += height;

            transform.position = pos;

            yield return null;
        }

        transform.position = target;
    }

    IEnumerator SpawnLoop()
    {
        while (isActive && !isDead)
        {
            yield return new WaitForSeconds(spawnCooldown);

            SpawnEnemies();
        }
    }

    void SpawnEnemies()
    {
        if (flyingEnemyPrefab == null || spawnPoint == null)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        for (int i = 0; i < enemiesPerSpawn; i++)
        {
            Vector3 spawnPos = spawnPoint.position;

            GameObject enemy = Instantiate(
                flyingEnemyPrefab,
                spawnPos,
                Quaternion.identity
            );

            // Give enemy the player reference
            BasicFlyingEnemy flying = enemy.GetComponent<BasicFlyingEnemy>();
            if (flying != null && player != null)
            {
                flying.SetPlayer(player.transform);
            }

            spawnedEnemies.Add(enemy);

            Debug.Log("Enemy spawned and linked to player");
        }
    }
    protected override void Die()
    {
        if (isDead) return;

        isDead = true;

        StopAllCoroutines();

        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }

        spawnedEnemies.Clear();

        base.Die();
    }
}