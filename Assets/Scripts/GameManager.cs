using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] PlayerEquipment playerEquipment;
    [SerializeField] GameObject CurrentBackground;
    [SerializeField] GameObject[] BackgroundPrefabs;
    [SerializeField] GameObject SceneTransition;
    [SerializeField] int levelFactor = 500;
    [SerializeField] int levelLength;
    public float SwitchLevelDuration = 2f;
    // ----- SHOP --------
    private int _movesLeftNumber = 0;
    public int extraBulletsBought = 0;
    public readonly int MAX_MOVE_LEFT = 3;
    public int MoveLeftBought
    {
        get { return _movesLeftNumber; }
        set
        {
            _movesLeftNumber = value;
            var playerScript = player.GetComponent<Player>();
            playerScript.StartPositionX = _movesLeftNumber switch
            {
                1 => -12f,
                2 => -20f,
                3 => -28f, // max
                _ => 0f // else, but really just 0
            };
        }
    }

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
    public GameObject Progress;
    public GameObject HighScoreTMP;

    public IReadOnlyList<Gun> guns;
    public int level;
    public readonly int TOTAL_LEVELS = 3;
    private int totalDistance = 0;

    void Start()
    {
        // SceneManager.sceneLoaded += OnSceneLoaded;
        InitGame();
    }

    void resetTimer()
    {
        timer = 0;
    }
    private void InitGame()
    {
        playerEquipment.LoadFromPlayerData();
        level = 0;
        levelLength = (level + 1) * levelFactor;
        // levelLength = 5; // remove
        var sceneTransition = GameObject.Find("SceneTransition");
        if (sceneTransition == null) // testing only?
        {
            Instantiate(SceneTransition, Vector3.zero, Quaternion.identity);
        }

        guns = new[]{
            new Gun(GunType.C_1911, 7, 2.3f, reload1911, fire1911),
            new Gun(GunType.Revolver, 5, 4f, reloadRevolver, fireRevolver),
            new Gun(GunType.Glonk, 15, 3f, reloadGlonk, fireGlonk),
            new Gun(GunType.AK, 30, 3f, reloadAK, fireAK),
        };

        gun = guns[0];
        reloadAmmo();
        coins = (int)CoinsEarned.Obstacle;
        if (PlayerData.GetBoolById(PlayerData.isGodMode))
        {
            coins = 100000;
        }
        lives = 1;
        maxJumps = 1;
        updateAllCounters();
        resetTimer();
        CurrentBackground = Instantiate(
            BackgroundPrefabs[TOTAL_LEVELS - 1],
            new Vector3(0, 0, 10), Quaternion.identity
        );
        StartCoroutine(SwitchBackground());
    }

    public void updateMeters()
    {
        if (levelEnd) return;
        var text = Meters.GetComponentInChildren<TextMeshProUGUI>();
        text.text = (levelLength - timerToMeters()).ToString() + "M";
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
        var progText = Progress.GetComponent<TextMeshProUGUI>();
        var currentScore = totalDistance + timerToMeters();
        progText.text = "Ran: " + currentScore.ToString() + "M";
        var highScoreTMP = HighScoreTMP.GetComponent<TextMeshProUGUI>();
        var highScore = PlayerData.GetIntById(PlayerData.highscoreId);
        if (currentScore > highScore)
        {
            PlayerData.SetIntById(PlayerData.highscoreId, currentScore);
            highScore = currentScore;
        }
        highScoreTMP.text = "Highscore: " + highScore.ToString() + "M";

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

        if (timerToMeters() >= levelLength && !levelEnd)
        {
            StartCoroutine(FinishCurrentLevel());
        }
    }

    private IEnumerator FinishCurrentLevel()
    {
        levelEnd = true;
        var spawner = ObstaclesSpawner.GetComponent<Spawner>();
        spawner.DestroyAllObstacles();
        ObstaclesSpawner.SetActive(false);
        var playerScript = player.GetComponent<Player>();
        playerScript.SetButtons(false);
        yield return playerScript.ExitRight();
        Debug.Log("player right of scene");
        // yield return SceneLoader.Instance.FadeOutEnum();
        PauseGame.Instance.Pause(PausePanel.showShopPanel); // will trigger the next level loading
        yield return null;
        // triggers: ShopManager.start()
    }
    public void LoadNextLevel()
    {
        StartCoroutine(LoadNextLevelRoutine());
    }

    private IEnumerator LoadNextLevelRoutine()
    {
        totalDistance += levelLength;
        levelEnd = false;
        level++;
        levelLength = (level + 1) * levelFactor;
        GroundMover.Instance.speed += 2;
        var playerScript = player.GetComponent<Player>();
        playerScript.SetButtons(true); //
        yield return SwitchBackground(); // same duration as EnterLeft, don't yield to sync them
        yield return playerScript.EnterLeft();
        // yield return new WaitForSeconds(SwitchLevelDuration);
        resetTimer();
        var spawner = ObstaclesSpawner.GetComponent<Spawner>();
        spawner.IncreaseEnemyPercentage();
        ObstaclesSpawner.SetActive(true);
    }

    IEnumerator SwitchBackground()
    {
        var newBackground = Instantiate(
            BackgroundPrefabs[level % TOTAL_LEVELS],
            new Vector3(0, 0, 10), Quaternion.identity
        );
        var newBGFade = newBackground.GetComponent<BackgroundFade>();
        var currentBGFade = CurrentBackground.GetComponent<BackgroundFade>();
        var duration = SwitchLevelDuration;
        newBGFade.Fade(0f, 1f, duration);
        currentBGFade.Fade(1f, 0f, duration);
        yield return new WaitForSeconds(duration);
        currentBGFade.StopFade();
        // after animation is complete
        if (CurrentBackground != null)
            Destroy(CurrentBackground);
        CurrentBackground = newBackground;

    }
}

public enum CoinsEarned
{
    Obstacle = 20,
    Enemy = 100,
    FlyingEnemy = 200,
    JumpOver = 20,
}

public enum GunType
{
    C_1911, Glonk, AK, Revolver,
}
public record Gun(GunType Type, int BulletCount, float reloadTimeSeconds, AudioClip reloadSound, AudioClip ShootSound);


// TODO:
// DONE fix phone resolution (black bars etc)
// add background music, looping or multiple songs
// Done add player walking animation 
// add jump/shoot particles effects 
// DONE add shoot screen shake (lol)
// Done add gear page shop thing, unlock skins with keys, have the guns be skins for now, no different behavior
// Done have the player (rabbit) in the gear and main menu
// DONE use audio settings slider or number input in options page
// have 10-20 funny sentences that the rabbit says in the main menu (4 rules of firearms safety, stay in school, etc)

// current resultion width 960, height 600

// 1. gear page
// 2. particals on shoot, jump, buy, buttons?
// change economy, add 30% more money or reduce prices
//  