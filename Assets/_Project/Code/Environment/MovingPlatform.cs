using System.Collections.Generic;
using TRIA.Core.Constants;
using UnityEngine;

namespace TRIA.Environment
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovingPlatform : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField]
        private float speed = 3f;

        [SerializeField]
        private Vector2[] waypoints;

        private Rigidbody2D rb;
        private Vector2[] globalWaypoints;
        private int targetIndex = 0;

        // We track the platform's velocity to "push" the player
        private Vector2 platformVelocity;
        private Vector2 previousPosition;

        // List of all characters currently standing on this platform
        private List<Rigidbody2D> trackedRigidbodies = new List<Rigidbody2D>();

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void Start()
        {
            globalWaypoints = new Vector2[waypoints.Length + 1];
            globalWaypoints[0] = transform.position;
            for (int i = 0; i < waypoints.Length; i++)
                globalWaypoints[i + 1] = (Vector2)transform.position + waypoints[i];

            previousPosition = rb.position;
        }

        private void FixedUpdate()
        {
            MovePlatform();

            // Calculate how much we moved this frame
            platformVelocity = (rb.position - previousPosition) / Time.fixedDeltaTime;
            previousPosition = rb.position;

            // Apply this platform's movement to anyone standing on it
            foreach (var characterRb in trackedRigidbodies)
            {
                if (characterRb != null)
                {
                    // This "glues" the player to the platform without parenting!
                    characterRb.position += platformVelocity * Time.fixedDeltaTime;
                }
            }
        }

        private void MovePlatform()
        {
            Vector2 target = globalWaypoints[targetIndex];
            rb.position = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);

            if (Vector2.Distance(rb.position, target) < 0.01f)
            {
                targetIndex = (targetIndex + 1) % globalWaypoints.Length;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(Tags.Player))
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null && !trackedRigidbodies.Contains(playerRb))
                {
                    trackedRigidbodies.Add(playerRb);
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(Tags.Player))
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (trackedRigidbodies.Contains(playerRb))
                {
                    trackedRigidbodies.Remove(playerRb);
                }
            }
        }

        // Visualization in the Scene Window
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Vector2 startPos = Application.isPlaying
                ? globalWaypoints[0]
                : (Vector2)transform.position;
            Vector2 lastPos = startPos;

            if (waypoints == null)
                return;

            foreach (var offset in waypoints)
            {
                Vector2 currentPos = startPos + offset;
                Gizmos.DrawSphere(currentPos, 0.2f);
                Gizmos.DrawLine(lastPos, currentPos);
                lastPos = currentPos;
            }
            // Draw line back to start
            Gizmos.DrawLine(lastPos, startPos);
        }
    }
}
