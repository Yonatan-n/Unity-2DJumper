
using UnityEngine;

[CreateAssetMenu(menuName = "Gear/Gear Item")]
public class GearItem : ScriptableObject
{
    public string Id;
    public string title;
    public GearSlot Slot;
    public Sprite Icon;
    public Vector3 LocalRotation; // Optional for glasses
    public Vector3 LocalOffset;  // Optional alignment tweak
    public Vector3 LocalScale = Vector3.one;
    public bool flipX = false;
    public int Price; // gems
    // --- For Guns Only ---
    public float ReloadTime;
    public float FireRate;
    public int Magazine;
    public FireMode fireMode;
}

public enum FireMode
{
    Single, // 1911
    BinaryTrigger, // glonk
    FullAuto, // ak
    Burst3Shots, // mp3
}

public enum GearSlot
{
    Hat = 0,
    Glasses = 1,
    Gun = 2,
}

public enum GearButtonState
{
    Buyable = 0,
    Equipable = 1,
    Unequipable = 2,
}
