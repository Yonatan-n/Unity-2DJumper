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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        back.onClick.AddListener(Back);
        keysCount.text = "Keys: " + PlayerData.getIntById(PlayerData.KeysId);
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
        }
    }

    private void OnItemSelected(GearItem item)
    {
        var state = GetState(item);

        if (state == GearButtonState.Buyable)
        {
            if (previewEquipment.TryBuy(item)) previewEquipment.Equip(item);
        }
        else if (state == GearButtonState.Equipable)
        {
            previewEquipment.Equip(item);
        }
        else if (state == GearButtonState.Unequipable)
        {
            previewEquipment.Unequip(item.Slot);
        }
        // RefreshUI(item);
    }

    private GearButtonState GetState(GearItem item)
    {
        if (!PlayerData.IsOwned(item))
            return GearButtonState.Buyable;
        if (PlayerData.GetEquippedId(item.Slot) == item.Id)
            return GearButtonState.Unequipable;
        return GearButtonState.Equipable;
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
