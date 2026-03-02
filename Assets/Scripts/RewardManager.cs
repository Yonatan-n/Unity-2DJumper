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
        Spawn(worldPos, (int)source / 40, coinTarget, () => OnCoinCollected(source));
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

    private void OnCoinCollected(CoinsEarned source)
    {
        // Add coin to PlayerData here
        GameManager.Instance.earnedCoins(source);
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