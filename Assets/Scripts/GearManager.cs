using UnityEngine;


public class GearManager : Singleton<GearManager>
{

    Player _player;


    void setPlayer(Player player)
    {
        _player = player;
    }

    void ApplyGear(GearSlot slot, SpriteRenderer sprite)
    {
        //
    }
}


