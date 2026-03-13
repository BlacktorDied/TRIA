using TRIA.Core;
using TRIA.Core.Constants;
using TRIA.Player;
using UnityEngine;

namespace TRIA.Items
{
    public class DashUnlockCollectible : MonoBehaviour
    {
        [Header("Identity")]
        [Tooltip("Unique ID for this ability unlock (e.g., 'Ability_Dash')")]
        [SerializeField]
        private string collectibleID = "Ability_Dash";

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

            // 1. Tell the Player locally that they can now dash
            PlayerAbilityHandler abilities = other.GetComponent<PlayerAbilityHandler>();
            if (abilities != null)
            {
                abilities.UnlockAbility(AbilityType.Dash);
            }

            // 2. Tell the GameManager globally so it can be saved
            if (GameManager.Instance != null)
            {
                GameManager.Instance.dashUnlocked = true;
                if (!GameManager.Instance.collectedCollectibles.Contains(collectibleID))
                {
                    GameManager.Instance.collectedCollectibles.Add(collectibleID);
                }
            }

            Debug.Log("<color=cyan><b>DASH ABILITY UNLOCKED!</b></color>");

            // Optional: Instantiate a "Pickup Effect" prefab here

            Destroy(gameObject);
        }
    }
}
