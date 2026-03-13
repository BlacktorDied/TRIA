using TRIA.Core.Constants;
using UnityEngine;

namespace TRIA.Core
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class TransitionZone : MonoBehaviour
    {
        [Header("Destination Settings")]
        [Tooltip("The exact name of the Unity Scene to load (e.g., AssemblyChamber_01)")]
        public string targetSceneName;

        [Tooltip("The ID of the Spawn Point in the next room (e.g., LeftDoor)")]
        public string targetSpawnPointId;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Safely check if the object touching the zone is the Player
            if (collision.CompareTag(Tags.Player))
            {
                Debug.Log($"Player entered Transition Zone. Heading to {targetSceneName}...");
                SceneLoader.Instance.TransitionToScene(targetSceneName, targetSpawnPointId);
            }
        }

        private void OnDrawGizmos()
        {
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            if (boxCollider == null)
                return;

            // Save the default matrix so we don't mess up other gizmos
            Matrix4x4 oldMatrix = Gizmos.matrix;

            // Apply the object's transform matrix to the Gizmo.
            // This ensures the Gizmo rotates and scales exactly like the GameObject.
            Gizmos.matrix = transform.localToWorldMatrix;

            // Draw a semi-transparent fill
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // Green with 30% opacity
            Gizmos.DrawCube(boxCollider.offset, boxCollider.size);

            // Draw a solid wireframe outline
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boxCollider.offset, boxCollider.size);

            // Restore the original matrix
            Gizmos.matrix = oldMatrix;
        }
    }
}
