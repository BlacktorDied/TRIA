using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 6f;
    public float lifeTime = 4f;

    private float timer = 0f;

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer > lifeTime)
            Destroy(gameObject);
    }
}
