using TRIA.Core;
using TRIA.Core.Constants;
using UnityEngine;

namespace TRIA.Items
{
    public class HealthCollectible : MonoBehaviour
    {
        [Header("Identity")]
        [Tooltip("Unique ID for this specific pickup (e.g., 'Basement_HP_1')")]
        public string collectibleID;

        [Header("Settings")]
        public int fragmentsToUpgrade = 4;
        public int maxHealthIncrease = 1;

        private void Start()
        {
            if (GameManager.Instance == null)
                return;

            if (GameManager.Instance.collectedCollectibles.Contains(collectibleID))
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Tags.Player))
                return;

            var gm = GameManager.Instance;

            gm.healthFragments++;
            Debug.Log(
                $"<color=red>Health Fragment Collected!</color> ({gm.healthFragments}/{fragmentsToUpgrade})"
            );

            if (gm.healthFragments >= fragmentsToUpgrade)
            {
                UpgradePlayerHealth(other.gameObject);
                gm.healthFragments = 0;
            }

            gm.collectedCollectibles.Add(collectibleID);
            Destroy(gameObject);
        }

        private void UpgradePlayerHealth(GameObject player)
        {
            var healthScript = player.GetComponent<PlayerHealth>();
            if (healthScript != null)
            {
                healthScript.IncreaseMaxHealth(maxHealthIncrease);
                // Sync with the manager's persistent data
                GameManager.Instance.playerMaxHealth += maxHealthIncrease;
                Debug.Log("<b>MAX HEALTH INCREASED!</b>");
            }
        }
    }
}
