using UnityEngine;


public enum ObstacleType
{
    Barrel, Tree, Car, WalkingEnemy, FlyingEnemy
}

public class Obstacle : MonoBehaviour
{
    public ObstacleType type;
    public bool WasShotted = false;
    public bool is_first = false;
    private bool hasBeenVisible = false;

    private CoinsEarned MapSourceToCoin(bool isShoot)
    {
        return (isShoot, gameObject.tag) switch
        {
            (false, Tags.Enemy) => CoinsEarned.EnemyAlive,
            (false, Tags.FlyingEnemy) => CoinsEarned.EnemyAlive,
            (false, _) => CoinsEarned.JumpOver,
            (true, Tags.Enemy) => CoinsEarned.Enemy,
            (true, Tags.FlyingEnemy) => CoinsEarned.FlyingEnemy,
            (true, _) => CoinsEarned.Obstacle
        };
    }

    void Start()
    {
    }

    void Update()
    {
        var moveSpeed = GroundMover.Instance.speed;
        transform.position = transform.position + (Vector3.left * moveSpeed * Time.deltaTime * GetSpeedIncrease(gameObject));

        if (!hasBeenVisible && IsVisibleToCamera()) hasBeenVisible = true;
        if (hasBeenVisible && IsOffLeftOrTop()) Cleanup();
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
    private bool IsVisibleToCamera()
    {
        Renderer renderer = GetComponent<Renderer>();
        return renderer.isVisible;
    }

    private bool IsOffLeftOrTop()
    {
        var viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        return viewportPos.x < 0 || viewportPos.y > 1;
    }

    private void Cleanup()
    {
        if (!gameObject.scene.isLoaded) return; // for debug, stop errors
        RewardManager.Instance.SpawnCoins(transform.position, MapSourceToCoin(WasShotted));
        Destroy(gameObject);
    }
}
