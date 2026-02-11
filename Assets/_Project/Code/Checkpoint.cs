using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.SetCheckpoint(transform.position);
            Debug.Log("Checkpoint reached!");
        }
    }
}
