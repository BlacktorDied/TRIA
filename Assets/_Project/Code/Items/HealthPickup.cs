using TRIA.Core.Constants;
using TRIA.Player;
using UnityEngine;

namespace TRIA.Items
{
    public class HealthPickup : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private int healthAmount = 1;

        [SerializeField]
        private float lifetime = 10f;

        private void Start()
        {
            // Pickups shouldn't last forever to save memory
            Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Tags.Player))
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.Heal(healthAmount);

                    // TODO: Trigger a small sparkle effect or "ding" sound here

                    Destroy(gameObject);
                }
            }
        }
    }
}
