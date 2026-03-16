using TRIA.Core;
using TRIA.Player;
using UnityEngine;

namespace TRIA.World
{
    public class Checkpoint : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                // Update the global checkpoint in GameManager
                GameManager.Instance.SetCheckpoint(transform.position);
                Debug.Log($"Checkpoint reached at {transform.position}");
            }
        }
    }
}
