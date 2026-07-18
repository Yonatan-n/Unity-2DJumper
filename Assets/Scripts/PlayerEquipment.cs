using UnityEngine;
using System.Collections.Generic;

public class PlayerEquipment : MonoBehaviour
{
    [System.Serializable]
    public class SlotBinding
    {
        public GearSlot Slot;
        public Transform Root; // Offset layer (NOT animated)
    }

    [SerializeField] private List<SlotBinding> slotBindings;

    private Dictionary<GearSlot, Transform> slotMap;

    private void Awake()
    {
        slotMap = new Dictionary<GearSlot, Transform>();

        foreach (var binding in slotBindings)
            slotMap[binding.Slot] = binding.Root;
    }

    public void Equip(GearItem item)
    {
        if (!slotMap.TryGetValue(item.Slot, out var root))
            return;

        var renderer = root.GetComponentInChildren<SpriteRenderer>();
        if (renderer == null)
            return;

        renderer.sprite = item.Icon;

        root.localPosition = item.LocalOffset;
        root.localRotation = Quaternion.Euler(item.LocalRotation);
        root.localScale = item.LocalScale;
        renderer.flipX = item.flipX;
        PlayerData.SetEquipped(item);
    }

    public bool TryBuy(GearItem item)
    {
        if (PlayerData.IsOwned(item))
            return true;

        int gems = PlayerData.GetIntById(PlayerData.GemsId, 0);

        if (gems < item.Price)
        {
            AudioManager.Instance.ShopNoMoney();
            return false;
        }

        AudioManager.Instance.ShopYes();
        PlayerData.SetIntById(PlayerData.GemsId, -item.Price, true);
        PlayerData.SetOwned(item);
        return true;
    }

    public void Unequip(GearSlot slot)
    {
        if (!slotMap.TryGetValue(slot, out var root))
            return;

        var renderer = root.GetComponentInChildren<SpriteRenderer>();
        if (renderer != null)
            renderer.sprite = null;

        root.localRotation = Quaternion.identity;
        root.localPosition = Vector3.zero;
        root.localScale = Vector3.one;

        PlayerData.setEmptyGear(slot);
    }

    public void LoadFromPlayerData()
    {
        foreach (GearSlot slot in System.Enum.GetValues(typeof(GearSlot)))
        {
            var id = PlayerData.GetEquippedId(slot);
            if (id == PlayerData.empty)
                continue;

            var item = PlayerData.GetGearById(id);
            if (item != null)
                Equip(item);
        }
    }
}
