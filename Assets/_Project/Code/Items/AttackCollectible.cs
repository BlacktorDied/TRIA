using TRIA.Core;
using TRIA.Core.Constants;
using TRIA.Player;
using UnityEngine;

namespace TRIA.Items
{
    public class AttackCollectible : MonoBehaviour
    {
        [Header("Identity")]
        [Tooltip("Unique ID for this specific pickup (e.g., 'Basement_Atk_1')")]
        public string collectibleID;

        [Header("Settings")]
        public int fragmentsToUpgrade = 4;
        public float attackPowerIncrease = 1f;

        private void Start()
        {
            // Safety check for GameManager
            if (GameManager.Instance == null)
                return;

            // Self-destruct if already in the player's collection
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

            // Increment fragments in the global manager
            gm.attackFragments++;
            Debug.Log(
                $"<color=orange>Attack Fragment Collected!</color> ({gm.attackFragments}/{fragmentsToUpgrade})"
            );

            // Check for stat upgrade
            if (gm.attackFragments >= fragmentsToUpgrade)
            {
                UpgradePlayerAttack(other.gameObject);
                gm.attackFragments = 0;
            }

            // Save to persistent world state
            gm.collectedCollectibles.Add(collectibleID);

            // TODO: Add VFX/SFX here
            Destroy(gameObject);
        }

        private void UpgradePlayerAttack(GameObject player)
        {
            var attackScript = player.GetComponent<PlayerAttack>();
            if (attackScript != null)
            {
                attackScript.IncreaseAttack(attackPowerIncrease);
                // Also update the persistent value in GameManager
                GameManager.Instance.playerAttack += attackPowerIncrease;
                Debug.Log("<b>ATTACK UPGRADED!</b> Power increased.");
            }
        }
    }
}
