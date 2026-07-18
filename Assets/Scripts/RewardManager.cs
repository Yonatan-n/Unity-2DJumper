using System;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance;

    [Header("References")]
    [SerializeField] Canvas canvas;
    [SerializeField] RectTransform coinTarget;
    [SerializeField] RectTransform lifeTarget;
    [SerializeField] RectTransform ammoTarget;

    [Header("Pooling")]
    [SerializeField] RewardFlyToUI rewardPrefab;
    [SerializeField] int initialPoolSize = 30;
    private static readonly Dictionary<CoinsEarned, (int coinsAmount, int totalValue)> CoinRewards = new(){
        {CoinsEarned.EnemyAlive,     (0, 0)},
        {CoinsEarned.Obstacle,     (1, 40)},
        {CoinsEarned.Enemy,        (2, 100)},
        {CoinsEarned.FlyingEnemy,  (4, 400)},
        {CoinsEarned.JumpOver,     (1, 40)},
    };
    private readonly Queue<RewardFlyToUI> pool = new();

    private void Awake()
    {
        Instance = this;
        Prewarm();
    }

    private void Prewarm()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            var obj = Instantiate(rewardPrefab, canvas.transform);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    private RewardFlyToUI GetFromPool()
    {
        if (pool.Count > 0)
            return pool.Dequeue();

        var obj = Instantiate(rewardPrefab, canvas.transform);
        obj.gameObject.SetActive(false);
        return obj;
    }

    public void ReturnToPool(RewardFlyToUI reward)
    {
        reward.gameObject.SetActive(false);
        pool.Enqueue(reward);
    }

    public void SpawnCoins(Vector3 worldPos, CoinsEarned source)
    {
        if (source == CoinsEarned.Obstacle || source == CoinsEarned.JumpOver)
            GameManager.Instance.ObstaclePassedThisRun();

        var (_coinCount, totalValue) = CoinRewards[source];
        if (_coinCount == 0) return; // no coins
        int valuePerCoin = totalValue / _coinCount;
        Spawn(worldPos, _coinCount, coinTarget, () => GameManager.Instance.earnedCoins(valuePerCoin));
    }
    public void SpawnAmmo(Vector3 worldPos, int amount)
    {
        Spawn(worldPos, amount, ammoTarget, OnAmmoCollected);
    }
    public void SpawnLife(Vector3 worldPos, int amount)
    {
        Spawn(worldPos, amount, lifeTarget, OnLifeCollected);
    }

    private void Spawn(Vector3 worldPos,
                       int amount,
                       RectTransform target,
                       Action onCollected)
    {
        for (int i = 0; i < amount; i++)
        {
            var reward = GetFromPool();
            reward.transform.SetParent(canvas.transform);
            reward.gameObject.SetActive(true);

            reward.Initialize(worldPos,
                              target,
                              canvas,
                              onCollected,
                              ReturnToPool);
        }
    }

    private void OnLifeCollected()
    {
        // Add life here
    }
    private void OnAmmoCollected()
    {
        // todo:
    }
}