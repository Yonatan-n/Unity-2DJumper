using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public enum ShopItemID
{
    extra_jump,
    extra_life,
    magazine_plus_3, magazine_plus_5,
    key_1, keys_2,
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
    public ShopItemData keyItem;
    public ShopItemData moveLeft;
    [SerializeField] TextMeshProUGUI KeysCounter;
    public string GetShopItemName(ShopItemID itemId)
    {
        if (itemId == ShopItemID.magazine_plus_3)
            return "+3 bullets";
        if (itemId == ShopItemID.magazine_plus_5)
            return "+5 bullets";
        if (itemId == ShopItemID.key_1)
            return "1 key";
        if (itemId == ShopItemID.keys_2)
            return "2 keys";
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
    void UpdateKeysCounter()
    {
        KeysCounter.text = $"Keys: {PlayerData.GetIntById(PlayerData.KeysId)}";
    }

    void BuildShop()
    {
        Debug.Log("build shop");
        UpdateKeysCounter();
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
            // Bullets / keys rotation
            AddBulletOrKey()
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

    ShopItemInstance AddBulletOrKey()
    {
        var gm = GameManager.Instance;
        int keys = PlayerData.GetIntById(PlayerData.KeysId);
        int cycle = gm.level % 3;
        if (cycle == 0)
            return new ShopItemInstance(bulletsItem, ExponentialPrice(200, gm.extraBulletsBought));
        else if (cycle == 1)
            return new ShopItemInstance(keyItem, ExponentialPrice(1000, keys));
        else
            return new ShopItemInstance(keyItem, ExponentialPrice(1800, keys));
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

    void BuyKey(int amount)
    {
        PlayerData.SetIntById(PlayerData.KeysId, amount, true);
        UpdateKeysCounter();
    }

    void ApplyItem(ShopItemInstance item)
    {
        var gm = GameManager.Instance;
        var shopActions = new Dictionary<ShopItemID, Action>{
            { ShopItemID.extra_life,        () => gm.Lives++ },
            { ShopItemID.extra_jump,        () => gm.maxJumps++ },
            { ShopItemID.magazine_plus_3,   () => gm.MaxAmmo+=3 },
            { ShopItemID.magazine_plus_5,   () => gm.MaxAmmo+=5 },
            { ShopItemID.key_1,             () => BuyKey(1)},
            { ShopItemID.keys_2,            () => BuyKey(2)},
            { ShopItemID.move_player_left,  () => gm.MoveLeftBought++ },
        };

        if (shopActions.TryGetValue(item.data.id, out var action))
        {
            action();
        }
    }
}
