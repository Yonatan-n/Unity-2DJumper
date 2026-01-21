using UnityEngine;

public class PipeMover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float deadZone = -45f;

    void Start()
    {

    }

    void Update()
    {
        transform.position = transform.position + (Vector3.left * moveSpeed * Time.deltaTime);
        if (transform.position.x < deadZone)
        {
            Destroy(gameObject);
        }
    }
}
