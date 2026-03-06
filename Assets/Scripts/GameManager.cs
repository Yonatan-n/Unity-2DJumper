using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : ParentAwareSingleton<GameManager>
{
    public readonly float GraceTime = 3f;
    public int lives = 1;
    public int maxJumps = 1; // can buy more later
    public GameObject LivesCounter;
    public int ammo;
    public int MaxAmmo;
    public GameObject AmmoCounter;
    public int coins;
    public GameObject CoinsCounter;
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
    public readonly int MAX_MOVE_LEFT = 6;
    public int MoveLeftBought
    {
        get { return _movesLeftNumber; }
        set
        {
            _movesLeftNumber = value;
            var playerScript = player.GetComponent<Player>();
            playerScript.StartPositionX = _movesLeftNumber switch
            {
                1 => -10f,
                2 => -14f,
                3 => -18f,
                4 => -22f,
                5 => -24f,
                6 => -28f, // max
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
    public int level;
    private int totalDistance = 0;
    private Player PlayerScript;
    public bool ShowControlsText = true;

    void Start()
    {
        InitGame();
    }

    void resetTimer()
    {
        timer = 0;
    }
    private void InitGame()
    {
        StatsTracker.Instance.OnSessionStarted();
        PlayerScript = player.GetComponent<Player>();
        playerEquipment.LoadFromPlayerData();
        level = 0;
        levelLength = levelFactor;
        // levelLength = 5; // remove
        var sceneTransition = GameObject.Find("SceneTransition");
        if (sceneTransition == null) // testing only?
        {
            Instantiate(SceneTransition, Vector3.zero, Quaternion.identity);
        }
        var gun = PlayerData.GetEquippedGun();
        MaxAmmo = gun.Magazine;
        reloadAmmo();
        coins = 0;
        if (PlayerData.GetBoolById(PlayerData.isGodMode))
        {
            coins = 100000;
        }
        lives = 1;
        maxJumps = 1;
        updateAllCounters();
        resetTimer();
        CurrentBackground = Instantiate(
            BackgroundPrefabs[0],
            new Vector3(0, 0, 10), Quaternion.identity
        );
        StartCoroutine(SwitchBackground());
        StartCoroutine(PlayerScript.EnterLeft());
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
        ammo = MaxAmmo;
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
        StatsTracker.Instance.OnPlayerDied();
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
    public void earnedCoins(int earned)
    {
        StatsTracker.Instance.OnCoinCollected(earned);
        AudioManager.Instance.CoinPickUp();
        Coins += earned;
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
        if (!levelEnd)
        {
            updateMeters();
        }

        if (timerToMeters() >= levelLength && !levelEnd)
        {
            StartCoroutine(FinishCurrentLevel());
        }
    }

    private IEnumerator FinishCurrentLevel()
    {
        levelEnd = true;
        Spawner.Instance.DestroyAllObstacles();
        ObstaclesSpawner.SetActive(false);
        PlayerScript.SetButtons(false);
        yield return PlayerScript.ExitRight();
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
        StatsTracker.Instance.OnLevelCleared(level); // using the next one because starts at 0
        resetTimer();
        Spawner.Instance.StartNewLevel(level);
        levelLength = levelFactor * 2; // same always, just faster
        GroundMover.Instance.IncreaseSpeed();
        var playerScript = player.GetComponent<Player>();
        if (level > 0 && ShowControlsText)
        {
            ShowControlsText = false; // set to false after the first level
            playerScript.HideControlButtons();
        }
        playerScript.SetButtons(true); //
        yield return SwitchBackground(); // same duration as EnterLeft, don't yield to sync them
        yield return playerScript.EnterLeft();
        // yield return new WaitForSeconds(SwitchLevelDuration);
        var isStartingGrace = PlayerData.GetBoolById(PlayerData.IsStartingGrace);
        if (isStartingGrace)
            yield return new WaitForSeconds(GraceTime);

        ObstaclesSpawner.SetActive(true);
    }

    IEnumerator SwitchBackground()
    {
        var newBackground = Instantiate(
            BackgroundPrefabs[level % BackgroundPrefabs.Length],
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
    Obstacle, Enemy, FlyingEnemy, JumpOver,
}

public static class Tags
{
    public const string Enemy = "Enemy";
    public const string FlyingEnemy = "FlyingEnemy";
    public const string Bullet = "Bullet";
    public const string BulletFMJ = "BulletFMJ";
    public const string Player = "Player";
    public const string Obstacle = "Obstacle";
    public const string Ground = "Ground";
    public const string Shield = "Shield";

}
public static class GunsId
{
    public const string DEFAULT_1911 = "9";
    public const string GLONK = "10";
    public const string VSP = "11";
    public const string AK = "13";
    public const string MP3 = "14";
}

public enum RewardType
{
    Coins,
    Lives,
    Ammo,
}

// youtube hats / glass / guns? ask omer

// Maybe: make the first level a tutorial like, you have no gun, unlock it at the end and play sound "he's got a gun!" lol
// cutsecene:
// level start talking to a tree/barrel about his wife sending him to get a carrot cake
// level 1 gets hit by enemy (make it that you can't move away from him), pull up a gun from behind the back and shoot him
// "Move bitch I'm going to the store"
// level 2 flying enemy, sees him rabbit: "You again? how did you get up there?", 
// flying: "Hello friend, I have never seen a creature like yourself in my entire life"
// rabbit: "I don't have time for this shit" (jump and shoots him)
// level 3: suicide enemy, rabbit sees him coming from a far, 
// rabbit: "another one? I don't have time for more talking, this I'm going to the fucking store" (shoots him from afar before dialog)
// level 4: shielded enemy, rabbit sees him. shielded: "Hello fuzzy one, we are the protectors of-...", 
// rabbit: "huh? can't talk I'm going to the store", (shoot once)
// <shield disappear>, enemy: "We are protected by-...", rabbit: "you still breathing? STORE!" (mag dumps into him)
// level 5: big enemy split into small ones. big one: "Hello little o-..." 
// rabbit: "the fuck is your problem? are you made out of little versions of youerslfe or something?"
// big enemy: "Well actually I'm-..." (rabbit shoots, enemy splits)
// rabbit: "Huh, would you look at that" (shoots some more)
// final shop, buys carrot cake.
// scene, brings it back to the wife. rabbit: "Hello darling, I brought you the carrot cake you love! <3"
// wife: "Oh! so cool...", wife:"umm", wife:"I really want burgers now... O_o". (you won page, after that back to main menu)
// next runs, starts with you going to get the wife a burger, but this time she is coming with you, extra lives ar rabbits, etc

// enemy 100% kills you, then cut scene, rabbit, get's mad (comic angrey vfx), pulls out a gun from behind the back
// shoot the enemy, sound clip "he's got a gun!" and then you get the gun unlocked and gear unlocked
// extra rabbits for extra lives, female rabbit (maybe settings option to turn off brutality)
// STORY
// wants to buy carrot in the store, he can always get it but today there is a 1+1 sale
// getting the rabbit wife 
