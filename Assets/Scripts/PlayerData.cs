using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
    public const string GemsId = "gems";
    public const string highscoreId = "highscore";
    public const string isGodMode = "isGodMode";
    public const string IsScreenShake = "IsScreenShake";
    public const string IsEarRinging = "IsEarRinging";
    public const string IsStartingGrace = "IsStartingGrace";
    public const string MasterVolume = "MasterVolume";
    public const string GearHatId = "gear_hat_id";
    public const string GearGlassesId = "gear_glasses_id";
    public const string GearGunId = "gear_gun_id";
    public const string isFirstStart = "isFirstStart";
    private const string _gearSlotPrefix = "gear_slot_";
    public const string empty = "empty";
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

    public static List<GearItem> GetBySlot(GearSlot slot)
    {
        return gearDatabase.Items.FindAll(i => i.Slot == slot);
    }
    // Main
    public static int GetIntById(string _id, int _default = 0) => PlayerPrefs.GetInt(_id, _default);
    public static void SetIntById(string id, int value, bool inc = false)
    {
        if (inc)
            value += GetIntById(id);
        PlayerPrefs.SetInt(id, value);
    }
    public static float GetFloatById(string _id, float _default = 0) => PlayerPrefs.GetFloat(_id, _default);
    public static void SetFloatById(string _id, float value) => PlayerPrefs.SetFloat(_id, value);
    public static bool GetBoolById(string id, bool defaultValue = false)
    {
        if (PlayerPrefs.HasKey(id))
            return PlayerPrefs.GetInt(id) == 1;

        PlayerPrefs.SetInt(id, defaultValue ? 1 : 0);
        return defaultValue;

    }
    public static void SetBoolById(string _id, bool value) => PlayerPrefs.SetInt(_id, value ? 1 : 0);

    // Ownership
    public static string GetOwnedKey(string gearId)
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
    public static GearItem GetGearById(string gearItemId)
    {
        return gearDatabase.GetById(gearItemId);
    }

    public static string GetSlotKey(GearSlot slot)
    {
        return _gearSlotPrefix + slot;
    }

    public static void SetEquipped(GearItem item)
    {
        PlayerPrefs.SetString(GetSlotKey(item.Slot), item.Id);
    }

    public static string GetEquippedId(GearSlot slot)
    {
        return PlayerPrefs.GetString(GetSlotKey(slot), empty);
    }

    public static GearItem GetEquippedGun()
    {
        var _gunId = GetEquippedId(GearSlot.Gun);
        return GetGearById(_gunId);
    }

    public static GearItem GetEquippedGlasses()
    {
        var _glassesId = GetEquippedId(GearSlot.Glasses);
        return GetGearById(_glassesId);
    }

    public static void setEmptyGear(GearSlot slot)
    {
        PlayerPrefs.SetString(_gearSlotPrefix + slot.ToString(), empty);
    }
    // for debugging
    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
