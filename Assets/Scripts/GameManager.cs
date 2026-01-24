using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Gun gun;
    public int Lives;
    public GameObject LivesCounter;
    public int Ammo;
    public GameObject AmmoCounter;
    public int Coins;
    public GameObject CoinsCounter;
    [SerializeField] AudioClip reload1911;
    [SerializeField] AudioClip fire1911;

    [SerializeField] AudioClip reloadRevolver;
    [SerializeField] AudioClip fireRevolver;
    [SerializeField] AudioClip reloadGlonk;
    [SerializeField] AudioClip fireGlonk;

    [SerializeField] AudioClip reloadAK;
    [SerializeField] AudioClip fireAK;

    public IReadOnlyList<Gun> Guns;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        Guns = new[]{
            new Gun(GunType.C_1911, 7, 2.5f, reload1911, fire1911),
            new Gun(GunType.Revolver, 5, 4f, reloadRevolver, fireRevolver),
            new Gun(GunType.Glonk, 15, 3f, reloadGlonk, fireGlonk),
            new Gun(GunType.AK, 30, 3f, reloadAK, fireAK),
        };

        gun = Guns[0];
        reloadAmmo();
        Coins = 0;
        Lives = 1;


    }

    public void updateAllCounters()
    {
        updateAmmo();
        updateCoins();
        updateLives();
    }
    public void GreyOutInAmmo(bool start)
    {
        if (!start)
        {
            reloadAmmo();
            updateAmmo();
        }
        var color = start ? Color.black : Color.white;
        var img = AmmoCounter.GetComponentInChildren<Image>();
        var text = AmmoCounter.GetComponentInChildren<TextMeshProUGUI>();
        img.color = color;
        text.color = color;
    }

    public void reloadAmmo()
    {
        Ammo = gun.BulletCount;
    }
    public void updateAmmo()
    {
        updateCounter(AmmoCounter, Ammo);
    }
    public void updateLives()
    {
        updateCounter(LivesCounter, Lives);
    }
    public void updateCoins()
    {
        updateCounter(CoinsCounter, Coins);
    }
    public void updateCounter(GameObject counter, int value)
    {
        var text = counter.GetComponentInChildren<TextMeshProUGUI>();
        text.text = value.ToString();
    }

    void Update()
    {

    }
}

public enum GunType
{
    C_1911, Glonk, AK, Revolver,
}
public record Gun(GunType Type, int BulletCount, float reloadTimeSeconds, AudioClip reloadSound, AudioClip ShootSound);