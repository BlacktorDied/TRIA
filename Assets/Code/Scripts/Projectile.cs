using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Variables

    [SerializeField] public Vector2 direction;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float lifeTime = 4f;

    private float timer = 0f;

    #endregion

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
        timer += Time.deltaTime;
        if (timer > lifeTime) Destroy(gameObject);
    }
}
