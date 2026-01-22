using UnityEngine;

public class Obstacle : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {
        var moveSpeed = GroundMover.speed;
        float speedPrecentIncrease;
        if (gameObject.CompareTag("Enemy"))
        {
            speedPrecentIncrease = 1.3f;
        }
        else
        {
            speedPrecentIncrease = 1f;
        }
        transform.position = transform.position + (Vector3.left * moveSpeed * Time.deltaTime * speedPrecentIncrease);
    }
}
