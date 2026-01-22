using UnityEngine;

public class Obstacle : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {
        var moveSpeed = GroundMover.speed;
        transform.position = transform.position + (Vector3.left * moveSpeed * Time.deltaTime);
    }
}
