using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField]
    public int maxHealthPoints = 3;

    [SerializeField]
    public int healthPoints;

    [Header("Death Settings")]
    [SerializeField]
    private float deathDelay = 2f;

    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    private MonoBehaviour[] playerScripts;
    private Vector2 respawnPosition;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerScripts = GetComponents<MonoBehaviour>();

        // Load persistent health if GameManager2 exists
        if (GameManager2.Instance != null)
        {
            maxHealthPoints = GameManager2.Instance.playerMaxHealth;
        }

        healthPoints = maxHealthPoints;

        respawnPosition = transform.position;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        healthPoints -= damage;
        healthPoints = Mathf.Clamp(healthPoints, 0, maxHealthPoints);

        Debug.Log($"Player took {damage} damage! HP: {healthPoints}/{maxHealthPoints}");

        if (healthPoints <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (isDead)
            return;

        healthPoints += amount;
        healthPoints = Mathf.Clamp(healthPoints, 0, maxHealthPoints);

        Debug.Log($"Player healed {amount} HP! HP: {healthPoints}/{maxHealthPoints}");
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealthPoints += amount;

        // Prevent weird values
        maxHealthPoints = Mathf.Max(1, maxHealthPoints);

        // Heal to full when max health increases
        healthPoints = maxHealthPoints;

        if (GameManager2.Instance != null)
        {
            GameManager2.Instance.playerMaxHealth = maxHealthPoints;
        }

        Debug.Log($"Max Health increased! Now: {healthPoints}/{maxHealthPoints}");
    }

    public void SetCheckpoint(Vector2 newPosition)
    {
        respawnPosition = newPosition;
        Debug.Log($"Checkpoint updated: {respawnPosition}");
    }

    private void Die()
    {
        if (isDead)
            return;
        isDead = true;

        Debug.Log("Player has died!");

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (playerScripts != null)
        {
            foreach (MonoBehaviour script in playerScripts)
            {
                if (script != null && script != this)
                    script.enabled = false;
            }
        }

        StartCoroutine(RespawnAtCheckpoint());
    }

    private IEnumerator RespawnAtCheckpoint()
    {
        yield return new WaitForSeconds(deathDelay);

        transform.position = respawnPosition;

        // Restore full health
        healthPoints = maxHealthPoints;
        healthPoints = Mathf.Clamp(healthPoints, 0, maxHealthPoints);

        isDead = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        if (playerScripts != null)
        {
            foreach (MonoBehaviour script in playerScripts)
            {
                if (script != null && script != this)
                    script.enabled = true;
            }
        }

        Debug.Log("Player respawned at checkpoint!");
    }
}
