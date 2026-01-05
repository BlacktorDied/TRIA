using UnityEngine;

public class TurretEnemy : MonoBehaviour
{
    [Header("Combat")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float bulletSpeed = 12f;
    public float bulletLifetime = 5f;
    public float shootInterval = 1f;

    [Header("Detection")]
    public float detectionRange = 6f; // Only shoot when player is within this range

    Transform player;
    float shootTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Ensure gravity doesn't affect it
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.isKinematic = true;
            rb.gravityScale = 0;
        }
    }

    void Update()
    {
        if (!player || !shootPoint || !bulletPrefab) return;

        // Check distance to player
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > detectionRange)
        {
            shootTimer = 0f; // reset timer when player out of range
            return; // don't shoot
        }

        // Optional: rotate turret to face player
        Vector2 direction = (player.position - shootPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        shootPoint.rotation = Quaternion.Euler(0, 0, angle);

        // Shooting
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            Shoot(direction);
            shootTimer = 0f;
        }
    }

    void Shoot(Vector2 dir)
    {
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);

        Projectile proj = bullet.GetComponent<Projectile>();
        if (proj)
        {
            proj.direction = dir;
            proj.speed = bulletSpeed;
            proj.shooter = this.gameObject; // IMPORTANT: prevents bullet from hitting turret
        }

        Destroy(bullet, bulletLifetime);
    }
}
