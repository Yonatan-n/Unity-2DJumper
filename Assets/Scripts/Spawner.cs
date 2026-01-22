using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject barrelPrefab;
    [SerializeField] GameObject treePrefab;
    [SerializeField] GameObject carPrefab;

    [SerializeField] List<Sprite> barrelsSprites;
    [SerializeField] List<Sprite> treesSprites;
    [SerializeField] List<Sprite> carsSprites;
    [SerializeField] float spawnRate = 3;
    [SerializeField] float deadZone = -45f;
    private List<GameObject> obstacles;
    private Dictionary<GameObject, List<Sprite>> obstacleMap;
    List<GameObject> obstacleMapKeys;
    private float timer = 0;
    private System.Random random;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        random = new System.Random();
        obstacleMap = new Dictionary<GameObject, List<Sprite>>
        {
            {barrelPrefab, barrelsSprites},
            {treePrefab, treesSprites},
            {carPrefab, carsSprites},
        };
        obstacleMapKeys = obstacleMap.Keys.ToListPooled();
        obstacles = new List<GameObject>();
        spawnRandomObstacle();
        // TODO: add cars, trees, select randmoly each time random range 3
        // random range when spawning, not every 2 seconds but something like 1.5-5 maybe
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
        //  
    }
    Sprite getRandomSprite(List<Sprite> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    GameObject SelectRandomFab()
    {
        return obstacleMapKeys[Random.Range(0, obstacleMapKeys.Count)];
    }
    void spawnRandomObstacle()
    {
        // var randomY = Random.Range(minY, maxY);
        // var position = new Vector3(transform.position.x, randomY, 0);
        var randomFab = SelectRandomFab();
        var randomSprite = getRandomSprite(obstacleMap[randomFab]);
        var sr = randomFab.GetComponent<SpriteRenderer>();
        sr.sprite = randomSprite;
        var obs = Instantiate(randomFab, transform.position, transform.rotation);
        obstacles.Add(obs);
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnRate)
        {
            spawnRandomObstacle();
            timer = 0;
        }

        if (obstacles.Count > 0 && obstacles[0].transform.position.x < deadZone)
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
