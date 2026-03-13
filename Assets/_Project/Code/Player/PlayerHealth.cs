using System.Collections;
using TRIA.Core;
using UnityEngine;

namespace TRIA.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [Header("Health Stats")]
        [SerializeField]
        private float maxHealthPoints = 3f;

        [SerializeField]
        private float currentHealth;

        [Header("Death Settings")]
        [SerializeField]
        private float deathDelay = 1.5f;

        public bool IsDead { get; private set; }

        private SpriteRenderer _sprite;
        private Rigidbody2D _rb;
        private PlayerController _controller;
        private Vector2 _respawnPosition;

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
            _controller = GetComponent<PlayerController>();

            if (GameManager.Instance != null)
                maxHealthPoints = GameManager.Instance.playerMaxHealth;

            currentHealth = maxHealthPoints;
            _respawnPosition = transform.position;
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

        // --- ADD THIS METHOD TO FIX THE ERROR ---
        public void IncreaseMaxHealth(float amount)
        {
            maxHealthPoints += amount;

            // Optionally heal the player by the same amount so the new heart isn't empty
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
            yield return new WaitForSecondsRealtime(deathDelay);
            transform.position = _respawnPosition;
            currentHealth = maxHealthPoints;
            IsDead = false;
            GameManager.TriggerFadeIn();
        }

        public void SetCheckpoint(Vector2 position) => _respawnPosition = position;
    }
}
