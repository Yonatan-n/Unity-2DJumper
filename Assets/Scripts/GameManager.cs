using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : ParentAwareSingleton<GameManager>
{
    public Gun gun;
    public int lives = 1;
    public int maxJumps = 1; // can buy more later
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
    [SerializeField] GameObject CurrentBackground;
    [SerializeField] GameObject[] BackgroundPrefabs;

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
    public readonly int TOTAL_LEVELS = 3;

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitGame();
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitGame();
    }
    private void InitGame()
    {
        level = 0;
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
        maxJumps = 1;
        updateAllCounters();
        SwitchBackground();
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
        levelEnd = true;
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

        // if (timerToMeters() >= 1000 + (1000 * level) && !levelEnd)
        if (timerToMeters() >= 30 && !levelEnd)
        {
            StartCoroutine(FinishCurrentLevel());
        }
    }

    private IEnumerator FinishCurrentLevel()
    {
        levelEnd = true;
        var spawner = ObstaclesSpawner.GetComponent<Spawner>();
        var playerScript = player.GetComponent<Player>();
        spawner.DestroyAllObstacles();
        yield return playerScript.ExitRight();
        Debug.Log("player right of scene");
        playerScript.HideButtons(true);
        SceneLoader.Instance.JustFadeOut();
        // TODO:
        // fade to black after player exited
        // show a shop model, buy +1 live, +1jump that's it for now
        // after shop over, fade back to new scene (new background), then animate player 
        // to enter from the LEFT
        // then start spawner
        // also, on load, add more enemies, longer distance, etc

        // level 2 is mostly level 1 with minor tweaks:
        // * darker background for night
        // * 2K instead of 1K duration
        // * 40% enemies, instead of 20%
        yield return null;
    }
    public void LoadNextLevel()
    {
        StartCoroutine(LoadNextLevelRoutine());
    }

    private IEnumerator LoadNextLevelRoutine()
    {
        levelEnd = false;
        level++;
        SwitchBackground();

        yield return null;
    }

    void SwitchBackground()
    {
        if (CurrentBackground != null)
            Destroy(CurrentBackground);
        CurrentBackground = Instantiate(
            BackgroundPrefabs[level % TOTAL_LEVELS],
            new Vector3(0, 0, 10), Quaternion.identity
        );
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