using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : Singleton<Spawner>
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
            {carPrefab, null},
        };
        obstacleMapKeys = obstacleMap.Keys.ToListPooled();
        obstacles = new List<GameObject>();
        spawnRandomObstacle();
        // TODO:
        // HUD for lives, mag, coins, distance, best distance
        // make game over screen / panel
        // add distance (time * speed maybe)
        // kil enemy add coins (50)
        // passed obstecle add coins (10?)
        // extra live cost 200, 400, 800...
        // +1 mag cost 150, 300, 600, 1200
        // double jump cost: 800, 1600, 3200
        // key: 1000, 2 keys 1800
        // 1911 costs 1 key (1 by default)
        // glock costs 2-3 keys
        // revolver costs 1 key (not sure what is speical about it, maybe double money)
        // AK consts 4 keys
        // sunglasses costs 1 key maybe 2 keys for some options
        // hat costs 1 key maybe 2 keys for some options
        // after live, coins, add progression to stage color + timer,
        // then add the store in between stage transitions
        // STORE item maybe:
        // every 3 stages it is repeated
        // 1st store: live, +1 mag, random skin, jump/smash/jetpack/ double coins/
        // 2nd store: key, +2 mag, double jump
        // 3rd store: buy 2keys, trade keys for coins (for end game), live, gun unlock
        //  after that, flying enemies that need to be jump-shooted, they could shoot at the player with some timer and aim indication

        // option page:
        // volume control

        // gear page:
        // keys counter
        // hat hat hat hat
        // glasses glasses glasses glasses
        // gun gun gun gun
        // <button>back to main menu</button>

        // main menu 
        // new background
        // rabbit sprite, maybe with a gun
        // funny random sentences generator (maybe just 10 to start), with speech bubble
        //

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

        // main menu
        // have skins/eqimpents page, to buy, and equipe different stuff hats etc
        // have the rabbit with a speach bubble say good advice like "eat you veggies!", "stay in school!"
        // drink more water! stay hydraded!
        // it is recomended to get about 7-8 hours of sleep each night to improve brain functions
        // go outside, touch the grass, ain't that nice!
        // always stay please, and thank you. thank you!

        // and after you unlock the first gun he will also say the firearms safty rules "keep your finger off the trigger until you are ready to fire" etc
        // be sure to keep you gun lubrecated and well maintaiend to prevent malfunctions!
        // god I love the blood
        // .45 - god's caliber
        //   

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
        // var randomY = Random.Range(minY, maxY);
        var randomNumberPrecentage = Random.Range(0, 100);
        GameObject randomFab;
        if (randomNumberPrecentage < enemyPercentage)
        {
            // spawn enemy
            randomFab = ChooseRandom(enemiesPrefabs);
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
        // var position = new Vector3(transform.position.x, randomY, 0);
        var obs = Instantiate(randomFab, transform.position, transform.rotation);
        obstacles.Add(obs);
        setNextSpawnRate();
    }

    public void ShootObstacleRemove(GameObject obstacle)
    {
        var obs = obstacle.GetComponent<Obstacle>();
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
