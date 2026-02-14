using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
    public const string KeysId = "keys";
    public const string highscoreId = "highscore";
    public const string isGodMode = "isGodMode";
    public const string IsScreenShake = "IsScreenShake";
    public const string IsEarRinging = "IsEarRinging";
    public const string GearHatId = "gear_hat_id";
    public const string GearGlassesId = "gear_glasses_id";
    public const string GearGunId = "gear_gun_id";
    // gear db
    private static GearDatabase _gearDatabase;
    public static GearDatabase gearDatabase
    {
        get
        {
            if (_gearDatabase == null)
                _gearDatabase = Resources.Load<GearDatabase>("MainGearDatabase");
            return _gearDatabase;
        }
    }


    public static GearItem GetById(string id)
    {
        return gearDatabase.Items.Find(i => i.Id == id);
    }

    public static List<GearItem> GetBySlot(GearSlot slot)
    {
        return gearDatabase.Items.FindAll(i => i.Slot == slot);
    }
    // Main
    public static int GetIntById(string _id, int _default = 0) => PlayerPrefs.GetInt(_id, _default);
    public static void SetIntById(string _id, int value, bool inc = false) =>
        PlayerPrefs.SetInt(_id, inc ? GetIntById(_id) + value : value);
    public static bool GetBoolById(string _id, bool _default = false) => PlayerPrefs.GetInt(_id, _default ? 1 : 0) == 1;
    public static void SetBoolById(string _id, bool value) => PlayerPrefs.SetInt(_id, value ? 1 : 0);

    // Ownership
    private static string GetOwnedKey(string gearId)
    {
        return "gear_owned_" + gearId;
    }

    public static bool IsOwned(GearItem item)
    {
        return PlayerPrefs.GetInt(GetOwnedKey(item.Id), 0) == 1;
    }

    public static void SetOwned(GearItem item, bool owned = true)
    {
        PlayerPrefs.SetInt(GetOwnedKey(item.Id), owned ? 1 : 0);
    }


    // Gear
    public static GearItem getGearById(string gearItemId)
    {
        return gearDatabase.GetById(gearItemId);
    }

    private static string GetSlotKey(GearSlot slot)
    {
        return "gear_slot_" + slot;
    }

    public static void SetEquipped(GearItem item)
    {
        PlayerPrefs.SetString(GetSlotKey(item.Slot), item.Id);
    }

    public static string GetEquippedId(GearSlot slot)
    {
        return PlayerPrefs.GetString(GetSlotKey(slot), "empty");
    }


    public static void setEmptyGear(GearSlot slot)
    {
        PlayerPrefs.SetString("gear_slot_" + slot.ToString(), "empty");
    }
    // for debugging
    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
