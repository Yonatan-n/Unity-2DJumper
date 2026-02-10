using System.Collections.Generic;
using UnityEngine;
using System;

public enum ShopItemID
{
    extra_jump,
    extra_life,
    magazine_plus_2, magazine_plus_1,
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

    int shopIndex;
    public string GetShopItemName(ShopItemID itemId)
    {
        if (itemId == ShopItemID.magazine_plus_2)
            return "+2 bullets";
        if (itemId == ShopItemID.magazine_plus_1)
            return "+1 bullets";
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

    void BuildShop()
    {
        Debug.Log("build shop");
        // clear old items
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        var gm = GameManager.Instance;
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
        int cycle = gm.level % 3;
        if (cycle == 0)
            return new ShopItemInstance(bulletsItem, ExponentialPrice(200, gm.extraBulletsBought));
        else if (cycle == 1)
            return new ShopItemInstance(keyItem, ExponentialPrice(1000, gm.Keys));
        else
            return new ShopItemInstance(keyItem, ExponentialPrice(1800, gm.Keys));
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
        return true;
    }

    void ApplyItem(ShopItemInstance item)
    {
        var gm = GameManager.Instance;
        var shopActions = new Dictionary<ShopItemID, Action>{
            { ShopItemID.extra_life,        () => gm.Lives++ },
            { ShopItemID.extra_jump,        () => gm.maxJumps++ },
            { ShopItemID.magazine_plus_1,   () => gm.gun = gm.gun with { BulletCount = gm.gun.BulletCount + 1 } },
            { ShopItemID.magazine_plus_2,   () => gm.gun = gm.gun with { BulletCount = gm.gun.BulletCount + 2 } },
            { ShopItemID.key_1,             () => gm.Keys +=1 },
            { ShopItemID.keys_2,            () => gm.Keys += 2 },
            { ShopItemID.move_player_left,  () => gm.MoveLeftBought++ },
        };

        if (shopActions.TryGetValue(item.data.id, out var action))
        {
            action();
        }
    }
}
