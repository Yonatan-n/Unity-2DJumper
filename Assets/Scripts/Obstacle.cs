using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool WasShotted = false;
    public bool is_first = false;

    private CoinsEarned MapSourceToCoin(bool isShoot)
    {
        return (isShoot, gameObject.tag) switch
        {
            (false, _) => CoinsEarned.JumpOver,
            (true, Tags.Enemy) => CoinsEarned.Enemy,
            (true, Tags.FlyingEnemy) => CoinsEarned.FlyingEnemy,
            (true, _) => CoinsEarned.Obstacle
        };
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
        RewardManager.Instance.SpawnCoins(transform.position, MapSourceToCoin(WasShotted));
        Destroy(gameObject);
    }
}
