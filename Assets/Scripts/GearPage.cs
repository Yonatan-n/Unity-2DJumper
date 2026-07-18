using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GearPage : MonoBehaviour
{
    public List<Sprite> Hats;
    public List<Sprite> Glasses;
    public List<Sprite> Guns;
    [SerializeField] Button back;
    [FormerlySerializedAs("keysCount")]
    [SerializeField] TextMeshProUGUI gemsCount;
    [SerializeField] Transform GridContainer;
    [SerializeField] GearItemButton ItemGearPrefab;
    [SerializeField] private GearDatabase database;
    [SerializeField] private PlayerEquipment previewEquipment;
    private List<GearItemButton> buttons = new List<GearItemButton>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // PlayerData.SetIntById(PlayerData.GemsId, 300);
        previewEquipment.LoadFromPlayerData();
        back.onClick.AddListener(Back);
        updateGemsCount();
        BuildGrid();
    }
    private void BuildGrid()
    {
        foreach (Transform child in GridContainer)
            Destroy(child.gameObject);

        foreach (var item in database.Items)
        {
            var _gearItem = Instantiate(ItemGearPrefab, GridContainer);
            _gearItem.Setup(item, OnItemSelected);
            buttons.Add(_gearItem);
        }
    }

    private void OnItemSelected(GearItem item)
    {
        var state = GetState(item);

        if (state == GearButtonState.Buyable && previewEquipment.TryBuy(item))
        {
            previewEquipment.Equip(item);
            updateGemsCount();
            if (item.Slot == GearSlot.Gun)
            {
                StatsTracker.Instance.OnGunUnlocked();
            }
        }
        else if (state == GearButtonState.Equipable)
        {
            previewEquipment.Equip(item);
        }
        else if (state == GearButtonState.Unequipable)
        {
            previewEquipment.Unequip(item.Slot);
        }
        RefreshAllButtons();
    }
    private void RefreshAllButtons()
    {
        foreach (var b in buttons)
            b.RefreshUI();
    }
    private GearButtonState GetState(GearItem item)
    {
        if (!PlayerData.IsOwned(item))
            return GearButtonState.Buyable;
        if (PlayerData.GetEquippedId(item.Slot) == item.Id)
            return GearButtonState.Unequipable;
        return GearButtonState.Equipable;
    }
    void updateGemsCount()
    {
        gemsCount.text = "" + PlayerData.GetIntById(PlayerData.GemsId);
    }
    void Back()
    {
        SceneLoader.Instance.LoadSceneByName("MainMenu");

    }
    [ContextMenu("Reset All Gear")]
    public void ResetAllGear()
    {
        // Clear all equipped slots
        foreach (GearSlot slot in System.Enum.GetValues(typeof(GearSlot)))
            PlayerPrefs.DeleteKey(PlayerData.GetSlotKey(slot));

        // Clear all owned items
        foreach (var item in database.Items)
            PlayerPrefs.DeleteKey(PlayerData.GetOwnedKey(item.Id));

        PlayerPrefs.Save();
    }

}
