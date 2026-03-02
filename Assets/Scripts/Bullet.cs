using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float moveSpeed = 12;
    [SerializeField] GameObject bulletSplit;

    public static readonly List<string> enemyTags = new() { Tags.Enemy, Tags.FlyingEnemy };

    float deadZone;
    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Camera cam = Camera.main;
        deadZone = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + 5f;
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
        if (enemyTags.Contains(other.gameObject.tag))
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy.TakeDamage(1))
                Spawner.Instance.ShootObstacleRemove(other.gameObject);
        }
        else
        {
            Instantiate(bulletSplit, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
