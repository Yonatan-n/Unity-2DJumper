using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GearItemButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Button button;
    [SerializeField] GameObject Price;
    GearItem _gearItem;

    public void Setup(GearItem gearItem, System.Action<GearItem> clickCallback)
    {
        _gearItem = gearItem;
        title.text = _gearItem.title;
        button.onClick.AddListener(() => { clickCallback(_gearItem); RefreshUI(); });
        icon.sprite = _gearItem.Icon;
        Debug.Log("setup button: " + _gearItem.title);
        RefreshUI();
    }
    private GearButtonState GetState()
    {
        if (!PlayerData.IsOwned(_gearItem))
            return GearButtonState.Buyable;

        if (PlayerData.GetEquippedId(_gearItem.Slot) == _gearItem.Id)
            return GearButtonState.Unequipable;

        return GearButtonState.Equipable;
    }
    public void RefreshUI()
    {
        var state = GetState();
        var label = button.GetComponentInChildren<TextMeshProUGUI>();
        var priceLabel = Price.GetComponentInChildren<TextMeshProUGUI>();
        switch (state)
        {
            case GearButtonState.Buyable:
                label.text = "Buy";
                int price = _gearItem.Price;
                var priceStr = $"{price} {(price == 1 ? "key" : "keys")}";
                priceLabel.text = priceStr;
                Price.SetActive(true);
                break;

            case GearButtonState.Equipable:
                label.text = "Equip";
                Price.SetActive(false);
                break;

            case GearButtonState.Unequipable:
                label.text = "Remove";
                Price.SetActive(false);
                break;
        }
    }
}
