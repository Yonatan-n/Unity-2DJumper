using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GearPage : MonoBehaviour
{
    public List<Sprite> Hats;
    public List<Sprite> Glasses;
    public List<Sprite> Guns;
    [SerializeField] Button back;
    [SerializeField] TextMeshProUGUI keysCount;
    [SerializeField] Transform GridContainer;
    [SerializeField] GearItemButton ItemGearPrefab;
    [SerializeField] private GearDatabase database;
    [SerializeField] private PlayerEquipment previewEquipment;
    private List<GearItemButton> buttons = new List<GearItemButton>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // PlayerData.ResetAll();
        // PlayerData.SetIntById(PlayerData.KeysId, 300);
        previewEquipment.LoadFromPlayerData();
        back.onClick.AddListener(Back);
        updateKeysCount();
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
            updateKeysCount();
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
    void updateKeysCount()
    {
        keysCount.text = "Keys: " + PlayerData.GetIntById(PlayerData.KeysId);
    }
    void Back()
    {
        SceneLoader.Instance.LoadSceneByName("MainMenu");

    }
    // Update is called once per frame
    void Update()
    {

    }
}
