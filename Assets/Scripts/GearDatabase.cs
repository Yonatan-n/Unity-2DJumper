using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Gear/Gear Database")]
public class GearDatabase : ScriptableObject
{
    public List<GearItem> Items = new List<GearItem>();

    public GearItem GetById(string id)
    {
        return Items.Find(i => i.Id == id);
    }

    public List<GearItem> GetBySlot(GearSlot slot)
    {
        return Items.FindAll(i => i.Slot == slot);
    }
}
