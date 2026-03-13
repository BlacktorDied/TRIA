using TRIA.Core.Constants;
using TRIA.Player;
using UnityEngine;

namespace TRIA.World
{
    public class Checkpoint : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField]
        private Color activeColor = Color.green;

        [SerializeField]
        private SpriteRenderer flagRenderer;

        private bool _isActivated = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isActivated)
                return;

            if (other.CompareTag(Tags.Player))
            {
                PlayerHealth player = other.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    // Update the player's respawn point
                    player.SetCheckpoint(transform.position);

                    ActivateCheckpoint();
                }
            }
        }

        private void ActivateCheckpoint()
        {
            _isActivated = true;
            Debug.Log("<color=green><b>Checkpoint Activated!</b></color>");

            // Visual feedback: Change color of the flag/light
            if (flagRenderer != null)
            {
                flagRenderer.color = activeColor;
            }

            // TODO: Play a "Save" sound effect
        }
    }
}
