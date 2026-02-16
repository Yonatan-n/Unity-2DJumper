using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : ParentAwareSingleton<Spawner>
{
    [SerializeField] GameObject barrelPrefab;
    [SerializeField] GameObject treePrefab;
    [SerializeField] GameObject carPrefab;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject enemyFlyingPrefab;
    bool isFlyingEnemiesUnlocked = false;
    [SerializeField] float unlockFlyingEnemiesAfterSec = 60f; // seconds

    [SerializeField] List<Sprite> barrelsSprites;
    [SerializeField] List<Sprite> treesSprites;
    [SerializeField] List<GameObject> carsPrefabs;
    [SerializeField] float spawnRateMin = 2;
    [SerializeField] float spawnRateMax = 6;
    private float spawnRate;
    [SerializeField] Transform FlyingSpawnPoint;
    [SerializeField] float deadZone = -45f;
    private List<GameObject> obstacles;
    private Dictionary<GameObject, List<Sprite>> obstacleMap;
    private List<GameObject> enemiesPrefabs;
    List<GameObject> obstacleMapKeys;
    private float spawnTimer = 0;
    private float totalTimer = 0;
    private int enemyPercentage = 20; // 20%
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isFlyingEnemiesUnlocked = true; // TODO: temp
        enemiesPrefabs = new List<GameObject>
        {
            enemyPrefab, // default
            enemyFlyingPrefab,
        };

        obstacleMap = new Dictionary<GameObject, List<Sprite>>
        {
            {barrelPrefab, barrelsSprites},
            {treePrefab, treesSprites},
            {carPrefab, null},
        };
        obstacleMapKeys = obstacleMap.Keys.ToListPooled();
        obstacles = new List<GameObject>();
        spawnRandomObstacle();
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
    public void IncreaseEnemyPercentage()
    {
        enemyPercentage += 20;
    }

    void setNextSpawnRate()
    {
        spawnRate = Random.Range(spawnRateMin, spawnRateMax + 1);
    }

    public static T ChooseRandom<T>(IList<T> collection)
    {
        return collection[Random.Range(0, collection.Count)];
    }

    public static bool RandomBool()
    {
        return Random.value > 0.5f;
    }
    void spawnRandomObstacle()
    {
        var position = transform.position;
        var rotation = transform.rotation;
        var randomNumberPercentage = Random.Range(0, 100);
        randomNumberPercentage = 0;
        GameObject randomFab;
        if (randomNumberPercentage < enemyPercentage)
        {
            // spawn enemy
            randomFab = ChooseRandom(enemiesPrefabs);
            randomFab = enemiesPrefabs[1];
            if (randomFab.tag == Tags.FlyingEnemy)
            {
                position = FlyingSpawnPoint.position;
                rotation = FlyingSpawnPoint.rotation;
            }
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
        var obs = Instantiate(randomFab, position, rotation);
        obstacles.Add(obs);
        setNextSpawnRate();
    }

    public void ShootObstacleRemove(GameObject obstacle)
    {
        Obstacle obs;
        obs = obstacle.GetComponent<Obstacle>();
        if (obs == null) obs = obstacle.GetComponentInParent<Obstacle>();
        obstacles.Remove(obstacle);
        obs.Destroyed(true);
    }

    void Update()
    {
        totalTimer += Time.deltaTime; // resets after game over
        spawnTimer += Time.deltaTime; // resets after each obstacle spawn 
        if (spawnTimer > spawnRate && !GameManager.Instance.levelEnd)
        {
            spawnRandomObstacle();
            spawnTimer = 0;
        }

        // todo remove false
        if (false && totalTimer > unlockFlyingEnemiesAfterSec && !isFlyingEnemiesUnlocked)
        {
            isFlyingEnemiesUnlocked = true;
            enemiesPrefabs.Add(enemyFlyingPrefab);
        }

        if (obstacles.Count > 0 && obstacles[0] != null && obstacles[0].transform.position.x < deadZone)
        {
            // only need to check first one every time
            // if player collieded, no need to destroy, just remove from the list
            if (obstacles[0])
            {
                var obs = obstacles[0].GetComponent<Obstacle>();
                obs.Destroyed(false);
            }
            obstacles.RemoveAt(0);
        }
    }
    public void DestroyAllObstacles()
    {
        foreach (var obs in obstacles)
            Destroy(obs);
        obstacles.Clear();

    }
}
