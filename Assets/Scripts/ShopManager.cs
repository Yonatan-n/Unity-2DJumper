using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;
using TMPro;

public enum ShopItemID
{
    extra_jump,
    extra_life,
    magazine_plus_3, magazine_plus_5,
    gem_1, gems_2,
    move_player_left,
}

[System.Serializable]
public class ShopItemInstance
{
    public ShopItemData data;
    public int price;

    public ShopItemInstance(ShopItemData data, int price)
    {
        this.data = data;
        this.price = price;
    }
}

public class ShopManager : MonoBehaviour
{
    [Header("UI")]
    public Transform itemContainer;
    public ShopItemUI itemPrefab;

    [Header("Items")]
    public ShopItemData extraLife;
    public ShopItemData extraJump;
    public ShopItemData bulletsItem;
    [FormerlySerializedAs("keyItem")]
    public ShopItemData gemItem;
    public ShopItemData moveLeft;
    [FormerlySerializedAs("KeysCounter")]
    [SerializeField] TextMeshProUGUI GemsCounter;
    public string GetShopItemName(ShopItemID itemId)
    {
        if (itemId == ShopItemID.magazine_plus_3)
            return "+3 bullets";
        if (itemId == ShopItemID.magazine_plus_5)
            return "+5 bullets";
        if (itemId == ShopItemID.gem_1)
            return "1 gem";
        if (itemId == ShopItemID.gems_2)
            return "2 gems";
        if (itemId == ShopItemID.move_player_left)
            return "better position";
        if (itemId == ShopItemID.extra_jump)
            return "+1 jump";
        if (itemId == ShopItemID.extra_life)
            return "+1 life";
        return itemId.ToString().Replace("_", " ");
    }

    void OnEnable()
    {
        BuildShop();
    }
    void UpdateGemsCounter()
    {
        GemsCounter.text = $"{PlayerData.GetIntById(PlayerData.GemsId)}";
    }

    void BuildShop()
    {
        Debug.Log("build shop");
        UpdateGemsCounter();
        // clear old items
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogWarning("ShopManager: GameManager not ready yet");
            return;
        }
        List<ShopItemInstance> items = new()
        {
            // Extra life
            new ShopItemInstance(
            extraLife,
            ExponentialPrice(200, gm.Lives)
        ),
            // Extra jump
            new ShopItemInstance(
            extraJump,
            ExponentialPrice(800, gm.maxJumps)
        ),
            // Bullets / gems rotation
            AddBulletOrGem()
        };
        if (gm.MoveLeftBought < gm.MAX_MOVE_LEFT)
        {
            // Move player left
            items.Add(
                new ShopItemInstance(
                moveLeft,
                ExponentialPrice(600, gm.MoveLeftBought)));
        }


        foreach (var item in items)
        {
            Debug.Log($"Creating item: {item.data.displayName} for {item.price}");
            Instantiate(itemPrefab, itemContainer)
                .Setup(item, this);
        }
    }

    int ExponentialPrice(int basePrice, int timesBought)
    {
        return basePrice * (1 << timesBought);
    }

    ShopItemInstance AddBulletOrGem()
    {
        var gm = GameManager.Instance;
        int gems = PlayerData.GetIntById(PlayerData.GemsId);
        int cycle = gm.level % 3;
        if (cycle == 0)
            return new ShopItemInstance(bulletsItem, ExponentialPrice(200, gm.extraBulletsBought));
        else if (cycle == 1)
            return new ShopItemInstance(gemItem, ExponentialPrice(1000, gems));
        else
            return new ShopItemInstance(gemItem, ExponentialPrice(1800, gems));
    }

    public bool TryBuy(ShopItemInstance item)
    {
        if (GameManager.Instance.Coins < item.price)
        {
            AudioManager.Instance.ShopNoMoney();
            // maybe add some shake effect later
            return false;
        }
        AudioManager.Instance.ShopYes();
        GameManager.Instance.Coins -= item.price;
        ApplyItem(item);
        StatsTracker.Instance.OnCoinSpent(item.price);
        return true;
    }

    void BuyGem(int amount)
    {
        PlayerData.SetIntById(PlayerData.GemsId, amount, true);
        UpdateGemsCounter();
    }

    void ApplyItem(ShopItemInstance item)
    {
        var gm = GameManager.Instance;
        var shopActions = new Dictionary<ShopItemID, Action>{
            { ShopItemID.extra_life,        () => gm.Lives++ },
            { ShopItemID.extra_jump,        () => gm.maxJumps++ },
            { ShopItemID.magazine_plus_3,   () => gm.MaxAmmo+=3 },
            { ShopItemID.magazine_plus_5,   () => gm.MaxAmmo+=5 },
            { ShopItemID.gem_1,             () => BuyGem(1)},
            { ShopItemID.gems_2,            () => BuyGem(2)},
            { ShopItemID.move_player_left,  () => gm.MoveLeftBought++ },
        };

        if (shopActions.TryGetValue(item.data.id, out var action))
        {
            action();
        }
    }
}
