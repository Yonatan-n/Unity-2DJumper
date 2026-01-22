using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float moveSpeed = 12;
    [SerializeField] float deadZone = 33f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Update()
    {
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        if (transform.position.x > deadZone)
        {
            Destroy(gameObject);
        }
    }
}
