using UnityEngine;

public class Obstacle : MonoBehaviour
{

    void Start()
    {

    }
    public void Destroyed(bool isShoot)
    {
        CoinsEarned source;
        if (gameObject.CompareTag("Enemy"))
        {
            source = CoinsEarned.Enemy;
        }
        else if (gameObject.CompareTag("FlyingEnemy"))
        {
            source = CoinsEarned.FlyingEnemy;
        }
        else
        {
            source = CoinsEarned.Obstacle;
        }
        if (!isShoot)
        {
            source = CoinsEarned.JumpOver;
        }
        Debug.Log("Obstacle destroyed " + source);
        Destroy(gameObject);
        GameManager.Instance.earnedCoins(source);
    }
    void Update()
    {
        var moveSpeed = GroundMover.Instance.speed;
        float speedPercentageIncrease;
        if (gameObject.CompareTag("Enemy"))
        {
            speedPercentageIncrease = 1.3f;
        }
        else
        {
            speedPercentageIncrease = 1f;
        }
        transform.position = transform.position + (Vector3.left * moveSpeed * Time.deltaTime * speedPercentageIncrease);
    }
}
