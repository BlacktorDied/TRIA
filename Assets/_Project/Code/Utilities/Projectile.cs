using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public Vector2 direction;
    [HideInInspector] public float speed;
    [HideInInspector] public GameObject shooter; // Who fired this bullet

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore the shooter itself
        if (shooter != null && other.gameObject == shooter)
            return;

        // Only stop for player or environment (tilemap, platforms, etc.)
        if (other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Kill();
        }
    }
}
