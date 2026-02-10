using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public Button buyButton;

    ShopItemInstance item;
    ShopManager shop;

    public void Setup(ShopItemInstance item, ShopManager shop)
    {
        this.item = item;
        this.shop = shop;

        icon.sprite = item.data.icon;
        nameText.text = item.data.displayName;
        priceText.text = item.price.ToString();

        buyButton.onClick.AddListener(OnBuy);
    }

    void OnBuy()
    {
        if (shop.TryBuy(item))
        {
            // remove item if bought
            Destroy(gameObject);
        }
    }
}
