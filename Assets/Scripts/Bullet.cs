using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float moveSpeed = 12;
    [SerializeField] GameObject bulletSplit;
    private Vector2 _direction;
    public static readonly List<string> enemyTags = new() { Tags.Enemy, Tags.FlyingEnemy };

    float deadZoneX;
    float deadZoneY;
    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Camera cam = Camera.main;
        var topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));
        deadZoneX = topRight.x + 5f;
        deadZoneY = topRight.y + 5f;
        // Convert the Z rotation angle into a movement direction
        float angle = transform.rotation.eulerAngles.z + 90f;
        Debug.Log($"Bullet spawned with Z rotation: {angle}");
        float radians = angle * Mathf.Deg2Rad;
        _direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        Debug.Log($"Bullet direction: {_direction}");
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + _direction * moveSpeed * Time.fixedDeltaTime);
        if (rb.position.x > deadZoneX || rb.position.y > deadZoneY)
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
            Destroy(gameObject);
        }
        else if (!other.gameObject.CompareTag(Tags.Bullet)) // prevent bullet colliding when entering a level
        {
            Instantiate(bulletSplit, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
