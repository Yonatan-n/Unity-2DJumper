using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : ParentAwareSingleton<Spawner>
{
    [SerializeField] GameObject barrelPrefab;
    [SerializeField] GameObject treePrefab;
    [SerializeField] GameObject carPrefab;
    [SerializeField] GameObject EnemyWalkerPrefab;
    [SerializeField] GameObject EnemyWalkerShield;
    [SerializeField] GameObject EnemyFlying;
    [SerializeField] GameObject EnemyFlyingShield;
    [SerializeField] List<Sprite> barrelsSprites;
    [SerializeField] List<Sprite> treesSprites;
    [SerializeField] List<GameObject> carsPrefabs;
    [SerializeField] float spawnRateMin = 2;
    [SerializeField] float spawnRateMax = 6;
    private float spawnRate;
    [SerializeField] Transform FlyingSpawnPoint;
    private List<GameObject> obstacles;
    private Dictionary<GameObject, List<Sprite>> obstacleMap;
    private List<GameObject> enemiesPrefabs;
    List<GameObject> obstacleMapKeys;
    private float spawnTimer; // gives 3 seconds of no enemies at the start if IsStartingGrace
    private int enemyPercentage = 20; // 20; // 20, 40, 60, 80
    private int shieldPercentage = 0; // 0, 15, 30, 45, 60, 75
    private readonly int maxShieldPercentage = 75;
    // private int maxPercentage = 100;
    private readonly int maxEnemyPercentage = 80;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var isStartingGrace = PlayerData.GetBoolById(PlayerData.IsStartingGrace);
        spawnTimer = isStartingGrace ? -GameManager.Instance.GraceTime : 0;
        enemiesPrefabs = new List<GameObject>
        {
            EnemyWalkerPrefab, // default
            // EnemyFlying,
            //EnemyWalkerShield
            // adding later enemyFlyingPrefab,
        };

        obstacleMap = new Dictionary<GameObject, List<Sprite>>
        {
            {barrelPrefab, barrelsSprites},
            {treePrefab, treesSprites},
            {carPrefab, null},
        };
        obstacleMapKeys = obstacleMap.Keys.ToListPooled();
        obstacles = new List<GameObject>();
        // spawnRandomObstacle();
        // TODO
        // option page:
        // volume control

        // main menu 
        // funny random sentences generator (maybe just 10 to start), with speech bubble

        // Then,
        // particles effect when destroying obstacles, maybe set the color manually for now there aren't many

        // for later

        // LATER: unlock 2nd female rabbit + small rabbit kids,they can be a sort of permanently extra lives for free

        //  powerups

        // FMJ bullet, can shoot trought multiple enemies and obstacles
        // jump down crush? like hold the button and then kill what you land on 

        // main menu
        // have the rabbit with a speach bubble say good advice like "eat you veggies!", "stay in school!"
        // drink more water! stay hydraded!
        // it is recomended to get about 7-8 hours of sleep each night to improve brain functions
        // go outside, touch the grass, ain't that nice!
        // always stay please, and thank you. thank you!

        // and after you unlock the first gun he will also say the firearms safty rules "keep your finger off the trigger until you are ready to fire" etc
        // be sure to keep you gun lubrecated and well maintaiend to prevent malfunctions!
        // god I love the blood
        // .45 - god's caliber
    }


    bool _flyingUnlocked = false;
    public void AddFlyingEnemies()
    {
        Debug.Log("Adding Flying enemies");
        if (_flyingUnlocked) return;
        enemiesPrefabs.Add(EnemyFlying);
        _flyingUnlocked = true;
    }
    bool _ShieldedUnlocked = false;
    public void AddShieldedEnemies()
    {
        Debug.Log("Adding Shielded enemies");
        if (_ShieldedUnlocked) return;
        // enemiesPrefabs.Add(enemyFlyingPrefab);
        _ShieldedUnlocked = true;
    }
    void setNextSpawnRate()
    {
        spawnRate = Random.Range(spawnRateMin, spawnRateMax + 1);
    }

    void IncreaseSpawnRate()
    {
        if (spawnRateMax > spawnRateMin) spawnRateMax--;
    }
    void IncreaseEnemyPercentage()
    {
        if (enemyPercentage < maxEnemyPercentage)
            enemyPercentage += 20;
        if (shieldPercentage < maxShieldPercentage) // unlocks  at level 1 (starts at 0)
            shieldPercentage += 15;
    }

    public void StartNewLevel(int level)
    {
        IncreaseSpawnRate();
        IncreaseEnemyPercentage();
        if (level == 1) AddShieldedEnemies();
        else if (level == 2) AddFlyingEnemies();

    }

    public static T ChooseRandom<T>(IList<T> collection)
    {
        return collection[Random.Range(0, collection.Count)];
    }

    public static bool RandomBool()
    {
        return Random.value > 0.5f;
    }
    private GameObject TryPromoteToShielded(GameObject fab)
    {
        if (Random.Range(0, 100) > shieldPercentage) return fab; // no shield
        if (fab == EnemyWalkerPrefab) return EnemyWalkerShield;
        if (fab == EnemyFlying) return EnemyFlyingShield;
        // should never happen
        return fab;
    }
    void spawnRandomObstacle()
    {
        GameObject randomFab;
        var _transform = transform;
        if (Random.Range(0, 100) < enemyPercentage)
        {
            // spawn enemy
            randomFab = TryPromoteToShielded(ChooseRandom(enemiesPrefabs));
            if (randomFab.CompareTag(Tags.FlyingEnemy))
                _transform = FlyingSpawnPoint;
        }
        else
        {
            // spawn obstacle
            randomFab = ChooseRandom(obstacleMapKeys);
            if (randomFab == carPrefab)
            {
                randomFab = ChooseRandom(carsPrefabs);
                var sr = randomFab.GetComponent<SpriteRenderer>();
                sr.flipX = RandomBool();
            }
            else
            {
                var sr = randomFab.GetComponent<SpriteRenderer>();
                sr.sprite = ChooseRandom(obstacleMap[randomFab]);
                sr.flipX = RandomBool();
            }
        }
        var position = new Vector3(
            Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + 1f,
             _transform.position.y, _transform.position.z
        );
        var obs = Instantiate(randomFab, position, _transform.rotation);
        obstacles.Add(obs);
        var enemy = obs.GetComponent<Enemy>();
        enemy = enemy != null ? enemy : obs.GetComponentInChildren<Enemy>();
        // enemy could be null for non enemy obstacles
        if (randomFab == EnemyWalkerPrefab || randomFab == EnemyFlying)
            enemy.Init(1, 0);
        else if (randomFab == EnemyWalkerShield || randomFab == EnemyFlyingShield)
            enemy.Init(1, 1);
        setNextSpawnRate();
    }

    public void ShootObstacleRemove(GameObject obstacle)
    {
        var obs = obstacle.GetComponent<Obstacle>();
        obs.WasShotted = true;
        obstacles.Remove(obstacle);
    }

    void Update()
    {
        spawnTimer += Time.deltaTime; // resets after each obstacle spawn 
        if (spawnTimer > spawnRate && !GameManager.Instance.levelEnd)
        {
            spawnRandomObstacle();
            spawnTimer = 0;
        }
    }
    public void DestroyAllObstacles()
    {
        foreach (var obs in obstacles)
            Destroy(obs);
        obstacles.Clear();

    }
}
