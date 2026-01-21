using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    [SerializeField] GameObject floorFab;
    [SerializeField] float spawnRate = 1;
    private float timer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var from = -30;
        var to = 30;
        var floorRenderer = floorFab.GetComponent<SpriteRenderer>();
        var width = floorRenderer.sprite.rect.width / 100;
        int amount = (int)Mathf.Ceil((to - from) / width);
        for (int i = 0; i < amount; i++)
        {
            var x = from + (i * width);
            var position = new Vector3(x, transform.position.y, 0);
            Instantiate(floorFab, position, transform.rotation);
        }

    }
    void SpawnFloor()
    {
        // var minY = transform.position.y - offsetY;
        // var maxY = transform.position.y + offsetY;
        // var randomY = Random.Range(minY, maxY);
        var position = new Vector3(transform.position.x, transform.position.y, 0);
        Instantiate(floorFab, position, transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnRate)
        {
            SpawnFloor();
            timer = 0;
        }
    }
}
