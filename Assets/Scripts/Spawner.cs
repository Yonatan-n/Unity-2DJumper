using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
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
    [SerializeField] List<Sprite> carsSprites;
    [SerializeField] float spawnRateMin = 2;
    [SerializeField] float spawnRateMax = 6;
    private float spawnRate;
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
        enemiesPrefabs = new List<GameObject>
        {
            enemyPrefab // default
        };

        obstacleMap = new Dictionary<GameObject, List<Sprite>>
        {
            {barrelPrefab, barrelsSprites},
            {treePrefab, treesSprites},
            {carPrefab, carsSprites},
        };
        obstacleMapKeys = obstacleMap.Keys.ToListPooled();
        obstacles = new List<GameObject>();
        spawnRandomObstacle();
        // TODO:
        // add enemies
        // shoot enemies
        // HUD for lives, mag, coins, distance, best distance

        // Then,
        // add magazine size + reload + disable button until it finishes,
        // jump counter (1 by default, could get more)
        // speed up level as time goes on
        // particl effect when destroying obstacles, maybe set the color manually for now there aren't many
        //  

        // for later
        // biomes? maybe just minor recolor the ground and background should not be too hard
        // main menu
        // unlock skins + select skins page
        // get coins per distance, enemies killed / skipped
        // store pop up on death to buy power ups, more lives, double jump, 
        // LATER: unlock 2nd female rabbit + small rabbit kids,they can be a sort of permanently extra lives for free
        // LATER: when you hit something they will die lol
        // LATER: fix the background floor jumping mid section

        //  powerups
        // double jump
        // more lives
        // FMJ bullet, can shoot trought multiple enemies and obstacles
        // faster reload?
        // jump down crush? like hold the button and then kill what you land on
        // 
    }

    void setNextSpawnRate()
    {
        spawnRate = Random.Range(spawnRateMin, spawnRateMax + 1);

    }
    Sprite getRandomSprite(List<Sprite> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    GameObject SelectRandomFab()
    {
        return obstacleMapKeys[Random.Range(0, obstacleMapKeys.Count)];
    }

    GameObject SelectRandomEnemy()
    {
        return enemiesPrefabs[Random.Range(0, enemiesPrefabs.Count)];
    }
    void spawnRandomObstacle()
    {
        // var randomY = Random.Range(minY, maxY);
        var randomNumberPrecentage = Random.Range(0, 100);
        GameObject randomFab;
        if (randomNumberPrecentage < enemyPercentage)
        {
            // spawn enemy
            randomFab = SelectRandomEnemy();
        }
        else
        {
            // spawn obstacle
            randomFab = SelectRandomFab();
            var sr = randomFab.GetComponent<SpriteRenderer>();
            sr.sprite = getRandomSprite(obstacleMap[randomFab]);

        }
        // var position = new Vector3(transform.position.x, randomY, 0);
        var obs = Instantiate(randomFab, transform.position, transform.rotation);
        obstacles.Add(obs);
        setNextSpawnRate();
    }
    // Update is called once per frame
    void Update()
    {
        totalTimer += Time.deltaTime; // resets after game over
        spawnTimer += Time.deltaTime; // resets after each obstacle spawn 
        if (spawnTimer > spawnRate)
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
                Destroy(obstacles[0]);
            }
            obstacles.RemoveAt(0);
        }
    }
}
