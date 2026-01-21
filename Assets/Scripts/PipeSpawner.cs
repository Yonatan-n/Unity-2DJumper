using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [SerializeField] GameObject pipe;
    [SerializeField] float spawnRate = 2;
    [SerializeField] float offsetY = 10;
    private float timer = 0;
    void Start()
    {
        SpawnPipe();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnRate)
        {
            SpawnPipe();
            timer = 0;
        }

    }

    private void SpawnPipe()
    {
        var minY = transform.position.y - offsetY;
        var maxY = transform.position.y + offsetY;
        var randomY = Random.Range(minY, maxY);
        var position = new Vector3(transform.position.x, randomY, 0);
        Instantiate(pipe, position, transform.rotation);
    }
}
