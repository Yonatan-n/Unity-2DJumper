using UnityEngine;
using System.Collections.Generic;

public class PlayerEquipment : MonoBehaviour
{
    [System.Serializable]
    public class SlotBinding
    {
        public GearSlot Slot;
        public SpriteRenderer Renderer;
    }

    [SerializeField] private List<SlotBinding> slotBindings;

    private Dictionary<GearSlot, SpriteRenderer> slotMap;

    private void Awake()
    {
        slotMap = new Dictionary<GearSlot, SpriteRenderer>();
        foreach (var binding in slotBindings)
            slotMap[binding.Slot] = binding.Renderer;
    }

    public void Equip(GearItem item)
    {
        if (!slotMap.TryGetValue(item.Slot, out var renderer))
            return;

        renderer.sprite = item.Icon;
        renderer.transform.localPosition = item.LocalOffset;
        renderer.transform.localRotation = Quaternion.Euler(item.LocalRotation);
        renderer.transform.localScale = item.LocalScale;
        PlayerData.SetEquipped(item);
    }

    public bool TryBuy(GearItem item)
    {
        if (PlayerData.IsOwned(item))
            return true;

        int keys = PlayerData.GetIntById(PlayerData.KeysId, 0);

        if (keys < item.Price)
        {
            AudioManager.Instance.ShopNoMoney();
            return false;
        }

        AudioManager.Instance.ShopYes();
        PlayerData.SetIntById(PlayerData.KeysId, -item.Price, true);
        PlayerData.SetOwned(item);
        return true;
    }

    public void Unequip(GearSlot slot)
    {
        if (slotMap.TryGetValue(slot, out var renderer))
        {
            renderer.sprite = null;
            renderer.transform.localRotation = Quaternion.identity;
            renderer.transform.localPosition = Vector3.zero;
            renderer.transform.localScale = Vector3.one;
            PlayerData.setEmptyGear(slot);
        }
    }
}
