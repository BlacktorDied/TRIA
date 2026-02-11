using TRIA.Core;
using TRIA.Core.Constants;
using UnityEngine;

namespace TRIA.Items
{
    public class DashUnlockCollectible : MonoBehaviour
    {
        [Header("Identity")]
        [Tooltip("Unique ID for this ability unlock (e.g., 'Unlock_Dash_Ability')")]
        public string collectibleID = "Ability_Dash";

        private void Start()
        {
            if (GameManager.Instance == null)
                return;

            // If the manager already knows we have the dash, remove the pickup from the world
            if (
                GameManager.Instance.dashUnlocked
                || GameManager.Instance.collectedCollectibles.Contains(collectibleID)
            )
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(Tags.Player))
                return;

            // Unlock ability globally
            GameManager.Instance.dashUnlocked = true;
            GameManager.Instance.collectedCollectibles.Add(collectibleID);

            Debug.Log("<color=cyan><b>DASH ABILITY UNLOCKED!</b></color>");

            // Optional: Trigger a specific UI popup or tutorial here

            Destroy(gameObject);
        }
    }
}
