using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "Shop/Item")]
public class ShopItemData : ScriptableObject
{
    public ShopItemID id; // "extra_life", "extra_jump", etc.
    public string displayName;
    public Sprite icon;
    public int basePrice;
    public string description;
}
