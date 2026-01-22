using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject barrelPrefab;
    [SerializeField] List<Sprite> barrelsSprites;
    [SerializeField] List<Sprite> treesSprites;
    [SerializeField] List<Sprite> carsSprites;
    [SerializeField] float spawnRate = 3;
    [SerializeField] float deadZone = -45f;
    private List<GameObject> obstacles;
    private float timer = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        obstacles = new List<GameObject>();
        spawnObstacle(barrelPrefab);
    }
    Sprite getRandomSprite(List<Sprite> list)
    {
        return list[Random.Range(0, list.Count)];
    }
    void spawnObstacle(GameObject obstaclePrefab)
    {
        // var randomY = Random.Range(minY, maxY);
        // var position = new Vector3(transform.position.x, randomY, 0);
        var sr = obstaclePrefab.GetComponent<SpriteRenderer>();
        sr.sprite = getRandomSprite(barrelsSprites);
        var obs = Instantiate(obstaclePrefab, transform.position, transform.rotation);
        obstacles.Add(obs);
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnRate)
        {
            spawnObstacle(barrelPrefab);
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
