using System.Collections;
using TRIA.Core;
using UnityEngine;

namespace TRIA.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [Header("Health Stats")]
        [SerializeField] private float maxHealthPoints = 3f;
        [SerializeField] private float currentHealth;

        [Header("Death Settings")]
        [SerializeField] private float deathDelay = 1.5f;

        public bool IsDead { get; private set; }

        private void Awake()
        {
            if (GameManager.Instance != null)
                maxHealthPoints = GameManager.Instance.playerMaxHealth;

            currentHealth = maxHealthPoints;
        }

        public void TakeDamage(float damage)
        {
            if (IsDead)
                return;

            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealthPoints);

            Debug.Log($"Player Health: {currentHealth}/{maxHealthPoints}");

            if (currentHealth <= 0)
                Die();
        }

        public void Heal(float amount)
        {
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealthPoints);
        }

        public void IncreaseMaxHealth(float amount)
        {
            maxHealthPoints += amount;
            currentHealth += amount;
            Debug.Log($"Max Health Increased! New Max: {maxHealthPoints}");
        }

        private void Die()
        {
            if (IsDead)
                return;

            IsDead = true;

            GameManager.TriggerFadeOut();
            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            // Wait a bit for death animation / fade
            yield return new WaitForSecondsRealtime(deathDelay);

            // Use GameManager to respawn player at last checkpoint
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RespawnPlayer(gameObject);
            }

            currentHealth = maxHealthPoints;
            IsDead = false;

            GameManager.TriggerFadeIn();
        }

        // Allow checkpoint to update respawn position
        public void SetCheckpoint(Vector2 position)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetCheckpoint(position);
        }
    }
}
