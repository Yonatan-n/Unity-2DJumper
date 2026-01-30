using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float moveSpeed = 12;
    [SerializeField] float deadZone = 33f;
    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + Vector2.right * moveSpeed * Time.fixedDeltaTime);
        if (rb.position.x > deadZone)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // bullets should not penetrate obstacles, but should kill enemies
        // FMJ can go through multiple enemies
        if (tag == "BulletFMJ")
        {
            // destroy multiple enemies
        }
        else
        {
            // normal bullet, only 1 enemy

        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            // blood particles
            AudioManager.Instance.EnemyIsHit();
            Spawner.Instance.ShootObstacleRemove(other.gameObject);
        }
        // yellow partials
        Destroy(gameObject);
    }
}
