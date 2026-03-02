using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool WasShotted = false;
    void Start()
    {

    }
    public void Destroyed(bool isShoot)
    {
        CoinsEarned source;
        if (gameObject.CompareTag(Tags.Enemy))
        {
            source = CoinsEarned.Enemy;
        }
        else if (gameObject.CompareTag(Tags.FlyingEnemy))
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
        transform.position = transform.position + (Vector3.left * moveSpeed * Time.deltaTime * GetSpeedIncrease(gameObject));
    }

    public static float GetSpeedIncrease(GameObject gameObject)
    {
        return gameObject.tag switch
        {
            Tags.Enemy => 1.3f,
            Tags.FlyingEnemy => 0.3f,
            _ => 1f
        };
    }
    void OnBecameInvisible()
    {
        Debug.Log($"OnBecameInvisible {WasShotted}");
        Destroyed(WasShotted);
    }
}
