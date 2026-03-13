using TRIA.Core.Constants;
using Unity.Cinemachine; // Required to interact with the Virtual Camera!
using UnityEngine;

namespace TRIA.Camera
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class CameraTargetFinder : MonoBehaviour
    {
        private CinemachineCamera vcam;

        private void Awake()
        {
            vcam = GetComponent<CinemachineCamera>();
        }

        private void Start()
        {
            FindAndFollowPlayer();
        }

        public void FindAndFollowPlayer()
        {
            // Use our safe Constants to find the persistent player
            GameObject player = GameObject.FindGameObjectWithTag(Tags.Player);

            if (player != null)
            {
                // Assign the player's transform to the camera's Follow target
                vcam.Follow = player.transform;

                // Optional: If you want the camera to also rotate to look at the player (rare in 2D, but good to have)
                // vcam.LookAt = player.transform;
            }
            else
            {
                Debug.LogWarning(
                    $"CameraTargetFinder: Could not find any object with tag '{Tags.Player}' to follow!"
                );
            }
        }
    }
}
