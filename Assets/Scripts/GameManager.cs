using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public Gun gun;
    public int lives = 1;
    public GameObject LivesCounter;
    public int ammo;
    public GameObject AmmoCounter;
    public int coins;
    public GameObject CoinsCounter;
    [SerializeField] AudioClip reload1911;
    [SerializeField] AudioClip fire1911;

    [SerializeField] AudioClip reloadRevolver;
    [SerializeField] AudioClip fireRevolver;
    [SerializeField] AudioClip reloadGlonk;
    [SerializeField] AudioClip fireGlonk;

    [SerializeField] AudioClip reloadAK;
    [SerializeField] AudioClip fireAK;
    [SerializeField] GameObject ObstaclesSpawner;
    [SerializeField] GameObject player;
    public bool levelEnd;
    private float timer;
    public float Timer
    {
        get { return timer; }
        set
        {
            timer = value;
            updateMeters();
        }
    }
    public GameObject Meters;

    public IReadOnlyList<Gun> guns;
    public int level;
    void Start()
    {
        level = 1;
        guns = new[]{
            new Gun(GunType.C_1911, 7, 2.3f, reload1911, fire1911),
            new Gun(GunType.Revolver, 5, 4f, reloadRevolver, fireRevolver),
            new Gun(GunType.Glonk, 15, 3f, reloadGlonk, fireGlonk),
            new Gun(GunType.AK, 30, 3f, reloadAK, fireAK),
        };

        gun = guns[0];
        reloadAmmo();
        coins = (int)CoinsEarned.Obstacle;
        lives = 1;
        updateAllCounters();
    }

    public void updateMeters()
    {
        if (levelEnd) return;
        var text = Meters.GetComponentInChildren<TextMeshProUGUI>();
        text.text = timerToMeters().ToString() + "M";
    }

    private int timerToMeters()
    {
        // change 8 to have nicer scaling for levels
        // level 1: 0-1,000
        // level 2 1,000-3,000
        // level3: 3000-6000
        return (int)(timer * 8f);
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
        var color = start ? Color.gray : Color.white;
        var img = AmmoCounter.GetComponentInChildren<Image>();
        var text = AmmoCounter.GetComponentInChildren<TextMeshProUGUI>();
        img.color = color;
        text.color = color;
    }
    public int Ammo
    {
        get { return ammo; }
        set
        {
            ammo = value;
            updateAmmo();
        }
    }
    public int Coins
    {
        get { return coins; }
        set
        {
            coins = value;
            updateCoins();
        }
    }
    public int Lives
    {
        get { return lives; }
        set
        {
            lives = value;
            updateLives();
        }
    }
    public void reloadAmmo()
    {
        ammo = gun.BulletCount;
    }
    public void updateAmmo()
    {
        updateCounter(AmmoCounter, ammo);
    }
    public void updateLives()
    {
        updateCounter(LivesCounter, Lives);
        if (Lives <= 0)
        {
            GameOver();
        }
    }
    void GameOver()
    {
        AudioManager.Instance.PlayGameOverSound();
        PauseGame.Instance.Pause(PausePanel.showGameOverPanel); // stop buttons, movments for now
        // show gameover ui
    }
    public void earnedCoins(CoinsEarned earned)
    {
        AudioManager.Instance.CoinPickUp();
        Coins += (int)earned;
        updateCoins();
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
        timer += Time.deltaTime;
        updateMeters();
        if (timerToMeters() >= 50 * level && !levelEnd)
        {
            StartCoroutine(NextLevel());
        }
    }

    private IEnumerator NextLevel()
    {
        levelEnd = true;
        var spawner = ObstaclesSpawner.GetComponent<Spawner>();
        var playerScript = player.GetComponent<Player>();
        spawner.DestroyAllObstacles();
        playerScript.NextLevel();
        // ObstaclesSpawner.SetActive(false);
        yield return null;
    }
}

public enum CoinsEarned
{
    Obstacle = 10,
    Enemy = 50,
    FlyingEnemy = 100,
    JumpOver = 10,
}

public enum GunType
{
    C_1911, Glonk, AK, Revolver,
}
public record Gun(GunType Type, int BulletCount, float reloadTimeSeconds, AudioClip reloadSound, AudioClip ShootSound);