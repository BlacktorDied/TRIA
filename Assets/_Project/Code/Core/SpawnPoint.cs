using UnityEngine;

namespace TRIA.Core
{
    public class SpawnPoint : MonoBehaviour
    {
        [Tooltip(
            "Automatically matches the parent object's name (or this object's name if no parent)."
        )]
        public string spawnId;

#if UNITY_EDITOR
        // This runs automatically in the Unity Editor to prevent manual typos
        private void OnValidate()
        {
            if (transform.parent != null)
            {
                // Grabs the name of the TransitionZone it is nested inside
                spawnId = transform.parent.name;
            }
            else
            {
                // Fallback: use its own name if it's not nested
                spawnId = gameObject.name;
            }
        }
#endif

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up);
        }
    }
}
